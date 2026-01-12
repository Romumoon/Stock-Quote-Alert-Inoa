using Microsoft.Extensions.Logging;

namespace StockQuoteAlert.Services;

public class StockMonitorService : IMonitorService
{
    private readonly IStockService _stockService;
    private readonly INotificationService _notificationService;
    private readonly ILogger<StockMonitorService> _logger;

    public StockMonitorService(
        IStockService stockService,
        INotificationService notificationService,
        ILogger<StockMonitorService> logger)
    {
        _stockService = stockService;
        _notificationService = notificationService;
        _logger = logger;
    }

    public async Task ExecuteAsync(string ticker, decimal sellPrice, decimal buyPrice)
    {
        using var timer = new PeriodicTimer(TimeSpan.FromMinutes(1));

        while (await timer.WaitForNextTickAsync())
        {
            try
            {
                var price = await _stockService.GetStockPriceAsync(ticker);
                if (!price.HasValue) continue;

                _logger.LogInformation("{Ticker} atual: R$ {Price:F2}", ticker, price.Value);

                if (price.Value >= sellPrice)
                    await _notificationService.SendAlertAsync(ticker, price.Value, "Venda");
                else if (price.Value <= buyPrice)
                    await _notificationService.SendAlertAsync(ticker, price.Value, "Compra");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Falha no ciclo de monitoramento.");
            }
        }
    }
}