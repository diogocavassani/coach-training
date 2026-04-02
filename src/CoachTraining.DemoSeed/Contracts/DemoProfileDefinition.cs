namespace CoachTraining.DemoSeed.Contracts;

public sealed record DemoProfileDefinition(
    string Profile,
    string ProfessorNome,
    string ProfessorEmail,
    string ProfessorSenha,
    IReadOnlyList<DemoScenarioSeed> Cenarios);
