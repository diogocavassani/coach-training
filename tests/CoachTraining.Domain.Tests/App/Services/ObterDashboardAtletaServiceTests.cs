using CoachTraining.App.Services;
using CoachTraining.Domain.Entities;
using CoachTraining.Domain.Enums;
using CoachTraining.Domain.ValueObjects;

namespace CoachTraining.Tests.App.Services;

public class ObterDashboardAtletaServiceTests
{
    private readonly ObterDashboardAtletaService _service;

    public ObterDashboardAtletaServiceTests()
    {
        _service = new ObterDashboardAtletaService();
    }

    [Fact]
    public void ObterDashboard_ComSessoesEstavel_RetornaDashboardComRiscoNormal()
    {
        var atleta = new Atleta("João Silva", Guid.NewGuid());
        var hoje = DateOnly.FromDateTime(DateTime.UtcNow);

        var sessoes = new List<SessaoDeTreino>
        {
            new SessaoDeTreino(Guid.NewGuid(), hoje.AddDays(-3), TipoDeTreino.Leve, 60, 5.0, new RPE(5)),
            new SessaoDeTreino(Guid.NewGuid(), hoje.AddDays(-2), TipoDeTreino.Ritmo, 60, 5.0, new RPE(6)),
            new SessaoDeTreino(Guid.NewGuid(), hoje.AddDays(-1), TipoDeTreino.Leve, 60, 5.0, new RPE(5)),
        };

        var dashboard = _service.ObterDashboard(atleta, sessoes);

        Assert.NotNull(dashboard);
        Assert.Equal(atleta.Id, dashboard.AtletaId);
        Assert.Equal(atleta.Nome, dashboard.Nome);
        Assert.Equal(StatusDeRisco.Normal, dashboard.StatusRisco);
        Assert.True(dashboard.CargaSemanal > 0);
    }

    [Fact]
    public void ObterDashboard_ComCargaCrescente_RetornaConstrucao()
    {
        var atleta = new Atleta("Maria Santos", Guid.NewGuid());
        var hoje = DateOnly.FromDateTime(DateTime.UtcNow);

        var sessoes = new List<SessaoDeTreino>
        {
            new SessaoDeTreino(Guid.NewGuid(), hoje.AddDays(-6), TipoDeTreino.Leve, 60, 5.0, new RPE(4)),
            new SessaoDeTreino(Guid.NewGuid(), hoje.AddDays(-5), TipoDeTreino.Ritmo, 90, 5.0, new RPE(5)),
            new SessaoDeTreino(Guid.NewGuid(), hoje.AddDays(-4), TipoDeTreino.Intervalado, 75, 5.0, new RPE(7)),
            new SessaoDeTreino(Guid.NewGuid(), hoje.AddDays(-3), TipoDeTreino.Longo, 120, 10.0, new RPE(6)),
            new SessaoDeTreino(Guid.NewGuid(), hoje.AddDays(-2), TipoDeTreino.Ritmo, 90, 5.0, new RPE(7)),
            new SessaoDeTreino(Guid.NewGuid(), hoje.AddDays(-1), TipoDeTreino.Leve, 60, 5.0, new RPE(5)),
        };

        var dashboard = _service.ObterDashboard(atleta, sessoes);

        Assert.NotNull(dashboard);
        Assert.Equal(FaseDoCiclo.Base, dashboard.FaseAtual);
        Assert.True(dashboard.DeltaPercentualSemanal >= 0);
    }

    [Fact]
    public void ObterDashboard_ComCargaElevada_RetornaPico()
    {
        var atleta = new Atleta("Pedro Costa", Guid.NewGuid());
        var hoje = DateOnly.FromDateTime(DateTime.UtcNow);

        var sessoes = new List<SessaoDeTreino>
        {
            new SessaoDeTreino(Guid.NewGuid(), hoje.AddDays(-3), TipoDeTreino.Intervalado, 120, 10.0, new RPE(9)),
            new SessaoDeTreino(Guid.NewGuid(), hoje.AddDays(-2), TipoDeTreino.Longo, 180, 20.0, new RPE(8)),
            new SessaoDeTreino(Guid.NewGuid(), hoje.AddDays(-1), TipoDeTreino.Intervalado, 120, 10.0, new RPE(9)),
        };

        var dashboard = _service.ObterDashboard(atleta, sessoes);

        Assert.NotNull(dashboard);
        Assert.Equal(FaseDoCiclo.Pico, dashboard.FaseAtual);
        Assert.True(dashboard.CargaAguda > 0);
    }

    [Fact]
    public void ObterDashboard_SemSessoes_RetornaDashboardVazio()
    {
        var atleta = new Atleta("Ana Paula", Guid.NewGuid());
        var sessoes = new List<SessaoDeTreino>();

        var dashboard = _service.ObterDashboard(atleta, sessoes);

        Assert.NotNull(dashboard);
        Assert.Equal(atleta.Id, dashboard.AtletaId);
        Assert.Equal(0, dashboard.CargaSemanal);
        Assert.Equal(0, dashboard.CargaUltimaSessao);
        Assert.Equal(StatusDeRisco.Normal, dashboard.StatusRisco);
        Assert.Equal(FaseDoCiclo.Base, dashboard.FaseAtual);
    }

    [Fact]
    public void ObterDashboard_EmTaper_RetornaPolimentoEReducaoVolume()
    {
        var atleta = new Atleta("Carlos Mendes", Guid.NewGuid());
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

        Assert.NotNull(dashboard);
        Assert.True(dashboard.EmJanelaDeTaper);
        Assert.Equal(FaseDoCiclo.Polimento, dashboard.FaseAtual);
        Assert.NotNull(dashboard.ReducaoVolumeTaper);
        Assert.True(dashboard.ReducaoVolumeTaper >= 0.0);
    }

    [Fact]
    public void ObterDashboard_ACWRAlto_RetornaRisco()
    {
        var atleta = new Atleta("Fabiana Oliveira", Guid.NewGuid());
        var hoje = DateOnly.FromDateTime(DateTime.UtcNow);

        var sessoes = new List<SessaoDeTreino>
        {
            new SessaoDeTreino(Guid.NewGuid(), hoje.AddDays(-30), TipoDeTreino.Leve, 60, 5.0, new RPE(4)),
            new SessaoDeTreino(Guid.NewGuid(), hoje.AddDays(-23), TipoDeTreino.Ritmo, 90, 5.0, new RPE(5)),
            new SessaoDeTreino(Guid.NewGuid(), hoje.AddDays(-16), TipoDeTreino.Leve, 60, 5.0, new RPE(4)),
            new SessaoDeTreino(Guid.NewGuid(), hoje.AddDays(-9), TipoDeTreino.Ritmo, 90, 5.0, new RPE(5)),
            new SessaoDeTreino(Guid.NewGuid(), hoje.AddDays(-3), TipoDeTreino.Intervalado, 120, 10.0, new RPE(9)),
            new SessaoDeTreino(Guid.NewGuid(), hoje.AddDays(-2), TipoDeTreino.Longo, 180, 20.0, new RPE(9)),
            new SessaoDeTreino(Guid.NewGuid(), hoje.AddDays(-1), TipoDeTreino.Intervalado, 120, 10.0, new RPE(9)),
        };

        var dashboard = _service.ObterDashboard(atleta, sessoes);

        Assert.NotNull(dashboard);
        Assert.True(dashboard.ACWR > 1.5);
        Assert.Equal(StatusDeRisco.Risco, dashboard.StatusRisco);
    }

    [Fact]
    public void ObterDashboard_DeltaPercentualAlto_RetornaRisco()
    {
        var atleta = new Atleta("Roberto Silva", Guid.NewGuid());
        var hoje = DateOnly.FromDateTime(DateTime.UtcNow);

        var sessoes = new List<SessaoDeTreino>
        {
            new SessaoDeTreino(Guid.NewGuid(), hoje.AddDays(-14), TipoDeTreino.Leve, 45, 5.0, new RPE(3)),
            new SessaoDeTreino(Guid.NewGuid(), hoje.AddDays(-13), TipoDeTreino.Leve, 45, 5.0, new RPE(3)),
            new SessaoDeTreino(Guid.NewGuid(), hoje.AddDays(-2), TipoDeTreino.Intervalado, 120, 10.0, new RPE(8)),
            new SessaoDeTreino(Guid.NewGuid(), hoje.AddDays(-1), TipoDeTreino.Longo, 180, 20.0, new RPE(8)),
        };

        var dashboard = _service.ObterDashboard(atleta, sessoes);

        Assert.NotNull(dashboard);
        Assert.True(dashboard.DeltaPercentualSemanal > 20);
        Assert.Equal(StatusDeRisco.Risco, dashboard.StatusRisco);
    }

    [Fact]
    public void ObterDashboard_ComObservacoesClin_AsservaAsObservacoes()
    {
        var observacoes = "Atleta com histórico de lesão no joelho";
        var atleta = new Atleta("Gustavo Pereira", Guid.NewGuid(), observacoesClinicas: observacoes);
        var sessoes = new List<SessaoDeTreino>
        {
            new SessaoDeTreino(Guid.NewGuid(), 
                DateOnly.FromDateTime(DateTime.UtcNow).AddDays(-1),
                TipoDeTreino.Leve,
                60,
                5.0,
                new RPE(4))
        };

        var dashboard = _service.ObterDashboard(atleta, sessoes);

        Assert.NotNull(dashboard);
        Assert.Equal(observacoes, dashboard.ObservacoesClin);
    }

    [Fact]
    public void ObterDashboard_ComNivelAtleta_AssertaNivel()
    {
        var nivel = "Intermediário";
        var atleta = new Atleta("Lucia Ferreira", Guid.NewGuid(), nivelEsportivo: nivel);
        var sessoes = new List<SessaoDeTreino>
        {
            new SessaoDeTreino(Guid.NewGuid(), 
                DateOnly.FromDateTime(DateTime.UtcNow).AddDays(-1),
                TipoDeTreino.Ritmo,
                90,
                5.0,
                new RPE(6))
        };

        var dashboard = _service.ObterDashboard(atleta, sessoes);

        Assert.NotNull(dashboard);
        Assert.Equal(nivel, dashboard.NivelAtleta);
    }

    [Fact]
    public void SessaoDeTreino_ComDuracaoZero_DeveLancarErroNoConstrutor()
    {
        var hoje = DateOnly.FromDateTime(DateTime.UtcNow);

        Assert.Throws<ArgumentOutOfRangeException>(() =>
            new SessaoDeTreino(Guid.NewGuid(), hoje.AddDays(-1), TipoDeTreino.Leve, 0, 0.0, new RPE(4)));
    }

    [Fact]
    public void ObterDashboard_DeveGerarSerieCargaSemanalCom12PontosOrdenados()
    {
        var atleta = new Atleta("Atleta Serie Carga", Guid.NewGuid());
        var hoje = DateOnly.FromDateTime(DateTime.UtcNow);
        var sessoes = new List<SessaoDeTreino>
        {
            new SessaoDeTreino(Guid.NewGuid(), hoje.AddDays(-2), TipoDeTreino.Ritmo, 50, 8.0, new RPE(6)),
            new SessaoDeTreino(Guid.NewGuid(), hoje.AddDays(-15), TipoDeTreino.Longo, 90, 14.0, new RPE(7))
        };

        var dashboard = _service.ObterDashboard(atleta, sessoes);

        Assert.Equal(12, dashboard.SerieCargaSemanal.Count);
        Assert.True(dashboard.SerieCargaSemanal.Zip(
                dashboard.SerieCargaSemanal.Skip(1),
                (atual, proximo) => proximo.SemanaInicio > atual.SemanaInicio).All(v => v));
    }

    [Fact]
    public void ObterDashboard_DeveCalcularSeriePaceSemanalComMediaPonderadaETratarSemanaSemDistancia()
    {
        var atleta = new Atleta("Atleta Pace", Guid.NewGuid());
        var hoje = DateOnly.FromDateTime(DateTime.UtcNow);
        var inicioSemanaAtual = ObterInicioSemana(hoje);
        var inicioSemanaAnterior = inicioSemanaAtual.AddDays(-7);
        var sessoes = new List<SessaoDeTreino>
        {
            // Semana atual: pace ponderado esperado = (30 + 60) / (5 + 10) = 6.0 min/km
            new SessaoDeTreino(Guid.NewGuid(), inicioSemanaAtual, TipoDeTreino.Ritmo, 30, 5.0, new RPE(6)),
            new SessaoDeTreino(Guid.NewGuid(), inicioSemanaAtual, TipoDeTreino.Longo, 60, 10.0, new RPE(7)),
            // Distancia zero na semana atual (deve ser ignorada no pace semanal)
            new SessaoDeTreino(Guid.NewGuid(), inicioSemanaAtual, TipoDeTreino.Leve, 40, 0.0, new RPE(4)),
            // Semana anterior com distancia zero (deve resultar em null)
            new SessaoDeTreino(Guid.NewGuid(), inicioSemanaAnterior, TipoDeTreino.Leve, 35, 0.0, new RPE(3))
        };

        var dashboard = _service.ObterDashboard(atleta, sessoes);

        var paceSemanaAtual = dashboard.SeriePaceSemanal.Single(p => p.SemanaInicio == inicioSemanaAtual);
        var paceSemanaAnterior = dashboard.SeriePaceSemanal.Single(p => p.SemanaInicio == inicioSemanaAnterior);

        Assert.Equal(6.0, paceSemanaAtual.ValorMinPorKm);
        Assert.Null(paceSemanaAnterior.ValorMinPorKm);
    }

    [Fact]
    public void ObterDashboard_DeveRetornarTreinosDaJanelaDe12Semanas()
    {
        var atleta = new Atleta("Atleta Janela", Guid.NewGuid());
        var hoje = DateOnly.FromDateTime(DateTime.UtcNow);
        var sessoes = new List<SessaoDeTreino>
        {
            new SessaoDeTreino(Guid.NewGuid(), hoje.AddDays(-10), TipoDeTreino.Ritmo, 45, 7.5, new RPE(6)),
            new SessaoDeTreino(Guid.NewGuid(), hoje.AddDays(-90), TipoDeTreino.Leve, 40, 6.0, new RPE(4))
        };

        var dashboard = _service.ObterDashboard(atleta, sessoes);

        Assert.Single(dashboard.TreinosJanela);
        Assert.Equal(hoje.AddDays(-10), dashboard.TreinosJanela[0].Data);
    }

    [Fact]
    public void ObterDashboard_LancaArgumentNullException_SeAtletaNull()
    {
        var sessoes = new List<SessaoDeTreino>();

        var exception = Assert.Throws<ArgumentNullException>(
            () => _service.ObterDashboard(null!, sessoes));

        Assert.Equal("atleta", exception.ParamName);
    }

    [Fact]
    public void ObterDashboard_LancaArgumentNullException_SeSessoesNull()
    {
        var atleta = new Atleta("Test", Guid.NewGuid());

        var exception = Assert.Throws<ArgumentNullException>(
            () => _service.ObterDashboard(atleta, null!));

        Assert.Equal("sessoes", exception.ParamName);
    }

    private static DateOnly ObterInicioSemana(DateOnly data)
    {
        var deslocamento = ((int)data.DayOfWeek + 6) % 7;
        return data.AddDays(-deslocamento);
    }
}
