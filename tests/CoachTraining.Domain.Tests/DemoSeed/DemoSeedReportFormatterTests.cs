using CoachTraining.DemoSeed.Reports;

namespace CoachTraining.Tests.DemoSeed;

public class DemoSeedReportFormatterTests
{
    [Fact]
    public void Format_DeveImprimirCredenciaisEHeadlinesDosAtletas()
    {
        var report = new DemoSeedReport(
            "demo-v1",
            "demo.professor@coachtraining.local",
            "Demo@123456",
            new[]
            {
                new DemoSeedReportAtleta("Ana Souza", "ana.souza@demo.local", "Sem alerta crítico"),
                new DemoSeedReportAtleta("Bruno Lima", "bruno.lima@demo.local", "Progressão controlada")
            }
        );

        var texto = DemoSeedReportFormatter.Format(report);

        Assert.Contains("demo.professor@coachtraining.local", texto);
        Assert.Contains("Demo@123456", texto);
        Assert.Contains("Ana Souza", texto);
        Assert.Contains("Bruno Lima", texto);
        Assert.Contains("Sem alerta crítico", texto);
        Assert.Contains("Progressão controlada", texto);
        Assert.Contains("✅", texto);
    }

    [Fact]
    public void Format_DeveConterSecoesBemDefinidas()
    {
        var report = new DemoSeedReport(
            "demo-v1",
            "demo.professor@coachtraining.local",
            "Demo@123456",
            new[]
            {
                new DemoSeedReportAtleta("Test Atleta", "test@demo.local", "Test Insight")
            }
        );

        var texto = DemoSeedReportFormatter.Format(report);

        Assert.Contains("PROFESSOR DEMO", texto);
        Assert.Contains("ATLETAS E INSIGHTS", texto);
        Assert.Contains("Email:", texto);
        Assert.Contains("Insight:", texto);
    }
}
