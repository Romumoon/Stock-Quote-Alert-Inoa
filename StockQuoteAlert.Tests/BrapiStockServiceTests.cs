using Moq;
using Xunit;
using Moq.Protected;
using System.Net;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;
using StockQuoteAlert.Services;
using StockQuoteAlert.Models;

namespace StockQuoteAlert.Tests;

public class BrapiStockServiceTests
{
    private readonly Mock<IConfiguration> _configMock;
    private readonly Mock<ILogger<BrapiStockService>> _loggerMock;

    public BrapiStockServiceTests()
    {
        _configMock = new Mock<IConfiguration>();
        _loggerMock = new Mock<ILogger<BrapiStockService>>();

        _configMock.Setup(c => c["BrapiSettings:ApiKey"]).Returns("test_token");
        _configMock.Setup(c => c["BrapiSettings:BaseUrl"]).Returns("https://brapi.dev/api/quote/");
    }

    [Fact]
    public async Task GetStockPriceAsync_ShouldReturnPrice_WhenApiReturnsSuccess()
    {
        // Arrange
        var mockResponse = new BrapiResponse
        {
            Results = new List<BrapiStockData> { new BrapiStockData { RegularMarketPrice = 25.50m } }
        };

        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = JsonContent.Create(mockResponse)
            });

        var httpClient = new HttpClient(handlerMock.Object);
        var service = new BrapiStockService(httpClient, _configMock.Object, _loggerMock.Object);

        var result = await service.GetStockPriceAsync("PETR4");

        Assert.Equal(25.50m, result);
    }

    [Fact]
    public async Task GetStockPriceAsync_ShouldReturnNull_WhenApiReturnsNotFound()
    {
        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage { StatusCode = HttpStatusCode.NotFound });

        var httpClient = new HttpClient(handlerMock.Object);
        var service = new BrapiStockService(httpClient, _configMock.Object, _loggerMock.Object);

        var result = await service.GetStockPriceAsync("INVALID3");

        Assert.Null(result);
    }
}