namespace CoachTraining.DemoSeed.Contracts;

public sealed record DemoScenarioSeed(
    string Slug,
    string Nome,
    string Email,
    string NivelEsportivo,
    string? ObservacoesClinicas,
    int TreinosPlanejadosPorSemana,
    string InsightEsperado,
    DemoProvaAlvoSeed? ProvaAlvo,
    IReadOnlyList<DemoSessaoSeed> Sessoes);
