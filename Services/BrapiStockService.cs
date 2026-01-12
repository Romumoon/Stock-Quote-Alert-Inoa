using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using StockQuoteAlert.Models;
using StockQuoteAlert.Services;
using System.Net.Http.Json;

public class BrapiStockService : IStockService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<BrapiStockService> _logger;
    private readonly string _apiKey;
    private readonly string _baseUrl;

    public BrapiStockService(HttpClient httpClient, IConfiguration configuration, ILogger<BrapiStockService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        _apiKey = configuration["BrapiSettings:ApiKey"] ?? "";
        _baseUrl = configuration["BrapiSettings:BaseUrl"] ?? "https://brapi.dev/api/quote/";
    }

    public async Task<decimal?> GetStockPriceAsync(string ticker)
    {
        try
        {
            var url = $"{_baseUrl}{ticker}?token={_apiKey}";
            var response = await _httpClient.GetFromJsonAsync<BrapiResponse>(url);

            var price = response?.Results.FirstOrDefault()?.RegularMarketPrice;

            if (price == null)
                _logger.LogWarning("Ativo {Ticker} não encontrado na Brapi.", ticker);

            return price;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao consultar API Brapi para o ativo {Ticker}", ticker);
            return null;
        }
    }
}