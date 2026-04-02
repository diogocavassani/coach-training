using CoachTraining.DemoSeed.Contracts;
using CoachTraining.Domain.Enums;

namespace CoachTraining.DemoSeed.Scenarios;

public sealed class ConstrucaoSaudavelScenario(DateOnly referencia) : DemoScenarioBuilderBase(referencia)
{
    public override DemoScenarioSeed Build()
    {
        // Semanas 11-7: progressão gradual
        var fase1 = BlocoSemanal(
            Enumerable.Range(7, 5),
            (DayOfWeek.Monday, TipoDeTreino.Leve, 30, 5.0, 3),
            (DayOfWeek.Tuesday, TipoDeTreino.Ritmo, 45, 8.0, 6),
            (DayOfWeek.Thursday, TipoDeTreino.Intervalado, 40, 7.0, 7),
            (DayOfWeek.Saturday, TipoDeTreino.Longo, 70, 12.0, 5)
        );

        // Semanas 6-0: intensificação
        var fase2 = BlocoSemanal(
            Enumerable.Range(0, 7),
            (DayOfWeek.Monday, TipoDeTreino.Leve, 30, 5.0, 3),
            (DayOfWeek.Tuesday, TipoDeTreino.Ritmo, 55, 10.0, 7),
            (DayOfWeek.Wednesday, TipoDeTreino.Intervalado, 45, 8.0, 8),
            (DayOfWeek.Thursday, TipoDeTreino.Leve, 30, 5.0, 3),
            (DayOfWeek.Saturday, TipoDeTreino.Longo, 85, 14.5, 5)
        );

        var sessoes = Combinar(fase1, fase2);

        return new DemoScenarioSeed(
            "construcao-saudavel",
            "Construção Saudável",
            "bruno.lima@demo.local",
            "Corredor",
            null,
            5,
            "Progressão consistente e controlada - evolução coerente",
            null,
            sessoes
        );
    }
}
