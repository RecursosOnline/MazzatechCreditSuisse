namespace Mazzatech.CreditSuisse;

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

public interface ITrade
{
    double Value { get; }
    ClientSector ClientSector { get; }
}

public interface ITradeCategorizationService
{
    List<string> Categorize(List<ITrade> portfolio);
}

public interface ITradeCategoryRule
{
    bool IsInCategory(ITrade trade);
    string CategoryName { get; }
}

[Flags]
public enum ClientSector
{
    Public,
    Private
}

public record Trade(double Value, ClientSector ClientSector) : ITrade;

public record Trade2(double Value, ClientSector ClientSector) : ITrade
{
    public double Value { get; private set; }
    public ClientSector ClientSector { get; private set; }
}

public class LowRiskRule : ITradeCategoryRule
{
    const double ValueToCompare = 1_000_000;
    public string CategoryName => "LOWRISK";

    public bool IsInCategory(ITrade trade)
    {
        return trade.Value < ValueToCompare && trade.ClientSector == ClientSector.Public;
    }
}

public class MediumRiskRule : ITradeCategoryRule
{
    const double ValueToCompare = 1_000_000;
    public string CategoryName => "MEDIUMRISK";

    public bool IsInCategory(ITrade trade)
    {
        return trade.Value > ValueToCompare && trade.ClientSector == ClientSector.Public;
    }
}

public class HighRiskRule : ITradeCategoryRule
{
    const double ValueToCompare = 1_000_000;
    public string CategoryName => "HIGHRISK";

    public bool IsInCategory(ITrade trade)
    {
        return trade.Value > ValueToCompare && trade.ClientSector == ClientSector.Private;
    }
}

public class TradeCategory
{
    public string CategoryName { get; private set; }
    public ITradeCategoryRule Rule { get; private set; }

    public TradeCategory(string categoryName, ITradeCategoryRule rule)
    {
        CategoryName = categoryName;
        Rule = rule;
    }

    public bool IsInCategory(ITrade trade) => Rule.IsInCategory(trade);
}

public class TradeCategorizationService : ITradeCategorizationService
{
    private readonly List<TradeCategory> _categories;

    public TradeCategorizationService(IEnumerable<ITradeCategoryRule> rules)
    {
        _categories = rules.Select(rule => new TradeCategory(rule.CategoryName, rule))
            .ToList();
    }

    public List<string> Categorize(List<ITrade> portfolio) =>
        portfolio.Select(trade => _categories.First(c => c.IsInCategory(trade)).CategoryName)
            .ToList();
}