using System.Net.Http.Json;
using Microsoft.Extensions.Configuration;
using StockQuoteAlert.Models;

namespace StockQuoteAlert.Services;

public class BrapiStockService : IStockService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private readonly string _baseUrl;

    public BrapiStockService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _apiKey = configuration["BrapiSettings:ApiKey"] ?? "";
        _baseUrl = configuration["BrapiSettings:BaseUrl"] ?? "https://brapi.dev/api/quote/";
    }

    public async Task<decimal?> GetStockPriceAsync(string ticker)
    {
        try
        {
            // Brapi usa o ticker na URL: https://brapi.dev/api/quote/PETR4?token=SUA_KEY
            var url = $"{_baseUrl}{ticker}?token={_apiKey}";
            var response = await _httpClient.GetFromJsonAsync<BrapiResponse>(url);

            return response?.Results.FirstOrDefault()?.RegularMarketPrice;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[API Brapi] Erro: {ex.Message}");
            return null;
        }
    }
}