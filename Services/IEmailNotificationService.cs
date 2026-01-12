namespace StockQuoteAlert.Services;

public interface IEmailNotificationService
{
    Task SendAlertAsync(string ticker, decimal currentPrice, string advice);
}