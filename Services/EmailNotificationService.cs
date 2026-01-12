using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace StockQuoteAlert.Services;

public class EmailNotificationService : INotificationService
{
    private readonly IConfiguration _config;
    private readonly ILogger<EmailNotificationService> _logger;

    public EmailNotificationService(IConfiguration config, ILogger<EmailNotificationService> logger)
    {
        _config = config;
        _logger = logger;
    }

    public async Task SendAlertAsync(string ticker, decimal currentPrice, string advice)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(_config["SmtpSettings:SenderName"], _config["SmtpSettings:SenderEmail"]));
        message.To.Add(new MailboxAddress("Investidor", _config["SmtpSettings:TargetEmail"]));

        message.Subject = $"ALERTA B3: {advice} {ticker}";
        message.Body = new TextPart("plain")
        {
            Text = $"Atenção!\n\nO ativo {ticker} atingiu a cotação de R$ {currentPrice:F2}.\n" +
                   $"Sugerimos realizar a {advice} conforme configurado."
        };

        using var client = new SmtpClient();
        try
        {
            await client.ConnectAsync(_config["SmtpSettings:Server"], int.Parse(_config["SmtpSettings:Port"] ?? "2525"), MailKit.Security.SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(_config["SmtpSettings:Username"], _config["SmtpSettings:Password"]);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);

            _logger.LogInformation("E-mail de {Advice} enviado com sucesso para {Target} (Ticker: {Ticker})", advice, _config["SmtpSettings:TargetEmail"], ticker);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Falha ao enviar e-mail de alerta para o ativo {Ticker}", ticker);
        }
    }
}