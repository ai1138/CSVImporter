using Microsoft.EntityFrameworkCore;

public class TradingContext : DbContext
{
    public TradingContext(DbContextOptions<TradingContext> options) : base(options)
    {
    }

    public DbSet<Exchange> Exchanges { get; set; }
    public DbSet<Sector> Sectors { get; set; }
    public DbSet<Ticker> Tickers { get; set; }
    public DbSet<TickerExchange> TickerExchanges { get; set; }
    public DbSet<StockHistory> StockHistories { get; set; }
    public DbSet<Price> Prices { get; set; }
    public DbSet<Adjustment> Adjustments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Primary key for the Exchanges table
        modelBuilder.Entity<Exchange>()
            .HasKey(e => e.ExchangeID);

        // Primary key for the Sectors table
        modelBuilder.Entity<Sector>()
            .HasKey(s => s.SICCode);

        // Primary key for the Tickers table
        modelBuilder.Entity<Ticker>()
            .HasKey(t => t.PermaTicker);

        // Composite primary key for the TickerExchanges table
        modelBuilder.Entity<TickerExchange>()
            .HasKey(te => new { te.PermaTicker, te.ExchangeID });

        // Primary key for the StockHistory table
        // Assuming StockHistory uses PermTicker as a key, adjust if needed
        modelBuilder.Entity<StockHistory>()
            .HasKey(sh => sh.PermaTicker);

        // Primary key for the Prices table
        modelBuilder.Entity<Price>()
            .HasKey(p => p.PriceID);

        // Primary key for the Adjustments table
        modelBuilder.Entity<Adjustment>()
            .HasKey(a => a.AdjustmentID);

        // Foreign key relationships and other configurations
        modelBuilder.Entity<Ticker>()
            .HasOne(t => t.Sector)
            .WithMany(s => s.Tickers)
            .HasForeignKey(t => t.SICCode);

        modelBuilder.Entity<StockHistory>()
            .HasOne(sh => sh.Ticker)
            .WithMany(t => t.StockHistories)
            .HasForeignKey(sh => sh.PermaTicker);

        modelBuilder.Entity<Price>()
            .HasOne(p => p.Ticker)
            .WithMany(t => t.Prices)
            .HasForeignKey(p => p.TickerSymbol)
            .HasPrincipalKey(t => t.TickerSymbol);

        modelBuilder.Entity<Adjustment>()
            .HasOne(a => a.Price)
            .WithMany(p => p.Adjustments)
            .HasForeignKey(a => a.PriceID);

        // Index configurations
        modelBuilder.Entity<Ticker>()
            .HasIndex(t => t.TickerSymbol)
            .IsUnique();

        modelBuilder.Entity<Exchange>()
            .HasIndex(e => e.ExchangeName);

        modelBuilder.Entity<Sector>()
            .HasIndex(s => s.SICSector);
    }


}
