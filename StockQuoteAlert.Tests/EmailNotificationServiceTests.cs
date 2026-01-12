using Moq;
using Xunit;
using MimeKit;
using MailKit;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using StockQuoteAlert.Services;

namespace StockQuoteAlert.Tests;

public class EmailNotificationServiceTests
{
    [Fact]
    public async Task SendAlertAsync_ShouldNotThrow_WhenConfigIsComplete()
    {
        var configMock = new Mock<IConfiguration>();
        configMock.Setup(c => c["SmtpSettings:Server"]).Returns("smtp.test.com");
        configMock.Setup(c => c["SmtpSettings:Port"]).Returns("587");
        configMock.Setup(c => c["SmtpSettings:SenderName"]).Returns("System");
        configMock.Setup(c => c["SmtpSettings:SenderEmail"]).Returns("sender@test.com");
        configMock.Setup(c => c["SmtpSettings:TargetEmail"]).Returns("target@test.com");

        var smtpMock = new Mock<ISmtpClient>();
        var service = new EmailNotificationService(configMock.Object, new Mock<ILogger<EmailNotificationService>>().Object, smtpMock.Object);

        await service.SendAlertAsync("PETR4", 35m, "Venda");

        smtpMock.Verify(s => s.ConnectAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<MailKit.Security.SecureSocketOptions>(), It.IsAny<CancellationToken>()), Times.Once);
        smtpMock.Verify(s => s.SendAsync(It.IsAny<MimeMessage>(), It.IsAny<CancellationToken>(), It.IsAny<ITransferProgress>()), Times.Once);
    }
}