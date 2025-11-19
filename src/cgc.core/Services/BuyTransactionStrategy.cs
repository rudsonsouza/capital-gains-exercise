using cgc.core.Entities;
using cgc.core.Interfaces;

namespace cgc.core.Services;

public class BuyTransactionStrategy : ITransactionStrategy
{
    private const int decimalPlaces = 2;

    public TaxResult Process(Transaction transaction, CalculatorState state)
    {
        if (state.CurrentPosition == 0)
        {
            state.AverageCost = Math.Round(transaction.UnitCost, decimalPlaces);
            state.CurrentPosition = transaction.Quantity;
        }
        else
        {
            decimal totalCost = (state.CurrentPosition * state.AverageCost) + 
                                (transaction.Quantity * transaction.UnitCost);
            state.CurrentPosition += transaction.Quantity;
            state.AverageCost = Math.Round(totalCost / state.CurrentPosition, decimalPlaces);
        }
        
        return new TaxResult { Tax = 0m };
    }
}

