using cgc.core.Entities;
using cgc.core.Factory;

namespace cgc.core.Application;

public class CapitalGainsCalculator
{
    private readonly CalculatorState _state;
    
    public CapitalGainsCalculator()
    {
        _state = new CalculatorState();
    }
    
    public TaxResult ProcessTransaction(Transaction transaction)
    {
        var strategy = TransactionFactory.GetStrategy(transaction.Operation);
        return strategy.Process(transaction, _state);
    }
}