using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using StockQuoteAlert.Services;
using System.Globalization;

using IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) => {
        services.AddHttpClient<IStockService, BrapiStockService>();
        services.AddTransient<IEmailNotificationService, EmailNotificationService>();
        services.AddTransient<IStockMonitorService, StockMonitorService>();
    })
    .ConfigureLogging(logging => {
        logging.ClearProviders();
        logging.AddConsole();
    })
    .Build();

var logger = host.Services.GetRequiredService<ILogger<Program>>();
var config = host.Services.GetRequiredService<IConfiguration>();
var monitorService = host.Services.GetRequiredService<IStockMonitorService>();

if (!ValidateArgsCount(args, logger)) return;
if (!TryParseParameters(args, out string ticker, out decimal sellPrice, out decimal buyPrice, logger)) return;
if (!ValidateConfiguration(config, logger)) return;

var lifetime = host.Services.GetRequiredService<IHostApplicationLifetime>();

logger.LogInformation("================================================");
logger.LogInformation("Iniciando Monitoramento: {Ticker}", ticker);
logger.LogInformation("Alvo Venda: R$ {Sell:F2} | Alvo Compra: R$ {Buy:F2}", sellPrice, buyPrice);
logger.LogInformation("================================================");

try
{
    await monitorService.ExecuteAsync(ticker, sellPrice, buyPrice, lifetime.ApplicationStopping);
}
catch (OperationCanceledException)
{
    logger.LogInformation("Monitoramento parou com sucesso.");
}

public partial class Program
{
    public static bool ValidateArgsCount(string[] args, ILogger logger)
    {
        if (args.Length >= 3) return true;
        logger.LogError("Parâmetros insuficientes. Uso: stock-quote-alert <ATIVO> <PRECO_VENDA> <PRECO_COMPRA>");
        return false;
    }

    public static bool TryParseParameters(string[] args, out string ticker, out decimal sell, out decimal buy, ILogger logger)
    {
        ticker = args[0].ToUpper();
        bool sellOk = decimal.TryParse(args[1], NumberStyles.Any, CultureInfo.InvariantCulture, out sell);
        bool buyOk = decimal.TryParse(args[2], NumberStyles.Any, CultureInfo.InvariantCulture, out buy);

        if (sellOk && buyOk) return true;

        logger.LogError("Os preços informados devem ser numéricos (ex: 22.50).");
        return false;
    }

    public static bool ValidateConfiguration(IConfiguration config, ILogger logger)
    {
        var targetEmail = config["SmtpSettings:TargetEmail"];
        var apiKey = config["BrapiSettings:ApiKey"];

        if (string.IsNullOrWhiteSpace(targetEmail) || !targetEmail.Contains("@"))
        {
        logger.LogError("Configuração: 'TargetEmail' não encontrado ou inválido no appsettings.json.");
            return false;
        }

        if (string.IsNullOrWhiteSpace(apiKey) || apiKey == "PLACEHOLDER")
        {
        logger.LogError("Configuração: 'ApiKey' da Brapi não encontrada nos User Secrets.");
            return false;
        }

        return true;
    }
}