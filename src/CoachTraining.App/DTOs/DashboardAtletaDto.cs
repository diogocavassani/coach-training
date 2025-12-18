using CoachTraining.Domain.Enums;

namespace CoachTraining.App.DTOs;

/// <summary>
/// DTO consolidado com todas as métricas do dashboard do atleta.
/// Apresenta visão completa da saúde e progressão do treinamento.
/// </summary>
public class DashboardAtletaDto
{
    /// <summary>
    /// Identificador único do atleta.
    /// </summary>
    public Guid AtletaId { get; set; }

    /// <summary>
    /// Nome do atleta.
    /// </summary>
    public string Nome { get; set; } = string.Empty;

    /// <summary>
    /// Carga da última sessão registrada (em unidades de session-RPE).
    /// </summary>
    public int CargaUltimaSessao { get; set; }

    /// <summary>
    /// Carga total agregada na semana atual.
    /// </summary>
    public int CargaSemanal { get; set; }

    /// <summary>
    /// Carga aguda (última semana completa).
    /// </summary>
    public int CargaAguda { get; set; }

    /// <summary>
    /// Carga crônica (média das últimas 4 semanas).
    /// </summary>
    public int CargaCronica { get; set; }

    /// <summary>
    /// Razão entre carga aguda e crônica (ACWR).
    /// Valores: &lt;0.8 (risco de destreinamento), 0.8-1.3 (seguro), ≥1.5 (risco de sobretreinamento).
    /// </summary>
    public double ACWR { get; set; }

    /// <summary>
    /// Mudança percentual de carga em relação à semana anterior.
    /// Valores &gt;20% indicam progressão abrupta.
    /// </summary>
    public double DeltaPercentualSemanal { get; set; }

    /// <summary>
    /// Fase atual do ciclo de treinamento.
    /// </summary>
    public FaseDoCiclo FaseAtual { get; set; }

    /// <summary>
    /// Status de risco consolidado (Normal, Atenção, Risco).
    /// Considera ACWR, progressão e proximidade de prova.
    /// </summary>
    public StatusDeRisco StatusRisco { get; set; }

    /// <summary>
    /// Indicador se o atleta está em janela de taper (7-21 dias antes da prova).
    /// </summary>
    public bool EmJanelaDeTaper { get; set; }

    /// <summary>
    /// Data da próxima prova agendada (se houver).
    /// </summary>
    public DateOnly? ProximaProva { get; set; }

    /// <summary>
    /// Redução de volume observada no taper (percentual entre 0 e 1).
    /// Esperado: 40-60%.
    /// </summary>
    public double? ReducaoVolumeTaper { get; set; }

    /// <summary>
    /// Data da última atualização das métricas.
    /// </summary>
    public DateTime DataUltimaAtualizacao { get; set; }

    /// <summary>
    /// Observações clínicas do atleta.
    /// </summary>
    public string? ObservacoesClin { get; set; }

    /// <summary>
    /// Nível/categoria do atleta.
    /// </summary>
    public string? NivelAtleta { get; set; }
}
