CREATE PROCEDURE GetTickerStats
    @Ticker VARCHAR(10)
AS
BEGIN
    SET NOCOUNT ON;

    -- Variables to hold results
    DECLARE @MovingAveragePrice FLOAT;
    DECLARE @HighPrice FLOAT;
    DECLARE @LowPrice FLOAT;

    -- Calculate the 52-day moving average price
    SELECT @MovingAveragePrice = AVG(Close)
    FROM (
        SELECT TOP 52 Close
        FROM PRICES
        WHERE Ticker = @Ticker
        AND Date >= DATEADD(DAY, -364, GETDATE())  -- Ensure only the last 364 days are considered
        ORDER BY Date DESC
    ) AS Last52Days;

    -- Calculate the 52-week high price
    SELECT @HighPrice = MAX(Close)
    FROM PRICES
    WHERE Ticker = @Ticker
    AND Date >= DATEADD(WEEK, -52, GETDATE());

    -- Calculate the 52-week low price
    SELECT @LowPrice = MIN(Close)
    FROM PRICES
    WHERE Ticker = @Ticker
    AND Date >= DATEADD(WEEK, -52, GETDATE());

    -- Return results
    SELECT 
        @MovingAveragePrice AS MovingAveragePrice,
        @HighPrice AS HighPrice,
        @LowPrice AS LowPrice;
END
GO
