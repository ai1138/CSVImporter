using System;
using System.Collections.Generic;
using CsvHelper.Configuration.Attributes;

public class Exchange
{
    [Name("exchangeid")]
    public int ExchangeID { get; set; }

    [Name("exchangename")]
    public string ExchangeName { get; set; }

    public List<TickerExchange> TickerExchanges { get; set; }
}

public class Sector
{
    public Guid SIID { get; set; } = Guid.NewGuid();    
    
    [Name("siccode")]
    public int SICCode { get; set; }

    [Name("sicsector")]
    public string SICSector { get; set; }

    public List<Ticker> Tickers { get; set; }
}

public class Ticker
{
    [Name("permaticker")]
    public int PermaTicker { get; set; }

    [Name("ticker")]
    public string TickerSymbol { get; set; }

    [Name("name")]
    public string Name { get; set; }

    [Name("category")]
    public string Category { get; set; }

    [Name("cusips")]
    public string CUSIPs { get; set; }

    [Name("siccode")]
    public int SICCode { get; set; }

    [Name("location")]
    public string Location { get; set; }

    [Name("companysite")]
    public string CompanySite { get; set; }

    public Sector Sector { get; set; }
    public List<TickerExchange> TickerExchanges { get; set; }
    public List<StockHistory> StockHistories { get; set; }
    public List<Price> Prices { get; set; }
}

public class TickerExchange
{
    [Name("permaticker")]
    public int PermaTicker { get; set; }

    [Name("exchangeid")]
    public int ExchangeID { get; set; }

    [Name("isdelisted")]
    public char IsDelisted { get; set; }

    public Ticker Ticker { get; set; }
    public Exchange Exchange { get; set; }
}

public class StockHistory
{
    [Name("permaticker")]
    public int PermaTicker { get; set; }

    [Name("lastupdated")]
    public DateTime LastUpdated { get; set; }

    [Name("firstadded")]
    public DateTime FirstAdded { get; set; }

    [Name("firstpricedate")]
    public DateTime FirstPriceDate { get; set; }

    [Name("lastpricedate")]
    public DateTime LastPriceDate { get; set; }

    [Name("firstquarter")]
    public DateTime FirstQuarter { get; set; }

    [Name("lastquarter")]
    public DateTime LastQuarter { get; set; }

    [Name("secfilings")]
    public string SECFilings { get; set; }

    public Ticker Ticker { get; set; }
}

public class Price
{
    [Name("priceid")]
    public int PriceID { get; set; }

    [Name("ticker")]
    public string TickerSymbol { get; set; }

    [Name("date")]
    public DateTime Date { get; set; }

    [Name("open")]
    public float Open { get; set; }

    [Name("high")]
    public float High { get; set; }

    [Name("low")]
    public float Low { get; set; }

    [Name("close")]
    public float Close { get; set; }

    [Name("volume")]
    public float Volume { get; set; }

    [Name("lastupdated")]
    public DateTime LastUpdated { get; set; }

    public Ticker Ticker { get; set; }
    public List<Adjustment> Adjustments { get; set; }
}

public class Adjustment
{
    [Name("adjustmentid")]
    public int AdjustmentID { get; set; }

    [Name("priceid")]
    public int PriceID { get; set; }

    [Name("closeadj")]
    public float CloseAdj { get; set; }

    [Name("closeunadj")]
    public float CloseUnadj { get; set; }

    public Price Price { get; set; }
}
