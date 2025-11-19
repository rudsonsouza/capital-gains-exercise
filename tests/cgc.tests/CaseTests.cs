using cgc.core.Application;
using cgc.core.Entities;

namespace cgc.tests;

public class CaseTests
{
    private List<TaxResult> ProcessTransactions(List<Transaction> transactions)
    {
        var calculator = new CapitalGainsCalculator();
        var results = new List<TaxResult>();

        foreach (var transaction in transactions)
        {
            var result = calculator.ProcessTransaction(transaction);
            results.Add(result);
        }

        return results;
    }
    
    [Fact]
    public void Case1_AllSales_NoTax()
    {
        var transactions = new List<Transaction>
        {
            new Transaction { Operation = "buy", UnitCost = 10.00m, Quantity = 100 },
            new Transaction { Operation = "sell", UnitCost = 15.00m, Quantity = 50 },
            new Transaction { Operation = "sell", UnitCost = 15.00m, Quantity = 50 }
        };

        var results = ProcessTransactions(transactions);

        Assert.Equal(3, results.Count);
        Assert.Equal(0.0m, results[0].Tax);
        Assert.Equal(0.0m, results[1].Tax);
        Assert.Equal(0.0m, results[2].Tax);
    }

    [Fact]
    public void Case2_ProfitAndLoss_TaxCalculation()
    {
        var transactions = new List<Transaction>
        {
            new Transaction { Operation = "buy", UnitCost = 10.00m, Quantity = 10000 },
            new Transaction { Operation = "sell", UnitCost = 20.00m, Quantity = 5000 },
            new Transaction { Operation = "sell", UnitCost = 5.00m, Quantity = 5000 }
        };

        var results = ProcessTransactions(transactions);

        Assert.Equal(3, results.Count);
        Assert.Equal(0.0m, results[0].Tax);
        Assert.Equal(10000.0m, results[1].Tax);
        Assert.Equal(0.0m, results[2].Tax);
    }

    [Fact]
    public void Case3_Loss_OffsetsProfit()
    {
        var transactions = new List<Transaction>
        {
            new Transaction { Operation = "buy", UnitCost = 10.00m, Quantity = 10000 },
            new Transaction { Operation = "sell", UnitCost = 5.00m, Quantity = 5000 },
            new Transaction { Operation = "sell", UnitCost = 20.00m, Quantity = 3000 }
        };

        var results = ProcessTransactions(transactions);

        Assert.Equal(3, results.Count);
        Assert.Equal(0.0m, results[0].Tax);
        Assert.Equal(0.0m, results[1].Tax);
        Assert.Equal(1000.0m, results[2].Tax);
    }

    [Fact]
    public void Case4_MultipleBuys_AverageCalculation()
    {
        var transactions = new List<Transaction>
        {
            new Transaction { Operation = "buy", UnitCost = 10.00m, Quantity = 10000 },
            new Transaction { Operation = "buy", UnitCost = 25.00m, Quantity = 5000 },
            new Transaction { Operation = "sell", UnitCost = 15.00m, Quantity = 10000 }
        };

        var results = ProcessTransactions(transactions);

        Assert.Equal(3, results.Count);
        Assert.Equal(0.0m, results[0].Tax);
        Assert.Equal(0.0m, results[1].Tax);
        Assert.Equal(0.0m, results[2].Tax);
    }

    [Fact]
    public void Case5_MaintainsdAverage()
    {
        var transactions = new List<Transaction>
        {
            new Transaction { Operation = "buy", UnitCost = 10.00m, Quantity = 10000 },
            new Transaction { Operation = "buy", UnitCost = 25.00m, Quantity = 5000 },
            new Transaction { Operation = "sell", UnitCost = 15.00m, Quantity = 10000 },
            new Transaction { Operation = "sell", UnitCost = 25.00m, Quantity = 5000 }
        };

        var results = ProcessTransactions(transactions);

        Assert.Equal(4, results.Count);
        Assert.Equal(0.0m, results[0].Tax);
        Assert.Equal(0.0m, results[1].Tax);
        Assert.Equal(0.0m, results[2].Tax);
        Assert.Equal(10000.0m, results[3].Tax);
    }

    [Fact]
    public void Case6_LossCarry_MultipleOperations()
    {
        var transactions = new List<Transaction>
        {
            new Transaction { Operation = "buy", UnitCost = 10.00m, Quantity = 10000 },
            new Transaction { Operation = "sell", UnitCost = 2.00m, Quantity = 5000 },
            new Transaction { Operation = "sell", UnitCost = 20.00m, Quantity = 2000 },
            new Transaction { Operation = "sell", UnitCost = 20.00m, Quantity = 2000 },
            new Transaction { Operation = "sell", UnitCost = 25.00m, Quantity = 1000 }
        };

        var results = ProcessTransactions(transactions);

        Assert.Equal(5, results.Count);
        Assert.Equal(0.0m, results[0].Tax);
        Assert.Equal(0.0m, results[1].Tax);
        Assert.Equal(0.0m, results[2].Tax);
        Assert.Equal(0.0m, results[3].Tax);
        Assert.Equal(3000.0m, results[4].Tax);
    }

    [Fact]
    public void Case7_NewBuy_ResetsdAverage()
    {
        var transactions = new List<Transaction>
        {
            new Transaction { Operation = "buy", UnitCost = 10.00m, Quantity = 10000 },
            new Transaction { Operation = "sell", UnitCost = 2.00m, Quantity = 5000 },
            new Transaction { Operation = "sell", UnitCost = 20.00m, Quantity = 2000 },
            new Transaction { Operation = "sell", UnitCost = 20.00m, Quantity = 2000 },
            new Transaction { Operation = "sell", UnitCost = 25.00m, Quantity = 1000 },
            new Transaction { Operation = "buy", UnitCost = 20.00m, Quantity = 10000 },
            new Transaction { Operation = "sell", UnitCost = 15.00m, Quantity = 5000 },
            new Transaction { Operation = "sell", UnitCost = 30.00m, Quantity = 4350 },
            new Transaction { Operation = "sell", UnitCost = 30.00m, Quantity = 650 }
        };

        var results = ProcessTransactions(transactions);

        Assert.Equal(9, results.Count);
        Assert.Equal(0.0m, results[0].Tax);
        Assert.Equal(0.0m, results[1].Tax);
        Assert.Equal(0.0m, results[2].Tax);
        Assert.Equal(0.0m, results[3].Tax);
        Assert.Equal(3000.0m, results[4].Tax);
        Assert.Equal(0.0m, results[5].Tax);
        Assert.Equal(0.0m, results[6].Tax);
        Assert.Equal(3700.0m, results[7].Tax);
        Assert.Equal(0.0m, results[8].Tax);
    }

    [Fact]
    public void Case8_SellAndBuy_Calculations()
    {
        var transactions = new List<Transaction>
        {
            new Transaction { Operation = "buy", UnitCost = 10.00m, Quantity = 10000 },
            new Transaction { Operation = "sell", UnitCost = 50.00m, Quantity = 10000 },
            new Transaction { Operation = "buy", UnitCost = 20.00m, Quantity = 10000 },
            new Transaction { Operation = "sell", UnitCost = 50.00m, Quantity = 10000 }
        };

        var results = ProcessTransactions(transactions);

        Assert.Equal(4, results.Count);
        Assert.Equal(0.0m, results[0].Tax);
        Assert.Equal(80000.0m, results[1].Tax);
        Assert.Equal(0.0m, results[2].Tax);
        Assert.Equal(60000.0m, results[3].Tax);
    }

    [Fact]
    public void Case9_Sale_DoesNotLoss()
    {
        var transactions = new List<Transaction>
        {
            new Transaction { Operation = "buy", UnitCost = 5000.00m, Quantity = 10 },
            new Transaction { Operation = "sell", UnitCost = 4000.00m, Quantity = 5 },
            new Transaction { Operation = "buy", UnitCost = 15000.00m, Quantity = 5 },
            new Transaction { Operation = "buy", UnitCost = 4000.00m, Quantity = 2 },
            new Transaction { Operation = "buy", UnitCost = 23000.00m, Quantity = 2 },
            new Transaction { Operation = "sell", UnitCost = 20000.00m, Quantity = 1 },
            new Transaction { Operation = "sell", UnitCost = 12000.00m, Quantity = 10 },
            new Transaction { Operation = "sell", UnitCost = 15000.00m, Quantity = 3 }
        };

        var results = ProcessTransactions(transactions);

        Assert.Equal(8, results.Count);
        Assert.Equal(0.0m, results[0].Tax);
        Assert.Equal(0.0m, results[1].Tax);
        Assert.Equal(0.0m, results[2].Tax);
        Assert.Equal(0.0m, results[3].Tax);
        Assert.Equal(0.0m, results[4].Tax);
        Assert.Equal(0.0m, results[5].Tax);
        Assert.Equal(1000.0m, results[6].Tax);
        Assert.Equal(2400.0m, results[7].Tax);
    }

}