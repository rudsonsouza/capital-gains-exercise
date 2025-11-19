using cgc.core.Entities;
using cgc.core.Interfaces;

namespace cgc.core.Services;

public class SellTransactionStrategy : ITransactionStrategy
{
    private const decimal taxRate = 0.20m;
    private const decimal taxValue = 20000m;
    private const int decimalPlaces = 2;

    public TaxResult Process(Transaction transaction, CalculatorState state)
    {
        decimal saleValue = transaction.UnitCost * transaction.Quantity;
        decimal profit = (transaction.UnitCost - state.AverageCost) * transaction.Quantity;
        
        state.CurrentPosition -= transaction.Quantity;
        
        if (state.CurrentPosition == 0)
        {
            state.AverageCost = 0m;
        }
        
        if (saleValue <= taxValue)
        {
            if (profit < 0)
            {
                state.AccountLoss += Math.Abs(profit);
            }

            return new TaxResult { Tax = 0m };
        }
        
        if (profit < 0)
        {
            state.AccountLoss += Math.Abs(profit);
            return new TaxResult { Tax = 0m };
        }
        
        decimal taxableProfit = profit;
        if (state.AccountLoss > 0)
        {
            if (state.AccountLoss >= taxableProfit)
            {
                state.AccountLoss -= taxableProfit;
                taxableProfit = 0m;
            }
            else
            {
                taxableProfit -= state.AccountLoss;
                state.AccountLoss = 0m;
            }
        }
        
        decimal tax = Math.Round(taxableProfit * taxRate, decimalPlaces);
        return new TaxResult { Tax = tax };
    }
}

