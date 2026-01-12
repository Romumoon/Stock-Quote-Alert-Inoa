using Moq;
using Xunit;
using Microsoft.Extensions.Logging;
using StockQuoteAlert.Services;

namespace StockQuoteAlert.Tests;

public class StockMonitorTests
{
    private readonly Mock<IStockService> _stockMock;
    private readonly Mock<IEmailNotificationService> _notifyMock;
    private readonly Mock<ILogger<StockMonitorService>> _loggerMock;
    private readonly StockMonitorService _service;

    public StockMonitorTests()
    {
        _stockMock = new Mock<IStockService>();
        _notifyMock = new Mock<IEmailNotificationService>();
        _loggerMock = new Mock<ILogger<StockMonitorService>>();
        _service = new StockMonitorService(_stockMock.Object, _notifyMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldSendSellAlert_WhenPriceIsHigh()
    {
        _stockMock.Setup(s => s.GetStockPriceAsync("PETR4")).ReturnsAsync(35.00m);
        var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(100));

        try { await _service.ExecuteAsync("PETR4", 30.00m, 20.00m, cts.Token); }
        catch (OperationCanceledException) { }

        _notifyMock.Verify(n => n.SendAlertAsync("PETR4", 35.00m, "Venda"), Times.AtLeastOnce());
    }

    [Fact]
    public async Task ExecuteAsync_ShouldSendBuyAlert_WhenPriceIsLow()
    {
        _stockMock.Setup(s => s.GetStockPriceAsync("PETR4")).ReturnsAsync(15.00m);
        var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(100));

        try { await _service.ExecuteAsync("PETR4", 30.00m, 20.00m, cts.Token); }
        catch (OperationCanceledException) { }

        _notifyMock.Verify(n => n.SendAlertAsync("PETR4", 15.00m, "Compra"), Times.AtLeastOnce());
    }

    [Fact]
    public async Task ExecuteAsync_ShouldNotSendAlert_WhenPriceIsNeutral()
    {
        _stockMock.Setup(s => s.GetStockPriceAsync("PETR4")).ReturnsAsync(25.00m);
        var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(100));

        try { await _service.ExecuteAsync("PETR4", 30.00m, 20.00m, cts.Token); }
        catch (OperationCanceledException) { }

        _notifyMock.Verify(n => n.SendAlertAsync(It.IsAny<string>(), It.IsAny<decimal>(), It.IsAny<string>()), Times.Never());
    }

    [Fact]
    public async Task ExecuteAsync_ShouldHandleApiError_AndContinue()
    {
        _stockMock.Setup(s => s.GetStockPriceAsync("PETR4")).ReturnsAsync((decimal?)null);
        var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(100));

        try { await _service.ExecuteAsync("PETR4", 30.00m, 20.00m, cts.Token); }
        catch (OperationCanceledException) { }

        _notifyMock.Verify(n => n.SendAlertAsync(It.IsAny<string>(), It.IsAny<decimal>(), It.IsAny<string>()), Times.Never());
    }
}