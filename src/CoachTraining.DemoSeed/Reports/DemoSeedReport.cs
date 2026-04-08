namespace CoachTraining.DemoSeed.Reports;

public sealed record DemoSeedReport(
    string Profile,
    string ProfessorEmail,
    string ProfessorSenha,
    IReadOnlyList<DemoSeedReportAtleta> Atletas);

public sealed record DemoSeedReportAtleta(
    string Nome,
    string Email,
    string InsightEsperado);
