using System.Globalization;
using CoachTraining.App.DTOs;

namespace CoachTraining.App.Services;

public static class GeradorDeInsights
{
    public static IList<string> GerarInsights(DashboardAtletaDto dto)
    {
        var insights = new List<(int priority, string msg)>();

        // 1. Risco por ACWR
        var acwrDisponivel = !double.IsNaN(dto.ACWR) && !double.IsInfinity(dto.ACWR) && dto.CargaCronica > 0;
        if (acwrDisponivel)
        {
            if (dto.ACWR >= 1.5)
            {
                insights.Add((1, $"Risco elevado: ACWR alto ({FormatNumber(dto.ACWR)}). Reduzir volume e priorizar recuperacao."));
            }
            else if (dto.ACWR < 0.8)
            {
                insights.Add((2, $"Atencao: ACWR baixo ({FormatNumber(dto.ACWR)}), risco de destreinamento. Reforcar estimulos."));
            }
        }
        else if (dto.CargaAguda > 0)
        {
            insights.Add((4, "ACWR indisponivel: carga cronica insuficiente para comparacao."));
        }

        // 2. Delta percentual semanal
        if (dto.DeltaPercentualSemanal > 20)
        {
            insights.Add((1, $"Aumento de carga >20% na semana ({FormatPercent(dto.DeltaPercentualSemanal)}): risco de sobrecarga. Rever progressao."));
        }
        else if (dto.DeltaPercentualSemanal < -30)
        {
            insights.Add((3, $"Reducao de carga acentuada ({FormatPercent(dto.DeltaPercentualSemanal)}): possivel polimento ou queda de forma."));
        }

        if (dto.CargaSemanalAnterior == 0 && dto.CargaSemanal > 0)
        {
            insights.Add((4, "Semana anterior sem carga registrada; comparar progressao com cautela."));
        }

        // 3. Taper
        if (dto.EmJanelaDeTaper)
        {
            if (dto.ReducaoVolumeTaper.HasValue)
            {
                var r = dto.ReducaoVolumeTaper.Value;
                var percent = FormatPercent(r * 100);
                if (r < 0.4)
                    insights.Add((1, $"Taper: reducao de volume insuficiente ({percent}). Esperado 40-60%."));
                else if (r <= 0.6)
                    insights.Add((3, $"Taper: reducao de volume adequada ({percent}). Polimento possivelmente adequado."));
                else
                    insights.Add((2, $"Taper: reducao de volume elevada ({percent}). Monitorar fadiga e percepcao do atleta."));
            }
            else
            {
                insights.Add((2, "Taper: atleta em janela de prova, mas sem dados suficientes para avaliar reducao de volume."));
            }
        }

        // 4. Observacoes clinicas
        if (!string.IsNullOrWhiteSpace(dto.ObservacoesClin))
        {
            insights.Add((1, $"Observacoes clinicas: {dto.ObservacoesClin}. Ajustar plano conforme restricoes."));
        }

        // 5. Informacao geral
        if (insights.Count == 0)
            insights.Add((5, "Nenhum alerta critico detectado. Treinamento dentro de parametros esperados."));

        return insights
            .OrderBy(i => i.priority)
            .ThenBy(i => i.msg)
            .Select(i => i.msg)
            .ToList();
    }

    private static string FormatNumber(double value)
        => value.ToString("0.##", CultureInfo.InvariantCulture);

    private static string FormatPercent(double value)
        => value.ToString("0.#", CultureInfo.InvariantCulture) + "%";
}
