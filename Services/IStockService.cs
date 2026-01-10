namespace StockQuoteAlert.Services;

public interface IStockService
{
    Task<decimal?> GetStockPriceAsync(string ticker);
}