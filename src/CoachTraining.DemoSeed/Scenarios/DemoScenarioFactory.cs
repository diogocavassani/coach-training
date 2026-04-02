using CoachTraining.DemoSeed.Contracts;

namespace CoachTraining.DemoSeed.Scenarios;

public static class DemoScenarioFactory
{
    public static DemoProfileDefinition CreateProfile(string profile, DateOnly referencia)
    {
        if (profile != "demo-v1")
        {
            throw new InvalidOperationException($"Perfil de demo desconhecido: {profile}");
        }

        var cenarios = new List<DemoScenarioSeed>
        {
            new BaseEstavelScenario(referencia).Build(),
            new ConstrucaoSaudavelScenario(referencia).Build(),
            new RiscoCargaAbruptaScenario(referencia).Build(),
            new TaperBemExecutadoScenario(referencia).Build(),
            new AderenciaBaixaScenario(referencia).Build(),
            new DivergenciaCargaRendimentoScenario(referencia).Build()
        };

        return new DemoProfileDefinition(
            "demo-v1",
            "Professor Demo",
            "demo.professor@coachtraining.local",
            "Demo@123456",
            cenarios
        );
    }
}
