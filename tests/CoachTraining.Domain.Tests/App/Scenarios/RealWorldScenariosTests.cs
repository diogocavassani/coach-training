using System.Globalization;
using CoachTraining.App.Services;
using CoachTraining.Domain.Entities;
using CoachTraining.Domain.Enums;
using CoachTraining.Domain.ValueObjects;

namespace CoachTraining.Tests.App.Scenarios;

public class RealWorldScenariosTests
{
    private readonly ObterDashboardAtletaService _service = new ObterDashboardAtletaService();

    private static (int Ano, int Semana) ObterAnoSemana(DateOnly data)
    {
        var cal = CultureInfo.CurrentCulture.Calendar;
        var numSemana = cal.GetWeekOfYear(
            data.ToDateTime(TimeOnly.MinValue),
            CalendarWeekRule.FirstFourDayWeek,
            DayOfWeek.Monday);

        return (data.Year, numSemana);
    }

    // Picks 3 dates from the last 7 days that fall in the same ISO week.
    private static List<DateOnly> SelecionarDatasTaperMesmaSemana(DateOnly hoje)
    {
        var janelaTaper = Enumerable.Range(1, 7)
            .Select(i => hoje.AddDays(-i))
            .ToList();

        var semanaComMaisDias = janelaTaper
            .GroupBy(d => ObterAnoSemana(d))
            .OrderByDescending(g => g.Count())
            .First();

        return semanaComMaisDias
            .OrderBy(d => d)
            .Take(3)
            .ToList();
    }

    [Fact]
    public void Cenario_Iniciante_RetornaBaseENormal()
    {
        var atleta = new Atleta("Iniciante");
        var hoje = DateOnly.FromDateTime(DateTime.UtcNow);
        // Low volume, few sessions
        var sessoes = new List<SessaoDeTreino>
        {
            new SessaoDeTreino(hoje.AddDays(-10), TipoDeTreino.Leve, 30, 3.0, new RPE(3)),
            new SessaoDeTreino(hoje.AddDays(-6), TipoDeTreino.Leve, 30, 3.0, new RPE(3)),
            new SessaoDeTreino(hoje.AddDays(-2), TipoDeTreino.Leve, 30, 3.0, new RPE(3)),
        };

        var dashboard = _service.ObterDashboard(atleta, sessoes);

        Assert.Equal(FaseDoCiclo.Base, dashboard.FaseAtual);
        Assert.Equal(StatusDeRisco.Normal, dashboard.StatusRisco);
    }

    [Fact]
    public void Cenario_Intermediario_RetornaConstrucao()
    {
        var atleta = new Atleta("Intermediario");
        var hoje = DateOnly.FromDateTime(DateTime.UtcNow);

        var sessoes = new List<SessaoDeTreino>();
        // Gradually increasing workload across recent days
        for (int i = 10; i >= 1; i--)  
        {
            var dur = 30 + (11 - i) * 10; // increasing duration
            sessoes.Add(new SessaoDeTreino(hoje.AddDays(-i), TipoDeTreino.Ritmo, dur, 5.0, new RPE(5 + (i % 2))));
        }

        var dashboard = _service.ObterDashboard(atleta, sessoes);

        Assert.Equal(FaseDoCiclo.Construcao, dashboard.FaseAtual);
    }

    [Fact]
    public void Cenario_Avancado_RetornaPico()
    {
        var atleta = new Atleta("Avancado");
        var hoje = DateOnly.FromDateTime(DateTime.UtcNow);

        var sessoes = new List<SessaoDeTreino>
        {
            new SessaoDeTreino(hoje.AddDays(-21), TipoDeTreino.Longo, 180, 20.0, new RPE(8)),
            new SessaoDeTreino(hoje.AddDays(-14), TipoDeTreino.Longo, 180, 20.0, new RPE(8)),
            new SessaoDeTreino(hoje.AddDays(-7), TipoDeTreino.Longo, 180, 20.0, new RPE(9)),
            new SessaoDeTreino(hoje.AddDays(-1), TipoDeTreino.Intervalado, 150, 15.0, new RPE(9)),
        };

        var dashboard = _service.ObterDashboard(atleta, sessoes);

        Assert.Equal(FaseDoCiclo.Pico, dashboard.FaseAtual);
    }

    [Fact]
    public void Cenario_Overreaching_RetornaRisco()
    {
        var atleta = new Atleta("Overreaching");
        var hoje = DateOnly.FromDateTime(DateTime.UtcNow);

        var sessoes = new List<SessaoDeTreino>
        {
            // base low training in earlier weeks
            new SessaoDeTreino(hoje.AddDays(-30), TipoDeTreino.Leve, 45, 5.0, new RPE(4)),
            new SessaoDeTreino(hoje.AddDays(-23), TipoDeTreino.Leve, 45, 5.0, new RPE(4)),
            // sudden spike in last week
            new SessaoDeTreino(hoje.AddDays(-3), TipoDeTreino.Intervalado, 180, 20.0, new RPE(9)),
            new SessaoDeTreino(hoje.AddDays(-2), TipoDeTreino.Longo, 240, 30.0, new RPE(9)),
            new SessaoDeTreino(hoje.AddDays(-1), TipoDeTreino.Intervalado, 180, 20.0, new RPE(9)),
        };

        var dashboard = _service.ObterDashboard(atleta, sessoes);

        Assert.True(dashboard.ACWR > 1.5 || dashboard.StatusRisco == StatusDeRisco.Risco);
    }

    [Fact]
    public void Cenario_TaperBemExecutado_RetornaPolimentoEValidacao()
    {
        var hoje = DateOnly.FromDateTime(DateTime.UtcNow);
        var datasTaper = SelecionarDatasTaperMesmaSemana(hoje);
        var prova = new ProvaAlvo(hoje.AddDays(14), 42.0, "Maratona");
        var atleta = new Atleta("Taper");

        var sessoes = new List<SessaoDeTreino>
        {
            // workload earlier (3 weeks before)
            new SessaoDeTreino(hoje.AddDays(-28), TipoDeTreino.Longo, 180, 20.0, new RPE(7)),
            new SessaoDeTreino(hoje.AddDays(-21), TipoDeTreino.Longo, 180, 20.0, new RPE(7)),
            new SessaoDeTreino(hoje.AddDays(-14), TipoDeTreino.Longo, 180, 20.0, new RPE(7)),
            // taper week: reduced volume ~50%
            new SessaoDeTreino(datasTaper[0], TipoDeTreino.Leve, 60, 5.0, new RPE(4)),
            new SessaoDeTreino(datasTaper[1], TipoDeTreino.Leve, 45, 5.0, new RPE(4)),
            new SessaoDeTreino(datasTaper[2], TipoDeTreino.Leve, 30, 3.0, new RPE(3)),
        };

        var dashboard = _service.ObterDashboard(atleta, sessoes, prova);

        Assert.Equal(FaseDoCiclo.Polimento, dashboard.FaseAtual);
        Assert.NotNull(dashboard.ReducaoVolumeTaper);
        Assert.True(dashboard.ReducaoVolumeTaper >= 0.4 && dashboard.ReducaoVolumeTaper <= 0.6);
    }
}
