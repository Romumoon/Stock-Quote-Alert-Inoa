namespace StockQuoteAlert.Services;

public interface IStockMonitorService
{
    Task ExecuteAsync(string ticker, decimal sellPrice, decimal buyPrice, CancellationToken ct);
}