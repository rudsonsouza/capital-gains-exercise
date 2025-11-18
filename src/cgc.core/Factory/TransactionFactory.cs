using cgc.core.Entities;
using cgc.core.Interfaces;
using cgc.core.Services;

namespace cgc.core.Factory;

public static class TransactionFactory
{
    private static readonly Dictionary<string, ITransactionStrategy> Strategies = new()
    {
        { "buy", new BuyTransactionStrategy() },
        { "sell", new SellTransactionStrategy() }
    };

    public static ITransactionStrategy GetStrategy(string operation)
    {
        return Strategies.GetValueOrDefault(operation.ToLowerInvariant(), new NullTransactionStrategy());
    }
}

internal class NullTransactionStrategy : ITransactionStrategy
{
    public TaxResult Process(Transaction transaction, CalculatorState state)
    {
        return new TaxResult { Tax = 0m };
    }
}

