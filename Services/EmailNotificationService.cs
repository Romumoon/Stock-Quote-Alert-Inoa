using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.Extensions.Configuration;

namespace StockQuoteAlert.Services;

public class EmailNotificationService : INotificationService
{
    private readonly IConfiguration _config;

    public EmailNotificationService(IConfiguration config)
    {
        _config = config;
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
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERRO E-MAIL] {ex.Message}");
        }
    }
}