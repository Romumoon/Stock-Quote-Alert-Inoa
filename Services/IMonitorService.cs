namespace StockQuoteAlert.Services;

public interface IMonitorService
{
    Task ExecuteAsync(string ticker, decimal sellPrice, decimal buyPrice);
}