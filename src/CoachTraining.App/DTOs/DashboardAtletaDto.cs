using CoachTraining.Domain.Enums;

namespace CoachTraining.App.DTOs;

/// <summary>
/// DTO consolidado com todas as metricas do dashboard do atleta.
/// Apresenta visao completa da saude e progressao do treinamento.
/// </summary>
public class DashboardAtletaDto
{
    /// <summary>
    /// Identificador unico do atleta.
    /// </summary>
    public Guid AtletaId { get; set; }

    /// <summary>
    /// Nome do atleta.
    /// </summary>
    public string Nome { get; set; } = string.Empty;

    /// <summary>
    /// Carga da ultima sessao registrada (em unidades de session-RPE).
    /// </summary>
    public int CargaUltimaSessao { get; set; }

    /// <summary>
    /// Carga total agregada na semana atual.
    /// </summary>
    public int CargaSemanal { get; set; }

    /// <summary>
    /// Carga total da semana anterior (para comparacao).
    /// </summary>
    public int CargaSemanalAnterior { get; set; }

    /// <summary>
    /// Carga aguda (ultima semana completa).
    /// </summary>
    public int CargaAguda { get; set; }

    /// <summary>
    /// Carga cronica (media das ultimas 4 semanas).
    /// </summary>
    public int CargaCronica { get; set; }

    /// <summary>
    /// Razao entre carga aguda e cronica (ACWR).
    /// Valores: &lt;0.8 (risco de destreinamento), 0.8-1.3 (seguro), &gt;=1.5 (risco de sobretreinamento).
    /// </summary>
    public double ACWR { get; set; }

    /// <summary>
    /// Mudanca percentual de carga em relacao a semana anterior.
    /// Valores &gt;20% indicam progressao abrupta.
    /// </summary>
    public double DeltaPercentualSemanal { get; set; }

    /// <summary>
    /// Fase atual do ciclo de treinamento.
    /// </summary>
    public FaseDoCiclo FaseAtual { get; set; }

    /// <summary>
    /// Status de risco consolidado (Normal, Atencao, Risco).
    /// Considera ACWR, progressao e proximidade de prova.
    /// </summary>
    public StatusDeRisco StatusRisco { get; set; }

    /// <summary>
    /// Indicador se o atleta esta em janela de taper (7-21 dias antes da prova).
    /// </summary>
    public bool EmJanelaDeTaper { get; set; }

    /// <summary>
    /// Data da proxima prova agendada (se houver).
    /// </summary>
    public DateOnly? ProximaProva { get; set; }

    /// <summary>
    /// Reducao de volume observada no taper (percentual entre 0 e 1).
    /// Esperado: 40-60%.
    /// </summary>
    public double? ReducaoVolumeTaper { get; set; }

    /// <summary>
    /// Data da ultima atualizacao das metricas.
    /// </summary>
    public DateTime DataUltimaAtualizacao { get; set; }

    /// <summary>
    /// Observacoes clinicas do atleta.
    /// </summary>
    public string? ObservacoesClin { get; set; }

    /// <summary>
    /// Nivel/categoria do atleta.
    /// </summary>
    public string? NivelAtleta { get; set; }

    /// <summary>
    /// Lista de insights gerados para o treinador, ordenados por criticidade.
    /// Mensagens human-readable que explicam o que foi detectado e acoes sugeridas.
    /// </summary>
    public IList<string> Insights { get; set; } = new List<string>();
}
