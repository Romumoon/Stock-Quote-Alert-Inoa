using System.Net.Http.Json;
using Microsoft.Extensions.Configuration;
using StockQuoteAlert.Models;

namespace StockQuoteAlert.Services;

public class HGStockService : IStockService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private readonly string _baseUrl;

    public HGStockService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _apiKey = configuration["HGSettings:ApiKey"] ?? throw new ArgumentNullException("ApiKey não configurada");
        _baseUrl = configuration["HGSettings:BaseUrl"] ?? throw new ArgumentNullException("BaseUrl não configurada");
    }

    public async Task<decimal?> GetStockPriceAsync(string ticker)
    {
        try
        {
            // Exemplo de URL: https://api.hgbrasil.com/finance/stock_price?key=CHAVE&symbol=PETR4
            var url = $"{_baseUrl}?key={_apiKey}&symbol={ticker}";

            var response = await _httpClient.GetFromJsonAsync<HGResponse>(url);

            if (response?.Results != null && response.Results.TryGetValue(ticker.ToUpper(), out var stockData))
            {
                return stockData.Price;
            }

            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao consultar API: {ex.Message}");
            return null;
        }
    }
}