using Microsoft.Extensions.Logging;

namespace StockQuoteAlert.Services;

public class StockMonitorService : IStockMonitorService
{
    private readonly IStockService _stockService;
    private readonly IEmailNotificationService _notificationService;
    private readonly ILogger<StockMonitorService> _logger;

    public StockMonitorService(
        IStockService stockService,
        IEmailNotificationService notificationService,
        ILogger<StockMonitorService> logger)
    {
        _stockService = stockService;
        _notificationService = notificationService;
        _logger = logger;
    }

    public async Task ExecuteAsync(string ticker, decimal sellPrice, decimal buyPrice, CancellationToken ct = default)
    {
        using var timer = new PeriodicTimer(TimeSpan.FromMinutes(1));

        try
        {
            do
            {
                var price = await _stockService.GetStockPriceAsync(ticker);
                if (!price.HasValue) continue;

                _logger.LogInformation("{Ticker} atual: R$ {Price:F2}", ticker, price.Value);

                if (price.Value >= sellPrice)
                    await _notificationService.SendAlertAsync(ticker, price.Value, "Venda");
                else if (price.Value <= buyPrice)
                    await _notificationService.SendAlertAsync(ticker, price.Value, "Compra");

            } while (await timer.WaitForNextTickAsync(ct)); // Espera aqui para a próxima volta
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Monitoramento encerrado pelo usuário.");
        }
    }
}