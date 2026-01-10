using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StockQuoteAlert.Services;

if (args.Length == 0)
{
    Console.WriteLine("Erro: Você precisa passar o nome de um ativo. Exemplo: PETR4");
    return;
}

string ticker = args[0].ToUpper();

using IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.AddHttpClient<IStockService, BrapiStockService>();
    })
    .Build();

var stockService = host.Services.GetRequiredService<IStockService>();

Console.WriteLine($"--- Iniciando Teste de Conexão com HG Brasil ---");
Console.WriteLine($"Consultando ativo: {ticker}...");

var price = await stockService.GetStockPriceAsync(ticker);

if (price.HasValue)
{
    Console.WriteLine("=====================================");
    Console.WriteLine($"SUCESSO!");
    Console.WriteLine($"Ativo: {ticker}");
    Console.WriteLine($"Preço Atual: R$ {price.Value:F2}");
    Console.WriteLine("=====================================");
}
else
{
    Console.WriteLine("=====================================");
    Console.WriteLine("FALHA na consulta.");
    Console.WriteLine("=====================================");
}