using CoachTraining.App.Services;
using CoachTraining.Domain.Entities;
using CoachTraining.Domain.Enums;
using CoachTraining.Domain.ValueObjects;

namespace CoachTraining.Tests.App.Services;

public class GeradorDeInsightsTests
{
    private readonly ObterDashboardAtletaService _service = new ObterDashboardAtletaService();

    [Fact]
    public void Insights_IncludesAcwrHigh_WhenAcwrHigh()
    {
        var atleta = new Atleta("Teste ACWR");
        var hoje = DateOnly.FromDateTime(DateTime.UtcNow);
        var sessoes = new List<SessaoDeTreino>
        {
            new SessaoDeTreino(hoje.AddDays(-3), TipoDeTreino.Intervalado, 120, 10.0, new RPE(9)),
            new SessaoDeTreino(hoje.AddDays(-2), TipoDeTreino.Longo, 180, 20.0, new RPE(9)),
            new SessaoDeTreino(hoje.AddDays(-1), TipoDeTreino.Intervalado, 120, 10.0, new RPE(9)),
        };

        var dashboard = _service.ObterDashboard(atleta, sessoes);

        Assert.Contains(dashboard.Insights, s => s.Contains("ACWR") || s.Contains("Risco"));
    }

    [Fact]
    public void Insights_IncludesDeltaHigh_WhenDeltaHigh()
    {
        var atleta = new Atleta("Teste Delta");
        var hoje = DateOnly.FromDateTime(DateTime.UtcNow);
        var sessoes = new List<SessaoDeTreino>
        {
            new SessaoDeTreino(hoje.AddDays(-14), TipoDeTreino.Leve, 45, 5.0, new RPE(3)),
            new SessaoDeTreino(hoje.AddDays(-13), TipoDeTreino.Leve, 45, 5.0, new RPE(3)),
            new SessaoDeTreino(hoje.AddDays(-2), TipoDeTreino.Intervalado, 120, 10.0, new RPE(8)),
            new SessaoDeTreino(hoje.AddDays(-1), TipoDeTreino.Longo, 180, 20.0, new RPE(8)),
        };

        var dashboard = _service.ObterDashboard(atleta, sessoes);

        Assert.Contains(dashboard.Insights, s => s.Contains("Aumento de carga >20%"));
    }

    [Fact]
    public void Insights_TaperMessage_WhenInTaperWindow()
    {
        var atleta = new Atleta("Teste Taper");
        var hoje = DateOnly.FromDateTime(DateTime.UtcNow);
        var prova = new ProvaAlvo(hoje.AddDays(10), 42.0, "Maratona teste");

        var sessoes = new List<SessaoDeTreino>
        {
            new SessaoDeTreino(hoje.AddDays(-7), TipoDeTreino.Longo, 180, 20.0, new RPE(7)),
            new SessaoDeTreino(hoje.AddDays(-6), TipoDeTreino.Ritmo, 90, 5.0, new RPE(6)),
            new SessaoDeTreino(hoje.AddDays(-5), TipoDeTreino.Intervalado, 60, 5.0, new RPE(6)),
            new SessaoDeTreino(hoje.AddDays(-4), TipoDeTreino.Leve, 45, 5.0, new RPE(4)),
            new SessaoDeTreino(hoje.AddDays(-3), TipoDeTreino.Ritmo, 60, 5.0, new RPE(5)),
            new SessaoDeTreino(hoje.AddDays(-2), TipoDeTreino.Leve, 45, 5.0, new RPE(4)),
            new SessaoDeTreino(hoje.AddDays(-1), TipoDeTreino.Leve, 30, 3.0, new RPE(3)),
        };

        var dashboard = _service.ObterDashboard(atleta, sessoes, prova);

        Assert.Contains(dashboard.Insights, s => s.Contains("Taper") || s.Contains("Polimento") || s.Contains("redução"));
    }
}
