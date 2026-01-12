using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestPlatform.TestHost;

namespace StockQuoteAlert.Tests;

public class ProgramValidationTests
{
    private readonly Mock<ILogger> _loggerMock;

    public ProgramValidationTests()
    {
        _loggerMock = new Mock<ILogger>();
    }

    [Theory]
    [InlineData(new string[] { "PETR4", "30.00", "20.00" }, true)]
    [InlineData(new string[] { "PETR4", "30.00" }, false)] // Faltando argumento
    public void ValidateArgsCount_ShouldIdentifyCorrectParameterAmount(string[] args, bool expected)
    {
        var result = Program.ValidateArgsCount(args, _loggerMock.Object);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void TryParseParameters_ShouldReturnTrue_ForValidNumbers()
    {
        string[] args = { "PETR4", "22.50", "20.00" };

        bool result = Program.TryParseParameters(args, out string ticker, out decimal sell, out decimal buy, _loggerMock.Object);

        Assert.True(result);
        Assert.Equal("PETR4", ticker);
        Assert.Equal(22.50m, sell);
        Assert.Equal(20.00m, buy);
    }

    [Fact]
    public void TryParseParameters_ShouldReturnFalse_ForInvalidNumbers()
    {
        string[] args = { "PETR4", "abc", "20.00" };

        bool result = Program.TryParseParameters(args, out _, out _, out _, _loggerMock.Object);

        Assert.False(result);
    }
}