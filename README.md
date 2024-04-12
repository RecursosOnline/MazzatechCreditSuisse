# Resposta ao enunciado
Veja o codigo completo em : [Mazztech.CreditSuisse](Mazztech.CreditSuisse)

## Question 1
```C#
public class TradeCategorizationServiceUnitTest
{
    [Fact]
    public void Deve_CriarServico_ComSucesso()
    {
        //Arrange
        var lowRisk = new LowRiskRule();
        var mediumRisk = new MediumRiskRule();
        var highRisk = new HighRiskRule();
        ITradeCategorizationService tradeCategorizationService =
            new TradeCategorizationService(new List<ITradeCategoryRule>() { lowRisk, mediumRisk, highRisk });
        
        var portfolio = new List<ITrade>()
        {
            new Trade(2000000,  ClientSector.Private),
            new Trade(400000, ClientSector.Public),
            new Trade(500000,  ClientSector.Public),
            new Trade(3000000, ClientSector.Public)
        };
        //Act
        var tradeCategories = tradeCategorizationService.Categorize(portfolio);
        //Assert
        Assert.Collection(tradeCategories,
            e =>
            {
                Assert.Equal("HIGHRISK", e);
            },
            e =>
            {
                Assert.Equal("LOWRISK", e);
            },
            e =>
            {
                Assert.Equal("LOWRISK", e);
            },
            e =>
            {
                Assert.Equal("MEDIUMRISK", e);
            }
            );
    }
}
```

<img width="981" alt="image" src="https://github.com/RecursosOnline/MazzatechCreditSuisse/assets/5785231/9cb9459f-9743-4e60-a39f-98090e4c7004">

## Question #2
```SQL
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
```
```SQL
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
```
<img width="738" alt="image" src="https://github.com/RecursosOnline/MazzatechCreditSuisse/assets/5785231/046ce71e-2570-4506-af1e-c862a9b4a77d">
