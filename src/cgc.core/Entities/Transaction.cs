using System.Text.Json.Serialization;

namespace cgc.core.Entities;

/// <summary>
/// Represents a stock market transaction (buy or sell).
/// </summary>
public class Transaction
{
    [JsonPropertyName("operation")]
    public string Operation { get; set; } = string.Empty;
    
    [JsonPropertyName("unit-cost")]
    public decimal UnitCost { get; set; }
    
    [JsonPropertyName("quantity")]
    public int Quantity { get; set; }
}