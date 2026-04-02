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

        // 3. Monotonia de carga
        var monotoniaSemanal = CalcularMonotoniaSemanal(dto);
        if (monotoniaSemanal is >= 2.0)
        {
            insights.Add((
                2,
                $"Monotonia elevada da carga na semana ({FormatNumber(monotoniaSemanal.Value)}). Distribuicao concentrada, revisar variacao de estimulo e recuperacao."));
        }

        // 4. Divergencia entre carga e rendimento
        if (TryCalcularDivergenciaCargaERendimento(dto, out var deltaCargaPercentual, out var deltaPacePercentual)
            && deltaCargaPercentual > 10
            && deltaPacePercentual > 3)
        {
            insights.Add((
                2,
                $"Divergencia carga x rendimento: carga recente subiu {FormatPercent(deltaCargaPercentual)}, mas o pace medio piorou {FormatPercent(deltaPacePercentual)}. Investigar fadiga e ajustar recuperacao."));
        }

        // 5. Aderencia ao planejamento
        if (dto.TreinosPlanejadosPorSemana.HasValue && dto.AderenciaPlanejamentoPercentual.HasValue)
        {
            var treinosPlanejados = dto.TreinosPlanejadosPorSemana.Value;
            var aderencia = dto.AderenciaPlanejamentoPercentual.Value;

            if (aderencia < 80)
            {
                insights.Add((
                    2,
                    $"Aderencia ao planejamento abaixo do esperado ({dto.TreinosRealizadosNaSemana}/{treinosPlanejados} treinos, {FormatPercent(aderencia)}). Revisar barreiras de execucao."));
            }
            else if (aderencia > 120)
            {
                insights.Add((
                    3,
                    $"Volume realizado acima do planejamento ({dto.TreinosRealizadosNaSemana}/{treinosPlanejados} treinos, {FormatPercent(aderencia)}). Confirmar se o plano semanal precisa de ajuste."));
            }
        }

        // 6. Taper
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

        // 7. Observacoes clinicas
        if (!string.IsNullOrWhiteSpace(dto.ObservacoesClin))
        {
            insights.Add((1, $"Observacoes clinicas: {dto.ObservacoesClin}. Ajustar plano conforme restricoes."));
        }

        // 8. Informacao geral
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

    private static double? CalcularMonotoniaSemanal(DashboardAtletaDto dto)
    {
        if (dto.TreinosJanela.Count == 0)
        {
            return null;
        }

        var referencia = DateOnly.FromDateTime(dto.DataUltimaAtualizacao == default ? DateTime.UtcNow : dto.DataUltimaAtualizacao);
        var inicio = referencia.AddDays(-6);
        var cargaPorDia = Enumerable.Range(0, 7)
            .Select(deslocamento => inicio.AddDays(deslocamento))
            .ToDictionary(data => data, _ => 0d);

        foreach (var treino in dto.TreinosJanela.Where(treino => treino.Data >= inicio && treino.Data <= referencia))
        {
            cargaPorDia[treino.Data] += treino.Carga;
        }

        var cargas = cargaPorDia.Values.ToList();
        var media = cargas.Average();
        if (media == 0)
        {
            return null;
        }

        var variancia = cargas.Average(carga => Math.Pow(carga - media, 2));
        var desvioPadrao = Math.Sqrt(variancia);
        if (desvioPadrao == 0)
        {
            return 999;
        }

        return Math.Round(media / desvioPadrao, 2);
    }

    private static bool TryCalcularDivergenciaCargaERendimento(
        DashboardAtletaDto dto,
        out double deltaCargaPercentual,
        out double deltaPacePercentual)
    {
        deltaCargaPercentual = 0;
        deltaPacePercentual = 0;

        var semanasComPace = dto.SerieCargaSemanal
            .Join(
                dto.SeriePaceSemanal.Where(serie => serie.ValorMinPorKm.HasValue),
                carga => carga.SemanaInicio,
                pace => pace.SemanaInicio,
                (carga, pace) => new
                {
                    carga.SemanaInicio,
                    Carga = carga.Valor,
                    Pace = pace.ValorMinPorKm!.Value
                })
            .OrderBy(semana => semana.SemanaInicio)
            .ToList();

        if (semanasComPace.Count < 4)
        {
            return false;
        }

        var baseComparacao = semanasComPace.TakeLast(4).ToList();
        var semanasBase = baseComparacao.Take(2).ToList();
        var semanasRecentes = baseComparacao.Skip(2).ToList();

        var cargaBase = semanasBase.Average(semana => semana.Carga);
        var cargaRecente = semanasRecentes.Average(semana => semana.Carga);
        var paceBase = semanasBase.Average(semana => semana.Pace);
        var paceRecente = semanasRecentes.Average(semana => semana.Pace);

        if (cargaBase <= 0 || paceBase <= 0)
        {
            return false;
        }

        deltaCargaPercentual = Math.Round((cargaRecente - cargaBase) / cargaBase * 100, 1, MidpointRounding.AwayFromZero);
        deltaPacePercentual = Math.Round((paceRecente - paceBase) / paceBase * 100, 1, MidpointRounding.AwayFromZero);
        return true;
    }
}
