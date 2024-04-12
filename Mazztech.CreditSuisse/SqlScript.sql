
CREATE DATABASE MazzatechCreditSuisse
GO
USE MazzatechCreditSuisse
GO
DROP TABLE Trade
    GO
CREATE TABLE Trade (
                       TradeId INT IDENTITY(1,1) PRIMARY KEY,
                       Value DECIMAL(18,2) NOT NULL,
                       ClientSector VARCHAR(10) NOT NULL,
                       Category VARCHAR(50)
);
GO
DROP PROCEDURE InsertTrade
    GO
DROP PROCEDURE InsertCollectionTrades
    GO
DROP TYPE TradeInfo
    GO
CREATE TYPE TradeInfo as TABLE
    (
    Value decimal(18,2),
    ClientSector varchar(10)
    )
    GO

CREATE PROCEDURE InsertCollectionTrades
    @tradeList TradeInfo READONLY
AS
BEGIN
INSERT INTO Trade
SELECT
    Value = t.Value,
    ClientSector = t.ClientSector,
    Category = CASE
                   WHEN T.Value < 1000000 AND T.ClientSector = 'Public' THEN 'LOWRISK'
                   WHEN T.Value >= 1000000 AND T.ClientSector = 'Public' THEN 'MEDIUMRISK'
                   WHEN T.Value >= 1000000 AND T.ClientSector = 'Private' THEN 'HIGHRISK'
        END
FROM @tradeList T
END;
GO

CREATE PROCEDURE InsertTrade
(
    @Value decimal(18,2),
    @ClientSector varchar(10)
)
    AS
BEGIN
	DECLARE @Category varchar(50)
	SET @Category = CASE 
							WHEN @Value < 1000000 AND @ClientSector = 'Public' THEN 'LOWRISK'
							WHEN @Value >= 1000000 AND @ClientSector = 'Public' THEN 'MEDIUMRISK'
							WHEN @Value >= 1000000 AND @ClientSector = 'Private' THEN 'HIGHRISK'
END
INSERT INTO Trade(Value, ClientSector, Category)
VALUES(@Value, @ClientSector, @Category)


END
GO

-- Inserindo varios trades
DECLARE @trades TradeInfo

INSERT INTO @trades (Value, ClientSector)
VALUES (2000000, 'Private'),
       (400000, 'Public'),
       (500000, 'Public'),
       (3000000, 'Public');


EXEC InsertCollectionTrades @trades;

-- Inserção individual

EXEC InsertTrade 1990000, 'Public';

