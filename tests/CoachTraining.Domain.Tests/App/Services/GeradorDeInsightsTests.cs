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
        var atleta = new Atleta("Teste ACWR", Guid.NewGuid());
        var hoje = DateOnly.FromDateTime(DateTime.UtcNow);
        var sessoes = new List<SessaoDeTreino>
        {
            new SessaoDeTreino(Guid.NewGuid(), hoje.AddDays(-3), TipoDeTreino.Intervalado, 120, 10.0, new RPE(9)),
            new SessaoDeTreino(Guid.NewGuid(), hoje.AddDays(-2), TipoDeTreino.Longo, 180, 20.0, new RPE(9)),
            new SessaoDeTreino(Guid.NewGuid(), hoje.AddDays(-1), TipoDeTreino.Intervalado, 120, 10.0, new RPE(9)),
        };

        var dashboard = _service.ObterDashboard(atleta, sessoes);

        Assert.Contains(dashboard.Insights, s => s.Contains("ACWR") || s.Contains("Risco"));
    }

    [Fact]
    public void Insights_IncludesDeltaHigh_WhenDeltaHigh()
    {
        var atleta = new Atleta("Teste Delta", Guid.NewGuid());
        var hoje = DateOnly.FromDateTime(DateTime.UtcNow);
        var sessoes = new List<SessaoDeTreino>
        {
            new SessaoDeTreino(Guid.NewGuid(), hoje.AddDays(-14), TipoDeTreino.Leve, 45, 5.0, new RPE(3)),
            new SessaoDeTreino(Guid.NewGuid(), hoje.AddDays(-13), TipoDeTreino.Leve, 45, 5.0, new RPE(3)),
            new SessaoDeTreino(Guid.NewGuid(), hoje.AddDays(-2), TipoDeTreino.Intervalado, 120, 10.0, new RPE(8)),
            new SessaoDeTreino(Guid.NewGuid(), hoje.AddDays(-1), TipoDeTreino.Longo, 180, 20.0, new RPE(8)),
        };

        var dashboard = _service.ObterDashboard(atleta, sessoes);

        Assert.Contains(dashboard.Insights, s => s.Contains("Aumento de carga >20%"));
    }

    [Fact]
    public void Insights_TaperMessage_WhenInTaperWindow()
    {
        var atleta = new Atleta("Teste Taper", Guid.NewGuid());
        var hoje = DateOnly.FromDateTime(DateTime.UtcNow);
        var prova = new ProvaAlvo(hoje.AddDays(10), 42.0, "Maratona teste");

        var sessoes = new List<SessaoDeTreino>
        {
            new SessaoDeTreino(Guid.NewGuid(), hoje.AddDays(-7), TipoDeTreino.Longo, 180, 20.0, new RPE(7)),
            new SessaoDeTreino(Guid.NewGuid(), hoje.AddDays(-6), TipoDeTreino.Ritmo, 90, 5.0, new RPE(6)),
            new SessaoDeTreino(Guid.NewGuid(), hoje.AddDays(-5), TipoDeTreino.Intervalado, 60, 5.0, new RPE(6)),
            new SessaoDeTreino(Guid.NewGuid(), hoje.AddDays(-4), TipoDeTreino.Leve, 45, 5.0, new RPE(4)),
            new SessaoDeTreino(Guid.NewGuid(), hoje.AddDays(-3), TipoDeTreino.Ritmo, 60, 5.0, new RPE(5)),
            new SessaoDeTreino(Guid.NewGuid(), hoje.AddDays(-2), TipoDeTreino.Leve, 45, 5.0, new RPE(4)),
            new SessaoDeTreino(Guid.NewGuid(), hoje.AddDays(-1), TipoDeTreino.Leve, 30, 3.0, new RPE(3)),
        };

        var dashboard = _service.ObterDashboard(atleta, sessoes, prova);

        Assert.Contains(dashboard.Insights, s => s.Contains("Taper") || s.Contains("Polimento") || s.Contains("redução"));
    }
    [Fact]
    public void Insights_AdicionaAlertaQuandoAderenciaAoPlanejamentoEstaBaixa()
    {
        var dashboard = new CoachTraining.App.DTOs.DashboardAtletaDto
        {
            TreinosPlanejadosPorSemana = 5,
            TreinosRealizadosNaSemana = 2,
            AderenciaPlanejamentoPercentual = 40
        };

        var insights = GeradorDeInsights.GerarInsights(dashboard);

        Assert.Contains(insights, insight => insight.Contains("planejamento", StringComparison.OrdinalIgnoreCase));
    }
    [Fact]
    public void Insights_AdicionaAlertaQuandoMonotoniaSemanalEstaElevada()
    {
        var hoje = DateOnly.FromDateTime(DateTime.UtcNow);
        var dashboard = new CoachTraining.App.DTOs.DashboardAtletaDto
        {
            DataUltimaAtualizacao = DateTime.UtcNow,
            TreinosJanela =
            [
                new() { Data = hoje.AddDays(-6), Carga = 100 },
                new() { Data = hoje.AddDays(-5), Carga = 100 },
                new() { Data = hoje.AddDays(-4), Carga = 100 },
                new() { Data = hoje.AddDays(-3), Carga = 100 },
                new() { Data = hoje.AddDays(-2), Carga = 100 },
                new() { Data = hoje.AddDays(-1), Carga = 100 }
            ]
        };

        var insights = GeradorDeInsights.GerarInsights(dashboard);

        Assert.Contains(insights, insight => insight.Contains("Monotonia", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Insights_AdicionaAlertaQuandoCargaSobeEMelhoraDeRendimentoNaoAcompanha()
    {
        var hoje = DateOnly.FromDateTime(DateTime.UtcNow);
        var dashboard = new CoachTraining.App.DTOs.DashboardAtletaDto
        {
            SerieCargaSemanal =
            [
                new() { SemanaInicio = hoje.AddDays(-28), SemanaFim = hoje.AddDays(-22), Valor = 800 },
                new() { SemanaInicio = hoje.AddDays(-21), SemanaFim = hoje.AddDays(-15), Valor = 820 },
                new() { SemanaInicio = hoje.AddDays(-14), SemanaFim = hoje.AddDays(-8), Valor = 1040 },
                new() { SemanaInicio = hoje.AddDays(-7), SemanaFim = hoje.AddDays(-1), Valor = 1080 }
            ],
            SeriePaceSemanal =
            [
                new() { SemanaInicio = hoje.AddDays(-28), SemanaFim = hoje.AddDays(-22), ValorMinPorKm = 5.0 },
                new() { SemanaInicio = hoje.AddDays(-21), SemanaFim = hoje.AddDays(-15), ValorMinPorKm = 4.95 },
                new() { SemanaInicio = hoje.AddDays(-14), SemanaFim = hoje.AddDays(-8), ValorMinPorKm = 5.25 },
                new() { SemanaInicio = hoje.AddDays(-7), SemanaFim = hoje.AddDays(-1), ValorMinPorKm = 5.35 }
            ]
        };

        var insights = GeradorDeInsights.GerarInsights(dashboard);

        Assert.Contains(insights, insight => insight.Contains("rendimento", StringComparison.OrdinalIgnoreCase));
    }
}
