using CoachTraining.DemoSeed.Contracts;
using CoachTraining.Domain.Enums;

namespace CoachTraining.DemoSeed.Scenarios;

public sealed class DivergenciaCargaRendimentoScenario(DateOnly referencia) : DemoScenarioBuilderBase(referencia)
{
    public override DemoScenarioSeed Build()
    {
        // Semanas 11-3: carga moderada com rendimento bom
        var faseEstavel = BlocoSemanal(
            Enumerable.Range(3, 9),
            (DayOfWeek.Monday, TipoDeTreino.Leve, 30, 5.0, 3),
            (DayOfWeek.Tuesday, TipoDeTreino.Ritmo, 50, 9.0, 6),
            (DayOfWeek.Thursday, TipoDeTreino.Intervalado, 45, 8.0, 7),
            (DayOfWeek.Saturday, TipoDeTreino.Longo, 70, 12.0, 5)
        );

        // Últimas 2 semanas: carga aumenta, rendimento piora
        var divergencia = new List<DemoSessaoSeed>
        {
            Sessao(2, DayOfWeek.Monday, TipoDeTreino.Leve, 30, 5.0, 3),
            Sessao(2, DayOfWeek.Tuesday, TipoDeTreino.Intervalado, 55, 10.0, 8),
            Sessao(2, DayOfWeek.Wednesday, TipoDeTreino.Ritmo, 55, 10.0, 7),
            Sessao(2, DayOfWeek.Thursday, TipoDeTreino.Intervalado, 50, 9.0, 8),
            Sessao(2, DayOfWeek.Saturday, TipoDeTreino.Longo, 90, 15.0, 6),
            Sessao(1, DayOfWeek.Monday, TipoDeTreino.Leve, 30, 5.0, 3),
            Sessao(1, DayOfWeek.Tuesday, TipoDeTreino.Intervalado, 60, 11.0, 9),
            Sessao(1, DayOfWeek.Wednesday, TipoDeTreino.Ritmo, 60, 11.0, 8),
            Sessao(1, DayOfWeek.Friday, TipoDeTreino.Intervalado, 55, 10.0, 9),
            Sessao(1, DayOfWeek.Saturday, TipoDeTreino.Longo, 95, 16.0, 7),
            Sessao(0, DayOfWeek.Monday, TipoDeTreino.Leve, 30, 5.0, 3),
            Sessao(0, DayOfWeek.Tuesday, TipoDeTreino.Intervalado, 60, 11.0, 9),
            Sessao(0, DayOfWeek.Thursday, TipoDeTreino.Ritmo, 50, 9.0, 7)
        };

        var sessoes = Combinar(faseEstavel, divergencia.OrderBy(s => s.Data).ToList());

        return new DemoScenarioSeed(
            "divergencia-carga-rendimento",
            "Divergência Carga x Rendimento",
            "gustavo.nunes@demo.local",
            "Corredor",
            "Travamento muscular recente",
            5,
            "Divergência: carga aumenta mas rendimento piora (pace mais lento) - sinais de fadiga",
            null,
            sessoes
        );
    }
}
