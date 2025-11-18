using cgc.core.Entities;

namespace cgc.core.Interfaces;

/// <summary>
/// Implements the Strategy Pattern to encapsulate transaction processing algorithms.
/// </summary>
public interface ITransactionStrategy
{
    TaxResult Process(Transaction transaction, CalculatorState state);
}

