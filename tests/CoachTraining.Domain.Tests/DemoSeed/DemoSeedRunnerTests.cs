using CoachTraining.App.Abstractions.Persistence;
using CoachTraining.App.Abstractions.Security;
using CoachTraining.App.Services;
using CoachTraining.DemoSeed;
using CoachTraining.Infra;
using CoachTraining.Infra.Persistence;
using CoachTraining.Infra.Persistence.Repositories;
using CoachTraining.Infra.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CoachTraining.Tests.DemoSeed;

public class DemoSeedRunnerTests
{
    [Fact]
    public async Task RunAsync_ComResetDemo_DeveCriarProfessorSeisAtletasEHistorico()
    {
        await using var provider = BuildProvider(Guid.NewGuid().ToString("N"));
        var runner = provider.GetRequiredService<DemoSeedRunner>();

        var report = await runner.RunAsync(
            new DemoSeedOptions("demo-v1", true, false, false),
            new DateOnly(2026, 4, 2)
        );

        var db = provider.GetRequiredService<CoachTrainingDbContext>();

        Assert.Equal("demo.professor@coachtraining.local", report.ProfessorEmail);
        Assert.Equal(6, report.Atletas.Count);
        Assert.Equal(6, db.Atletas.Count());
        Assert.True(db.SessoesDeTreino.Count() >= 60, "Deve ter pelo menos 60 sessões");
    }

    [Fact]
    public async Task RunAsync_ExecutadoDuasVezesComResetDemo_NaoDeveDuplicarDataset()
    {
        await using var provider = BuildProvider(Guid.NewGuid().ToString("N"));
        var runner = provider.GetRequiredService<DemoSeedRunner>();
        var dataReferencia = new DateOnly(2026, 4, 2);

        await runner.RunAsync(new DemoSeedOptions("demo-v1", true, false, false), dataReferencia);
        await runner.RunAsync(new DemoSeedOptions("demo-v1", true, false, false), dataReferencia);

        var db = provider.GetRequiredService<CoachTrainingDbContext>();
        Assert.Equal(1, db.Professores.Count());
        Assert.Equal(6, db.Atletas.Count());
    }

    [Fact]
    public async Task RunAsync_ReportDeveConterAtletasComInsights()
    {
        await using var provider = BuildProvider(Guid.NewGuid().ToString("N"));
        var runner = provider.GetRequiredService<DemoSeedRunner>();

        var report = await runner.RunAsync(
            new DemoSeedOptions("demo-v1", false, false, false),
            new DateOnly(2026, 4, 2)
        );

        Assert.NotEmpty(report.Atletas);
        Assert.All(report.Atletas, atleta =>
        {
            Assert.NotEmpty(atleta.Nome);
            Assert.NotEmpty(atleta.Email);
            Assert.NotEmpty(atleta.InsightEsperado);
        });
    }

    private static ServiceProvider BuildProvider(string databaseName)
    {
        var services = new ServiceCollection();

        services.AddDbContext<CoachTrainingDbContext>(options =>
            options.UseInMemoryDatabase(databaseName)
        );

        services.AddScoped<IAtletaRepository, AtletaRepository>();
        services.AddScoped<IProfessorRepository, ProfessorRepository>();
        services.AddScoped<ISessaoDeTreinoRepository, SessaoDeTreinoRepository>();
        services.AddScoped<IProvaAlvoRepository, ProvaAlvoRepository>();
        services.AddScoped<IPasswordHasher, BCryptPasswordHasher>();

        services.AddScoped<CadastroProfessorService>();
        services.AddScoped<CadastroAtletaService>();
        services.AddScoped<CadastrarSessaoDeTreinoService>();
        services.AddScoped<GerenciarProvaAlvoService>();
        services.AddScoped<GerenciarPlanejamentoBaseService>();
        services.AddScoped<DemoSeedRunner>();

        return services.BuildServiceProvider();
    }
}
