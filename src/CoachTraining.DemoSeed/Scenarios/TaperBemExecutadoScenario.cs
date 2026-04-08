using CoachTraining.DemoSeed.Contracts;
using CoachTraining.Domain.Enums;

namespace CoachTraining.DemoSeed.Scenarios;

public sealed class TaperBemExecutadoScenario(DateOnly referencia) : DemoScenarioBuilderBase(referencia)
{
    public override DemoScenarioSeed Build()
    {
        // Semanas 11-4: construção de base
        var construcao = BlocoSemanal(
            Enumerable.Range(4, 8),
            (DayOfWeek.Monday, TipoDeTreino.Leve, 30, 5.0, 3),
            (DayOfWeek.Tuesday, TipoDeTreino.Ritmo, 50, 9.0, 6),
            (DayOfWeek.Wednesday, TipoDeTreino.Intervalado, 45, 8.0, 8),
            (DayOfWeek.Thursday, TipoDeTreino.Leve, 30, 5.0, 3),
            (DayOfWeek.Saturday, TipoDeTreino.Longo, 80, 13.5, 5)
        );

        // Semanas 3-1: taper reduzindo volume
        var taper = BlocoSemanal(
            Enumerable.Range(1, 3),
            (DayOfWeek.Monday, TipoDeTreino.Leve, 20, 3.5, 2),
            (DayOfWeek.Tuesday, TipoDeTreino.Ritmo, 30, 5.0, 6),
            (DayOfWeek.Thursday, TipoDeTreino.Leve, 20, 3.5, 2),
            (DayOfWeek.Saturday, TipoDeTreino.Longo, 40, 7.0, 4)
        );

        var sessoes = Combinar(construcao, taper);

        // Prova é em 10 dias a partir da referência
        var dataProva = Referencia.AddDays(10);

        return new DemoScenarioSeed(
            "taper-bem-executado",
            "Taper Bem Executado",
            "diego.alves@demo.local",
            "Corredor",
            null,
            5,
            "Taper adequado - redução de volume 40-50% pré-prova",
            new DemoProvaAlvoSeed(dataProva, 21.1, "Meia Maratona"),
            sessoes
        );
    }
}
