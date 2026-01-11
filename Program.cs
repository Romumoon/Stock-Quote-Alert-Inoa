using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StockQuoteAlert.Services;
using System.Globalization;

if (args.Length < 3)
{
    Console.WriteLine("Erro! Uso correto: stock-quote-alert.exe <ATIVO> <PRECO_VENDA> <PRECO_COMPRA>");
    Console.WriteLine("Exemplo: dotnet run -- PETR4 22.67 22.59");
    return;
}

string ticker = args[0].ToUpper();
if (!decimal.TryParse(args[1], NumberStyles.Any, CultureInfo.InvariantCulture, out decimal sellPrice) ||
    !decimal.TryParse(args[2], NumberStyles.Any, CultureInfo.InvariantCulture, out decimal buyPrice))
{
    Console.WriteLine("Erro: Os preços de venda e compra devem ser números válidos (use ponto para decimais).");
    return;
}

using IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.AddHttpClient<IStockService, BrapiStockService>();
        services.AddTransient<INotificationService, EmailNotificationService>();
    })
    .Build();

var stockService = host.Services.GetRequiredService<IStockService>();
var notificationService = host.Services.GetRequiredService<INotificationService>();

Console.WriteLine("--------------------------------------------------");
Console.WriteLine($"MONITORANDO: {ticker}");
Console.WriteLine($"ALERTA DE VENDA: > R$ {sellPrice:F2}");
Console.WriteLine($"ALERTA DE COMPRA: < R$ {buyPrice:F2}");
Console.WriteLine("Pressione [Ctrl + C] para parar.");
Console.WriteLine("--------------------------------------------------");

using var timer = new PeriodicTimer(TimeSpan.FromMinutes(1));

while (await timer.WaitForNextTickAsync())
{
    Console.Write($"[{DateTime.Now:HH:mm:ss}] Consultando {ticker}... ");

    var currentPrice = await stockService.GetStockPriceAsync(ticker);

    if (currentPrice.HasValue)
    {
        Console.WriteLine($"Preço atual: R$ {currentPrice.Value:F2}");

        if (currentPrice.Value >= sellPrice)
        {
            Console.WriteLine(">> ALVO DE VENDA ATINGIDO!");
            await notificationService.SendAlertAsync(ticker, currentPrice.Value, "Venda");
        }
        else if (currentPrice.Value <= buyPrice)
        {
            Console.WriteLine(">> ALVO DE COMPRA ATINGIDO!");
            await notificationService.SendAlertAsync(ticker, currentPrice.Value, "Compra");
        }
    }
    else
    {
        Console.WriteLine("Falha ao obter cotação. Verifique a conexão/chave.");
    }
}