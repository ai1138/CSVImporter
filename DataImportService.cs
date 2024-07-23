using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

public class DataImportService : BackgroundService
{
    private readonly ILogger<DataImportService> _logger;
    private readonly IServiceProvider _serviceProvider;
    private static readonly string TickerUrl = "https://www.alphaforge.net/A0B1C3/TICKERS.zip";
    private static readonly string PriceUrl = "https://www.alphaforge.net/A0B1C3/PRICES.zip";

    public DataImportService(ILogger<DataImportService> logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                _logger.LogInformation("Starting data download and import process.");
                await DownloadAndImportAsync();
                _logger.LogInformation("Data import process completed successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred during the data import process: {ex.Message}");
            }
            await Task.Delay(TimeSpan.FromHours(24), stoppingToken); // Repeat every 24 hours
        }
    }

    private async Task DownloadAndImportAsync()
    {
        var tickerStream = await DownloadAndExtractCsvStreamAsync(TickerUrl);
        var priceStream = await DownloadAndExtractCsvStreamAsync(PriceUrl);
        await ImportDataToDatabaseAsync(tickerStream, priceStream);
    }

    private async Task<Stream> DownloadAndExtractCsvStreamAsync(string url)
    {
        using HttpClient client = new HttpClient();
        string tempZipFilePath = Path.GetTempFileName(); // Temp file for ZIP

        try
        {
            using (var stream = await client.GetStreamAsync(url))
            using (var fileStream = new FileStream(tempZipFilePath, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                await stream.CopyToAsync(fileStream);
            }

            var archive = new ZipArchive(new FileStream(tempZipFilePath, FileMode.Open, FileAccess.Read), ZipArchiveMode.Read);
            foreach (ZipArchiveEntry entry in archive.Entries)
            {
                if (entry.FullName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
                {
                    string tempCsvFilePath = Path.GetTempFileName();
                    entry.ExtractToFile(tempCsvFilePath, overwrite: true);
                    return new FileStream(tempCsvFilePath, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, FileOptions.DeleteOnClose);
                }
            }
        }
        finally
        {
            if (File.Exists(tempZipFilePath))
            {
                File.Delete(tempZipFilePath);  // Ensure temp file is deleted to free up space
            }
        }

        throw new InvalidOperationException("No CSV file found in the downloaded ZIP.");
    }

    private async Task ImportDataToDatabaseAsync(Stream tickerStream, Stream priceStream)
    {
        using var scope = _serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<TradingContext>();

        dbContext.ChangeTracker.AutoDetectChangesEnabled = false; // Optimize performance for bulk inserts

        await ImportCsvData<Ticker>(dbContext, tickerStream);
        await ImportCsvData<Price>(dbContext, priceStream);

        dbContext.ChangeTracker.AutoDetectChangesEnabled = true; // Re-enable change tracking after bulk operations
    }

    private async Task ImportCsvData<T>(DbContext dbContext, Stream stream) where T : class
    {
        var modelType = typeof(T);
        var keyNames = dbContext.Model.FindEntityType(modelType).FindPrimaryKey().Properties
                        .Select(p => p.Name).ToList();

        using var reader = new StreamReader(stream);
        using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HeaderValidated = null,
            MissingFieldFound = null
        });

        var records = new List<T>();
        await foreach (var record in csv.GetRecordsAsync<T>())
        {
            bool isValid = true;
            foreach (var keyName in keyNames)
            {
                var prop = modelType.GetProperty(keyName);
                if (prop == null || prop.GetValue(record) == null || String.IsNullOrEmpty(prop.GetValue(record).ToString()))
                {
                    isValid = false;
                    break;
                }
            }

            if (isValid)
            {
                records.Add(record);
            }
        }

        dbContext.Set<T>().AddRange(records);
        await dbContext.SaveChangesAsync();
    }
}
