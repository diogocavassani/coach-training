using System.Globalization;
using CoachTraining.DemoSeed.Scenarios;

namespace CoachTraining.Tests.DemoSeed;

public class DemoScenarioFactoryTests
{
    [Fact]
    public void CreateProfile_DemoV1_DeveRetornarSeisCenariosComEmailsUnicos()
    {
        var profile = DemoScenarioFactory.CreateProfile("demo-v1", new DateOnly(2026, 4, 2));

        Assert.Equal("demo.professor@coachtraining.local", profile.ProfessorEmail);
        Assert.Equal(6, profile.Cenarios.Count);
        Assert.Equal(6, profile.Cenarios.Select(cenario => cenario.Email).Distinct(StringComparer.OrdinalIgnoreCase).Count());
        Assert.All(profile.Cenarios, cenario =>
        {
            var semanas = cenario.Sessoes
                .Select(sessao => CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(
                    sessao.Data.ToDateTime(TimeOnly.MinValue),
                    System.Globalization.CalendarWeekRule.FirstDay,
                    DayOfWeek.Monday))
                .Distinct()
                .Count();

            Assert.InRange(semanas, 10, 12);
        });
    }

    [Fact]
    public void CreateProfile_DemoV1_DeveConterTaperEAderenciaBaixa()
    {
        var dataReferencia = new DateOnly(2026, 4, 2);
        var profile = DemoScenarioFactory.CreateProfile("demo-v1", dataReferencia);

        var taper = profile.Cenarios.Single(cenario => cenario.Slug == "taper-bem-executado");
        var aderenciaBaixa = profile.Cenarios.Single(cenario => cenario.Slug == "aderencia-baixa");

        Assert.NotNull(taper.ProvaAlvo);
        var diasAteProva = taper.ProvaAlvo!.DataProva.DayNumber - dataReferencia.DayNumber;
        Assert.InRange(diasAteProva, 7, 21);
        
        Assert.Equal(5, aderenciaBaixa.TreinosPlanejadosPorSemana);
        
        var dataLimite = new DateOnly(2026, 3, 27);
        var sessoesPrimeiraReferenciaAderencia = aderenciaBaixa.Sessoes.Count(sessao => sessao.Data >= dataLimite);
        Assert.True(sessoesPrimeiraReferenciaAderencia < 12, "Aderência fraca deve ter menos de 12 sessões nas últimas 2 semanas");
        
        // Todas as sessões devem estar dentro de um range razoável (não muito no futuro)
        Assert.All(profile.Cenarios, cenario => 
            Assert.All(cenario.Sessoes, sessao =>
                Assert.True(sessao.Data <= dataReferencia.AddDays(7), $"Sessão {sessao.Data} deve estar <= {dataReferencia.AddDays(7)}")
            )
        );
    }

    [Fact]
    public void CreateProfile_ProfileDesconhecido_DeveLancarInvalidOperationException()
    {
        var exception = Assert.Throws<InvalidOperationException>(() =>
            DemoScenarioFactory.CreateProfile("nao-existe", new DateOnly(2026, 4, 2)));

        Assert.Contains("nao-existe", exception.Message);
    }
}
