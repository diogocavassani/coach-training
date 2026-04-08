using System.Text;

namespace CoachTraining.DemoSeed.Reports;

public static class DemoSeedReportFormatter
{
    public static string Format(DemoSeedReport report)
    {
        var builder = new StringBuilder();

        builder.AppendLine("╔══════════════════════════════════════════════════════════════════════════════╗");
        builder.AppendLine("║                    DEMO DATASET SEED - RELATÓRIO FINAL                        ║");
        builder.AppendLine("╚══════════════════════════════════════════════════════════════════════════════╝");
        builder.AppendLine();

        builder.AppendLine($"📋 Perfil: {report.Profile}");
        builder.AppendLine();

        builder.AppendLine("👨‍🎓 PROFESSOR DEMO");
        builder.AppendLine($"   Email: {report.ProfessorEmail}");
        builder.AppendLine($"   Senha: {report.ProfessorSenha}");
        builder.AppendLine();

        builder.AppendLine("🏃 ATLETAS E INSIGHTS");
        builder.AppendLine();

        foreach (var (atleta, index) in report.Atletas.Select((a, i) => (a, i + 1)))
        {
            builder.AppendLine($"{index}. {atleta.Nome}");
            builder.AppendLine($"   Email: {atleta.Email}");
            builder.AppendLine($"   Insight: {atleta.InsightEsperado}");
            builder.AppendLine();
        }

        builder.AppendLine("✅ Dataset criado com sucesso!");
        builder.AppendLine("   Execute a API e acesse: http://localhost:4200");
        builder.AppendLine($"   Autentique como: {report.ProfessorEmail}");

        return builder.ToString().TrimEnd();
    }
}
