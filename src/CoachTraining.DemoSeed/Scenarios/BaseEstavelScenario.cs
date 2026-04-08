using CoachTraining.DemoSeed.Contracts;
using CoachTraining.Domain.Enums;

namespace CoachTraining.DemoSeed.Scenarios;

public sealed class BaseEstavelScenario(DateOnly referencia) : DemoScenarioBuilderBase(referencia)
{
    public override DemoScenarioSeed Build()
    {
        var sessoes = BlocoSemanal(
            Enumerable.Range(0, 12),
            (DayOfWeek.Monday, TipoDeTreino.Leve, 30, 5.0, 3),
            (DayOfWeek.Tuesday, TipoDeTreino.Ritmo, 50, 9.0, 6),
            (DayOfWeek.Thursday, TipoDeTreino.Intervalado, 45, 8.0, 8),
            (DayOfWeek.Saturday, TipoDeTreino.Longo, 75, 13.0, 5)
        );

        return new DemoScenarioSeed(
            "base-estavel",
            "Base Estável",
            "ana.souza@demo.local",
            "Corredora",
            null,
            4,
            "Sem alerta crítico - carga controlada e consistente",
            null,
            sessoes
        );
    }
}
