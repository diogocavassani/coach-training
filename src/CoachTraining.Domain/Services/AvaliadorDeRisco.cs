using System;
using CoachTraining.Domain.Enums;
using CoachTraining.Domain.ValueObjects;

namespace CoachTraining.Domain.Services;

public class AvaliadorDeRisco
{
    // ACWR thresholds based on scientific literature
    private const double AcwrBaixo = 0.8;
    private const double AcwrAlto = 1.5;
    private const int DeltaPercentualAlerta = 20;

    /// <summary>
    /// Calcula ACWR (Acute : Chronic Workload Ratio)
    /// </summary>
    public static double CalcularAcwr(CargaTreino cargaAguda, CargaTreino cargaCronica)
    {
        if (cargaCronica.Valor == 0)
            return cargaAguda.Valor > 0 ? double.PositiveInfinity : 0;

        return Math.Round((double)cargaAguda.Valor / cargaCronica.Valor, 2);
    }

    /// <summary>
    /// Calcula delta percentual de carga semanal (mudança de uma semana para a outra)
    /// </summary>
    public static double CalcularDeltaPercentual(CargaTreino semanaSemanal, CargaTreino semanaAnterior)
    {
        if (semanaAnterior.Valor == 0)
            return semanaSemanal.Valor > 0 ? 100 : 0;

        return Math.Round(((double)(semanaSemanal.Valor - semanaAnterior.Valor) / semanaAnterior.Valor) * 100, 1);
    }

    /// <summary>
    /// Avalia o status de risco baseado em ACWR
    /// </summary>
    public static StatusDeRisco AvaliarRiscoAcwr(double acwr)
    {
        if (acwr < AcwrBaixo)
            return StatusDeRisco.Atencao; // possível destreinamento
        
        if (acwr >= AcwrAlto)
            return StatusDeRisco.Risco;  // risco aumentado
        
        return StatusDeRisco.Normal;     // zona segura
    }

    /// <summary>
    /// Avalia risco baseado em progressão de carga semanal
    /// </summary>
    public static StatusDeRisco AvaliarRiscoProgressao(double deltaPercentual)
    {
        if (Math.Abs(deltaPercentual) > DeltaPercentualAlerta)
            return StatusDeRisco.Risco;

        return StatusDeRisco.Normal;
    }

    /// <summary>
    /// Avalia risco combinado (ACWR + Progressão)
    /// </summary>
    public static StatusDeRisco AvaliarRiscoCombinado(double acwr, double deltaPercentual)
    {
        var riscoAcwr = AvaliarRiscoAcwr(acwr);
        var riscoProgressao = AvaliarRiscoProgressao(deltaPercentual);

        // Se qualquer um indicar risco, retorna risco
        if (riscoAcwr == StatusDeRisco.Risco || riscoProgressao == StatusDeRisco.Risco)
            return StatusDeRisco.Risco;

        // Se qualquer um indicar atenção, retorna atenção
        if (riscoAcwr == StatusDeRisco.Atencao || riscoProgressao == StatusDeRisco.Atencao)
            return StatusDeRisco.Atencao;

        return StatusDeRisco.Normal;
    }
}
