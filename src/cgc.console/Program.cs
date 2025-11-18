using System.Text.Json;
using System.Text.Json.Serialization;
using cgc.core.Application;
using cgc.core.Entities;

var jsonOptions = new JsonSerializerOptions
{
    WriteIndented = false,
    DefaultIgnoreCondition = JsonIgnoreCondition.Never
};

string? line;
while ((line = Console.ReadLine()) != null)
{
    if (string.IsNullOrWhiteSpace(line))
    {
        break;
    }

    var transactions = JsonSerializer.Deserialize<List<Transaction>>(line);
    if (transactions == null || transactions.Count == 0)
    {
        continue;
    }

    var calculator = new CapitalGainsCalculator();
    var results = transactions.Select(calculator.ProcessTransaction).ToList();

    var output = JsonSerializer.Serialize(results, jsonOptions);
    Console.WriteLine(output);
}