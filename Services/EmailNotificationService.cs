using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace StockQuoteAlert.Services;

public class EmailNotificationService : IEmailNotificationService
{
    private readonly IConfiguration _config;
    private readonly ILogger<EmailNotificationService> _logger;
    private readonly ISmtpClient _smtpClient; // Injetado para permitir Mock

    public EmailNotificationService(IConfiguration config, ILogger<EmailNotificationService> logger, ISmtpClient? smtpClient = null)
    {
        _config = config;
        _logger = logger;
        _smtpClient = smtpClient ?? new SmtpClient();
    }

    public async Task SendAlertAsync(string ticker, decimal currentPrice, string advice)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(_config["SmtpSettings:SenderName"], _config["SmtpSettings:SenderEmail"]));
        message.To.Add(new MailboxAddress("Investor", _config["SmtpSettings:TargetEmail"]));
        message.Subject = $"ALERTA B3: {advice} {ticker}";

        message.Body = new TextPart("plain")
        {
            Text = $"Atenção!\n\nO ativo {ticker} atingiu R$ {currentPrice:F2}. Sugerimos realizar a {advice}."
        };

        try
        {
            await _smtpClient.ConnectAsync(_config["SmtpSettings:Server"], int.Parse(_config["SmtpSettings:Port"] ?? "587"), MailKit.Security.SecureSocketOptions.StartTls);
            await _smtpClient.AuthenticateAsync(_config["SmtpSettings:Username"], _config["SmtpSettings:Password"]);
            await _smtpClient.SendAsync(message);
            await _smtpClient.DisconnectAsync(true);
            _logger.LogInformation("Email sent for {Ticker}", ticker);
        }
        catch (Exception ex) { _logger.LogError(ex, "Failed to send email."); }
    }
}