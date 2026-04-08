using CoachTraining.DemoSeed.Contracts;
using CoachTraining.Domain.Enums;

namespace CoachTraining.DemoSeed.Scenarios;

public sealed class RiscoCargaAbruptaScenario(DateOnly referencia) : DemoScenarioBuilderBase(referencia)
{
    public override DemoScenarioSeed Build()
    {
        // Semanas 11-2: carga moderada
        var faseModurada = BlocoSemanal(
            Enumerable.Range(2, 10),
            (DayOfWeek.Monday, TipoDeTreino.Leve, 30, 5.0, 3),
            (DayOfWeek.Tuesday, TipoDeTreino.Ritmo, 45, 8.0, 5),
            (DayOfWeek.Thursday, TipoDeTreino.Leve, 30, 5.0, 3),
            (DayOfWeek.Saturday, TipoDeTreino.Longo, 65, 11.0, 4)
        );

        // Última semana: aumento abrupto
        var ultimas2Semanas = new List<DemoSessaoSeed>
        {
            Sessao(1, DayOfWeek.Monday, TipoDeTreino.Leve, 30, 5.0, 3),
            Sessao(1, DayOfWeek.Tuesday, TipoDeTreino.Intervalado, 60, 11.0, 9),
            Sessao(1, DayOfWeek.Wednesday, TipoDeTreino.Ritmo, 55, 10.0, 7),
            Sessao(1, DayOfWeek.Thursday, TipoDeTreino.Intervalado, 50, 9.0, 8),
            Sessao(1, DayOfWeek.Saturday, TipoDeTreino.Longo, 90, 15.0, 6),
            Sessao(0, DayOfWeek.Monday, TipoDeTreino.Leve, 30, 5.0, 3),
            Sessao(0, DayOfWeek.Tuesday, TipoDeTreino.Intervalado, 60, 11.0, 9),
            Sessao(0, DayOfWeek.Thursday, TipoDeTreino.Ritmo, 60, 11.0, 7)
        };

        var sessoes = Combinar(faseModurada, ultimas2Semanas.OrderBy(s => s.Data).ToList());

        return new DemoScenarioSeed(
            "risco-carga-abrupta",
            "Risco: Carga Abrupta",
            "carla.mendes@demo.local",
            "Corredora",
            null,
            4,
            "ALERTA: Delta semanal elevado e ACWR alto - sobrecarga detectada",
            null,
            sessoes
        );
    }
}
