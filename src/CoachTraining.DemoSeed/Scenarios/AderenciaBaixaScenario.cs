using CoachTraining.DemoSeed.Contracts;
using CoachTraining.Domain.Enums;

namespace CoachTraining.DemoSeed.Scenarios;

public sealed class AderenciaBaixaScenario(DateOnly referencia) : DemoScenarioBuilderBase(referencia)
{
    public override DemoScenarioSeed Build()
    {
        // Semanas 11-4: aderência boa
        var aderenciaAlta = BlocoSemanal(
            Enumerable.Range(4, 8),
            (DayOfWeek.Monday, TipoDeTreino.Leve, 30, 5.0, 3),
            (DayOfWeek.Tuesday, TipoDeTreino.Ritmo, 50, 9.0, 6),
            (DayOfWeek.Wednesday, TipoDeTreino.Intervalado, 45, 8.0, 8),
            (DayOfWeek.Thursday, TipoDeTreino.Leve, 30, 5.0, 3),
            (DayOfWeek.Saturday, TipoDeTreino.Longo, 75, 13.0, 5)
        );

        // Semanas 3-0: aderência baixa (apenas 2 treinos/semana em vez de 5)
        var aderenciaFraca = new List<DemoSessaoSeed>
        {
            Sessao(3, DayOfWeek.Tuesday, TipoDeTreino.Ritmo, 45, 8.0, 5),
            Sessao(3, DayOfWeek.Saturday, TipoDeTreino.Longo, 65, 11.0, 4),
            Sessao(2, DayOfWeek.Monday, TipoDeTreino.Leve, 25, 4.0, 2),
            Sessao(2, DayOfWeek.Friday, TipoDeTreino.Ritmo, 40, 7.0, 5),
            Sessao(1, DayOfWeek.Tuesday, TipoDeTreino.Leve, 25, 4.0, 2),
            Sessao(1, DayOfWeek.Saturday, TipoDeTreino.Longo, 60, 10.0, 4),
            Sessao(0, DayOfWeek.Wednesday, TipoDeTreino.Ritmo, 40, 7.0, 5),
            Sessao(0, DayOfWeek.Saturday, TipoDeTreino.Leve, 25, 4.0, 2)
        };

        var sessoes = Combinar(aderenciaAlta, aderenciaFraca.OrderBy(s => s.Data).ToList());

        return new DemoScenarioSeed(
            "aderencia-baixa",
            "Aderência Baixa",
            "fernanda.rocha@demo.local",
            "Corredora",
            "Agenda ocupada, dificuldade de manter consistência",
            5,
            "Planejado 5/semana, executado 2/semana nas últimas semanas - baixa aderência",
            null,
            sessoes
        );
    }
}
