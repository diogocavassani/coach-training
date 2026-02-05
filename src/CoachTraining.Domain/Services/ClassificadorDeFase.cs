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

    private const double LimiarCargaRelativaPico = 1.15;
    private const double LimiarCargaAbsolutaElevada = 1000;
    private const double LimiarReducaoCargaRelativa = 0.85;
    private const double LimiarTendenciaCrescente = 0.10;
    private const int MinimoSessoesParaTendencia = 3;

    /// <summary>
    /// Classifica a fase do treinamento baseado na tendência de carga
    /// Base: carga estável ou abaixo da média
    /// Construção: carga crescente controlada
    /// Pico: carga elevada e sustentada
    /// Polimento: redução de volume (taper)
    /// </summary>
    public static FaseDoCiclo ClassificarFase(IEnumerable<CargaTreino> cargas, DateOnly referencia, ProvaAlvo? prova = null)
    {
        // Sem dados suficientes para inferir tendência fisiológica,
        // assume fase Base por segurança (regra conservadora)
        var cargasList = cargas.ToList();
        if (cargasList.Count < 2)
            return FaseDoCiclo.Base; // default if insufficient data

        // Média das cargas recentes (janela móvel, até 4 períodos disponíveis)
        // Usada como referência relativa para comparação da carga atual
        var mediaCarga = cargasList
            .TakeLast(Math.Min(4, cargasList.Count))
            .Average(c => c.Valor);
        var ultimaCarga = cargasList.Last().Valor;

        // Se houver prova alvo e estivermos na janela pré-prova,
        // assume fase de Polimento (taper), independentemente da tendência
        if (prova != null && IsInTaperWindow(referencia, prova.DataProva))
        {
            return FaseDoCiclo.Polimento;
        }

        // --- Verifica critérios de Pico antes da verificação de tendência ---
        // Carga elevada sustentada: média das últimas sessões acima da média recente
        // evita classificar Pico por um único pico isolado
        var mediaUltimasDuas = cargasList.TakeLast(2).Average(c => c.Valor);
        if (mediaUltimasDuas > mediaCarga * LimiarCargaRelativaPico)
            return FaseDoCiclo.Pico;

        // Pico por carga elevada sustentada (mesmo sem última sessão extrema)
        if (cargasList.Count >= 3 && ultimaCarga > LimiarCargaAbsolutaElevada)
            return FaseDoCiclo.Pico;

        // Verifica tendência de progressão da carga ao longo dos períodos recentes
        // Construção exige crescimento consistente, não apenas sessões isoladas
        if (cargasList.Count >= MinimoSessoesParaTendencia)
        {
            // Tendência positiva acima do limiar indica progressão controlada
            var trendWindow = cargasList.TakeLast(Math.Min(4, cargasList.Count)).Select(c => c.Valor).ToList();
            var trend = CalcularTendencia(trendWindow);
            if (trend > LimiarTendenciaCrescente) // upward trend > 10%
                return FaseDoCiclo.Construcao;
        }

        // Carga atual significativamente abaixo da média recente
        // indica redução de carga ou estabilização (Base)
        if (ultimaCarga < mediaCarga * LimiarReducaoCargaRelativa) 
            return FaseDoCiclo.Base;

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
