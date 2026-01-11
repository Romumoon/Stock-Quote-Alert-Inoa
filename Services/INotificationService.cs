namespace StockQuoteAlert.Services;

public interface INotificationService
{
    Task SendAlertAsync(string ticker, decimal currentPrice, string advice);
}