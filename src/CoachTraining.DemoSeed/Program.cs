using CoachTraining.App.Services;
using CoachTraining.DemoSeed;
using CoachTraining.DemoSeed.Reports;
using CoachTraining.Infra;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var options = DemoSeedOptions.Parse(args);

if (options.HelpRequested)
{
    Console.WriteLine("Usage: dotnet run --project src/CoachTraining.DemoSeed -- --profile demo-v1 --reset-demo");
    Console.WriteLine("Flags: --profile <name> | --reset-demo | --reset-all | --help");
    return;
}

var builder = Host.CreateApplicationBuilder(args);

// Verificar connection string antes de construir o host
var tempConfig = builder.Configuration;
var connectionString = tempConfig["ConnectionStrings:DefaultConnection"] ?? tempConfig["connectionStrings:defaultConnection"];

if (string.IsNullOrWhiteSpace(connectionString))
{
    Console.WriteLine("Erro: ConnectionStrings:DefaultConnection não configurada em appsettings.json ou variáveis de ambiente.");
    Environment.Exit(1);
}

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddScoped<CadastroProfessorService>();
builder.Services.AddScoped<CadastroAtletaService>();
builder.Services.AddScoped<CadastrarSessaoDeTreinoService>();
builder.Services.AddScoped<GerenciarProvaAlvoService>();
builder.Services.AddScoped<DemoSeedRunner>();

using var host = builder.Build();
using var scope = host.Services.CreateScope();

try
{
    var runner = scope.ServiceProvider.GetRequiredService<DemoSeedRunner>();
    var report = await runner.RunAsync(options);
    Console.WriteLine(DemoSeedReportFormatter.Format(report));
}
catch (Exception ex)
{
    Console.WriteLine($"Erro ao executar o seed: {ex.Message}");
    if (ex.InnerException != null)
    {
        Console.WriteLine($"Detalhes: {ex.InnerException.Message}");
    }
    Environment.Exit(1);
}
