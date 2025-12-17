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
        var atleta = new Atleta("João Silva");
        var hoje = DateOnly.FromDateTime(DateTime.UtcNow);

        var sessoes = new List<SessaoDeTreino>
        {
            new SessaoDeTreino(hoje.AddDays(-3), TipoDeTreino.Leve, 60, 5.0, new RPE(5)),
            new SessaoDeTreino(hoje.AddDays(-2), TipoDeTreino.Ritmo, 60, 5.0, new RPE(6)),
            new SessaoDeTreino(hoje.AddDays(-1), TipoDeTreino.Leve, 60, 5.0, new RPE(5)),
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
        var atleta = new Atleta("Maria Santos");
        var hoje = DateOnly.FromDateTime(DateTime.UtcNow);

        var sessoes = new List<SessaoDeTreino>
        {
            new SessaoDeTreino(hoje.AddDays(-6), TipoDeTreino.Leve, 60, 5.0, new RPE(4)),
            new SessaoDeTreino(hoje.AddDays(-5), TipoDeTreino.Ritmo, 90, 5.0, new RPE(5)),
            new SessaoDeTreino(hoje.AddDays(-4), TipoDeTreino.Intervalado, 75, 5.0, new RPE(7)),
            new SessaoDeTreino(hoje.AddDays(-3), TipoDeTreino.Longo, 120, 10.0, new RPE(6)),
            new SessaoDeTreino(hoje.AddDays(-2), TipoDeTreino.Ritmo, 90, 5.0, new RPE(7)),
            new SessaoDeTreino(hoje.AddDays(-1), TipoDeTreino.Leve, 60, 5.0, new RPE(5)),
        };

        var dashboard = _service.ObterDashboard(atleta, sessoes);

        Assert.NotNull(dashboard);
        Assert.Equal(FaseDoCiclo.Construcao, dashboard.FaseAtual);
        Assert.True(dashboard.DeltaPercentualSemanal >= 0);
    }

    [Fact]
    public void ObterDashboard_ComCargaElevada_RetornaPico()
    {
        var atleta = new Atleta("Pedro Costa");
        var hoje = DateOnly.FromDateTime(DateTime.UtcNow);

        var sessoes = new List<SessaoDeTreino>
        {
            new SessaoDeTreino(hoje.AddDays(-3), TipoDeTreino.Intervalado, 120, 10.0, new RPE(9)),
            new SessaoDeTreino(hoje.AddDays(-2), TipoDeTreino.Longo, 180, 20.0, new RPE(8)),
            new SessaoDeTreino(hoje.AddDays(-1), TipoDeTreino.Intervalado, 120, 10.0, new RPE(9)),
        };

        var dashboard = _service.ObterDashboard(atleta, sessoes);

        Assert.NotNull(dashboard);
        Assert.Equal(FaseDoCiclo.Pico, dashboard.FaseAtual);
        Assert.True(dashboard.CargaAguda > 0);
    }

    [Fact]
    public void ObterDashboard_SemSessoes_RetornaDashboardVazio()
    {
        var atleta = new Atleta("Ana Paula");
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
        var atleta = new Atleta("Carlos Mendes");
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

        Assert.NotNull(dashboard);
        Assert.True(dashboard.EmJanelaDeTaper);
        Assert.Equal(FaseDoCiclo.Polimento, dashboard.FaseAtual);
        Assert.NotNull(dashboard.ReducaoVolumeTaper);
        Assert.True(dashboard.ReducaoVolumeTaper >= 0.0);
    }

    [Fact]
    public void ObterDashboard_ACWRAlto_RetornaRisco()
    {
        var atleta = new Atleta("Fabiana Oliveira");
        var hoje = DateOnly.FromDateTime(DateTime.UtcNow);

        var sessoes = new List<SessaoDeTreino>
        {
            new SessaoDeTreino(hoje.AddDays(-30), TipoDeTreino.Leve, 60, 5.0, new RPE(4)),
            new SessaoDeTreino(hoje.AddDays(-23), TipoDeTreino.Ritmo, 90, 5.0, new RPE(5)),
            new SessaoDeTreino(hoje.AddDays(-16), TipoDeTreino.Leve, 60, 5.0, new RPE(4)),
            new SessaoDeTreino(hoje.AddDays(-9), TipoDeTreino.Ritmo, 90, 5.0, new RPE(5)),
            new SessaoDeTreino(hoje.AddDays(-3), TipoDeTreino.Intervalado, 120, 10.0, new RPE(9)),
            new SessaoDeTreino(hoje.AddDays(-2), TipoDeTreino.Longo, 180, 20.0, new RPE(9)),
            new SessaoDeTreino(hoje.AddDays(-1), TipoDeTreino.Intervalado, 120, 10.0, new RPE(9)),
        };

        var dashboard = _service.ObterDashboard(atleta, sessoes);

        Assert.NotNull(dashboard);
        Assert.True(dashboard.ACWR > 1.5);
        Assert.Equal(StatusDeRisco.Risco, dashboard.StatusRisco);
    }

    [Fact]
    public void ObterDashboard_DeltaPercentualAlto_RetornaRisco()
    {
        var atleta = new Atleta("Roberto Silva");
        var hoje = DateOnly.FromDateTime(DateTime.UtcNow);

        var sessoes = new List<SessaoDeTreino>
        {
            new SessaoDeTreino(hoje.AddDays(-14), TipoDeTreino.Leve, 45, 5.0, new RPE(3)),
            new SessaoDeTreino(hoje.AddDays(-13), TipoDeTreino.Leve, 45, 5.0, new RPE(3)),
            new SessaoDeTreino(hoje.AddDays(-2), TipoDeTreino.Intervalado, 120, 10.0, new RPE(8)),
            new SessaoDeTreino(hoje.AddDays(-1), TipoDeTreino.Longo, 180, 20.0, new RPE(8)),
        };

        var dashboard = _service.ObterDashboard(atleta, sessoes);

        Assert.NotNull(dashboard);
        Assert.True(dashboard.DeltaPercentualSemanal > 20);
        Assert.True(dashboard.StatusRisco == StatusDeRisco.Risco || dashboard.StatusRisco == StatusDeRisco.Atencao);
    }

    [Fact]
    public void ObterDashboard_ComObservacoesClin_AsservaAsObservacoes()
    {
        var observacoes = "Atleta com histórico de lesão no joelho";
        var atleta = new Atleta("Gustavo Pereira", observacoesClinicas: observacoes);
        var sessoes = new List<SessaoDeTreino>
        {
            new SessaoDeTreino(
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
        var atleta = new Atleta("Lucia Ferreira", nivelEsportivo: nivel);
        var sessoes = new List<SessaoDeTreino>
        {
            new SessaoDeTreino(
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
        var atleta = new Atleta("Test");

        var exception = Assert.Throws<ArgumentNullException>(
            () => _service.ObterDashboard(atleta, null!));

        Assert.Equal("sessoes", exception.ParamName);
    }
}
