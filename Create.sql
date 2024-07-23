-- Create the Exchanges Table
CREATE TABLE EXCHANGES (
    ExchangeID INT IDENTITY(1,1) PRIMARY KEY,
    ExchangeName VARCHAR(50)
);

-- Create the Sectors Table
CREATE TABLE SECTORS (
    SIID uniqueidentifier PRIMARY KEY,
    SICCode INT,
    SICSector VARCHAR(100)
);

-- Create the Tickers Table
CREATE TABLE TICKERS 
    PermaTicker INT PRIMARY KEY,
    Ticker VARCHAR(10) NOT NULL,
    Name VARCHAR(255),
    Category VARCHAR(100),
    CUSIPs VARCHAR(50),
    SIID uniqueidentifier,
    Location VARCHAR(255),
    CompanySite VARCHAR(255),
    FOREIGN KEY (SIID) REFERENCES SECTORS(SIID),
    UNIQUE (Ticker)  -- Ensures Ticker is unique and can be referenced by foreign keys
);

-- Create the Ticker Exchanges Junction Table
CREATE TABLE TICKER_EXCHANGES (
    PermaTicker INT,
    ExchangeID INT,
    IsDelisted CHAR(1),
    FOREIGN KEY (PermaTicker) REFERENCES TICKERS(PermaTicker),
    FOREIGN KEY (ExchangeID) REFERENCES EXCHANGES(ExchangeID),
    PRIMARY KEY (PermaTicker, ExchangeID)
);

-- Create the Stock History Table
CREATE TABLE STOCK_HISTORY (
    PermaTicker INT,
    LastUpdated DATE,
    FirstAdded DATE,
    FirstPriceDate DATE,
    LastPriceDate DATE,
    FirstQuarter DATE,
    LastQuarter DATE,
    SECFilings VARCHAR(255),
    FOREIGN KEY (PermaTicker) REFERENCES TICKERS(PermaTicker)
);

-- Create the Prices Table
CREATE TABLE PRICES (
    PriceID INT IDENTITY(1,1) PRIMARY KEY,
    Ticker VARCHAR(10),
    Date DATE,
    Open FLOAT,
    High FLOAT,
    Low FLOAT,
    Close FLOAT,
    Volume FLOAT,
    LastUpdated DATE,
    FOREIGN KEY (Ticker) REFERENCES TICKERS(Ticker)  -- References the unique Ticker in TICKERS table
);

-- Create the Adjustments Table
CREATE TABLE ADJUSTMENTS (
    AdjustmentID INT IDENTITY(1,1) PRIMARY KEY,
    PriceID INT,
    CloseAdj FLOAT,
    CloseUnadj FLOAT,
    FOREIGN KEY (PriceID) REFERENCES PRICES(PriceID)
);

-- Indices for potentially frequent queries
CREATE INDEX idx_ticker ON TICKERS(Ticker);
CREATE INDEX idx_exchange_name ON EXCHANGES(ExchangeName);
CREATE INDEX idx_sector_code ON SECTORS(SICCode);
CREATE INDEX idx_price_date ON PRICES(Date);
