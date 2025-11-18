using System.Text.Json;
using System.Text.Json.Serialization;
using cgc.core.Application;
using cgc.core.Entities;

var jsonOptions = new JsonSerializerOptions
{
    WriteIndented = false,
    DefaultIgnoreCondition = JsonIgnoreCondition.Never
};

if (args.Length > 0)
{
    foreach (var argument in args)
    {
        ProcessPayload(argument);
    }
}

void ProcessPayload(string payload)
{
    if (string.IsNullOrWhiteSpace(payload))
    {
        return;
    }

    try
    {
        var transactions = JsonSerializer.Deserialize<List<Transaction>>(payload);
        if (transactions == null || transactions.Count == 0)
        {
            return;
        }

        var calculator = new CapitalGainsCalculator();
        var results = transactions.Select(calculator.ProcessTransaction).ToList();

        var output = JsonSerializer.Serialize(results, jsonOptions);
        Console.WriteLine(output);
    }
    catch (JsonException ex)
    {
        Console.Error.WriteLine($"Erro ao processar JSON: {ex.Message}");
        Console.Error.WriteLine($"Payload recebido: {payload}");
    }
}