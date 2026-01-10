using System.Text.Json.Serialization;

namespace StockQuoteAlert.Models;

public class BrapiResponse
{
    [JsonPropertyName("results")]
    public List<BrapiStockData> Results { get; set; } = [];
}

public class BrapiStockData
{
    [JsonPropertyName("symbol")]
    public string Symbol { get; set; } = string.Empty;

    [JsonPropertyName("regularMarketPrice")]
    public decimal RegularMarketPrice { get; set; }
}