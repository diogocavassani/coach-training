using System.Globalization;
using CoachTraining.App.DTOs;
using CoachTraining.Domain.Enums;

namespace CoachTraining.App.Services;

public static class GeradorDeInsights
{
    public static IList<string> GerarInsights(DashboardAtletaDto dto)
    {
        var insights = new List<(int priority, string msg)>();

        // 1. Risco por ACWR
        if (dto.ACWR >= 1.5 || dto.StatusRisco == StatusDeRisco.Risco)
        {
            insights.Add((1, $"Risco aumentado: ACWR alto ({dto.ACWR.ToString("0.##", CultureInfo.InvariantCulture)}). Considere reduzir volume e priorizar recuperação."));
        }
        else if (dto.ACWR < 0.8)
        {
            insights.Add((2, $"Atenção: ACWR baixo ({dto.ACWR.ToString("0.##", CultureInfo.InvariantCulture)}), risco de destreinamento. Avaliar estímulos."));
        }

        // 2. Delta percentual semanal
        if (dto.DeltaPercentualSemanal > 20)
        {
            insights.Add((1, $"Aumento de carga >20% na semana ({dto.DeltaPercentualSemanal}%) — risco de sobrecarga. Rever progressão."));
        }
        else if (dto.DeltaPercentualSemanal < -30)
        {
            insights.Add((3, $"Redução de carga acentuada ({dto.DeltaPercentualSemanal}%) — possível polimento ou queda de forma."));
        }

        // 3. Taper
        if (dto.EmJanelaDeTaper)
        {
            if (dto.ReducaoVolumeTaper.HasValue)
            {
                var r = dto.ReducaoVolumeTaper.Value;
                if (r < 0.4)
                    insights.Add((1, $"Taper: redução de volume insuficiente ({Math.Round(r * 100)}%). Esperado 40–60%."));
                else if (r >= 0.4 && r <= 0.6)
                    insights.Add((3, $"Taper: redução de volume adequada ({Math.Round(r * 100)}%). Polimento possivelmente adequado."));
                else
                    insights.Add((2, $"Taper: redução de volume elevada ({Math.Round(r * 100)}%). Monitorar fadiga e percepção do atleta."));
            }
            else
            {
                insights.Add((2, "Taper: atleta em janela de prova mas sem redução de volume detectada."));
            }
        }

        // 4. Observações clínicas
        if (!string.IsNullOrWhiteSpace(dto.ObservacoesClin))
        {
            insights.Add((1, $"Observações clínicas: {dto.ObservacoesClin}. Considere ajustar plano conforme restrições."));
        }

        // 5. Informação geral
        if (insights.Count == 0)
            insights.Add((5, "Nenhum alerta crítico detectado. Treinamento dentro de parâmetros esperados."));

        // Ordenar por prioridade (menor = mais crítico)
        var ordered = insights.OrderBy(i => i.priority).ThenBy(i => i.msg).Select(i => i.msg).ToList();
        return ordered;
    }
}
