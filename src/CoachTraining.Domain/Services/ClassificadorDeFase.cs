using System;
using System.Collections.Generic;
using System.Linq;
using CoachTraining.Domain.Entities;
using CoachTraining.Domain.Enums;
using CoachTraining.Domain.ValueObjects;

namespace CoachTraining.Domain.Services;

public class ClassificadorDeFase
{
    // Taper window: typically 7-21 days before event
    private const int TaperMinDays = 7;
    private const int TaperMaxDays = 21;
    
    // Expected volume reduction during taper: 40-60%
    private const double TaperReductionMin = 0.4;
    private const double TaperReductionMax = 0.6;

    /// <summary>
    /// Classifica a fase do treinamento baseado na tendência de carga
    /// Base: carga estável ou abaixo da média
    /// Construção: carga crescente controlada
    /// Pico: carga elevada e sustentada
    /// Polimento: redução de volume (taper)
    /// </summary>
    public static FaseDoCiclo ClassificarFase(IEnumerable<CargaTreino> cargas, DateOnly referencia, ProvaAlvo? prova = null)
    {
        var cargasList = cargas.ToList();
        if (cargasList.Count < 2)
            return FaseDoCiclo.Base; // default if insufficient data

        // Average of last 4 weeks
        var mediaCarga = cargasList.TakeLast(Math.Min(4, cargasList.Count)).Average(c => c.Valor);
        var ultimaCarga = cargasList.Last().Valor;

        // If there's a target event, check if we're in taper window
        if (prova != null && IsInTaperWindow(referencia, prova.DataProva))
        {
            return FaseDoCiclo.Polimento;
        }

        // Classify based on trend
        if (ultimaCarga > mediaCarga * 1.15) // sustained high load
            return FaseDoCiclo.Pico;

        if (ultimaCarga < mediaCarga * 0.85) // reduced load
            return FaseDoCiclo.Base;

        // Check if trending upward (construção)
        if (cargasList.Count >= 3)
        {
            var trend = CalcularTendencia(cargasList.TakeLast(4).Select(c => c.Valor).ToList());
            if (trend > 0.10) // upward trend > 10%
                return FaseDoCiclo.Construcao;
        }

        return FaseDoCiclo.Base;
    }

    /// <summary>
    /// Verifica se a data está na janela de taper (7-21 dias antes da prova)
    /// </summary>
    public static bool IsInTaperWindow(DateOnly referencia, DateOnly dataProva)
    {
        var daysUntilEvent = (dataProva.ToDateTime(TimeOnly.MinValue) - referencia.ToDateTime(TimeOnly.MinValue)).Days;
        return daysUntilEvent >= TaperMinDays && daysUntilEvent <= TaperMaxDays;
    }

    /// <summary>
    /// Calcula a redução de volume esperada no taper
    /// Compara carga 3 semanas antes vs semana anterior à prova
    /// </summary>
    public static double CalcularReducaoVolumeTaper(CargaTreino cargaAntesDeTaper, CargaTreino cargaDuranteTaper)
    {
        if (cargaAntesDeTaper.Valor == 0)
            return 0;

        var reducao = 1.0 - ((double)cargaDuranteTaper.Valor / cargaAntesDeTaper.Valor);
        return Math.Round(reducao, 2);
    }

    /// <summary>
    /// Valida se o taper está sendo executado corretamente
    /// Esperado: redução de 40-60% de volume
    /// </summary>
    public static bool ValidarTaper(double reducaoPercentual)
    {
        return reducaoPercentual >= TaperReductionMin && reducaoPercentual <= TaperReductionMax;
    }

    /// <summary>
    /// Detecta alertas de taper inadequado
    /// Retorna: Normal (execução correta), Atencao (redução fora de intervalo), Risco (sem redução)
    /// </summary>
    public static StatusDeRisco AvaliarTaper(double reducaoPercentual, bool emJanelaDeYaper)
    {
        if (!emJanelaDeYaper)
            return StatusDeRisco.Normal; // fora da janela, não há expectativa

        if (reducaoPercentual < 0.1) // quase nenhuma redução
            return StatusDeRisco.Risco;

        if (!ValidarTaper(reducaoPercentual))
            return StatusDeRisco.Atencao; // redução inadequada (muito ou pouco)

        return StatusDeRisco.Normal;
    }

    /// <summary>
    /// Calcula tendência linear simples (regressão)
    /// Retorna coeficiente: > 0 = crescente, < 0 = decrescente, ~0 = estável
    /// </summary>
    private static double CalcularTendencia(List<int> valores)
    {
        if (valores.Count < 2)
            return 0;

        var n = valores.Count;
        var x = Enumerable.Range(0, n).Select(i => (double)i).ToList();
        var y = valores.Select(v => (double)v).ToList();

        var xMean = x.Average();
        var yMean = y.Average();

        var numerador = x.Zip(y, (xi, yi) => (xi - xMean) * (yi - yMean)).Sum();
        var denominador = x.Select(xi => Math.Pow(xi - xMean, 2)).Sum();

        if (Math.Abs(denominador) < 0.001)
            return 0;

        return numerador / denominador;
    }
}
