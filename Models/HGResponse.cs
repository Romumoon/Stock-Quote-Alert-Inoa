using System.Text.Json.Serialization;

namespace StockQuoteAlert.Models;

public class HGResponse
{
    [JsonPropertyName("results")]
    public Dictionary<string, HGStockData> Results { get; set; } = [];
}

public class HGStockData
{
    [JsonPropertyName("symbol")]
    public string Symbol { get; set; } = string.Empty;

    [JsonPropertyName("price")]
    public decimal Price { get; set; }
}