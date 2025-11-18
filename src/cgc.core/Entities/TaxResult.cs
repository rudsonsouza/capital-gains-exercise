using System.Text.Json.Serialization;

namespace cgc.core.Entities;

/// <summary>
/// Represents the tax result for a transaction.
/// </summary>
public class TaxResult
{
    /// <summary>
    /// The tax amount to be paid for the transaction.
    /// </summary>
    [JsonPropertyName("tax")]
    public decimal Tax { get; set; }
}