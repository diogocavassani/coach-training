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
    /// Quantidade de treinos planejados por semana para o atleta.
    /// </summary>
    public int? TreinosPlanejadosPorSemana { get; set; }

    /// <summary>
    /// Quantidade de sessoes registradas na janela semanal analisada.
    /// </summary>
    public int TreinosRealizadosNaSemana { get; set; }

    /// <summary>
    /// Percentual de aderencia entre treinos planejados e realizados.
    /// </summary>
    public double? AderenciaPlanejamentoPercentual { get; set; }

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

    /// <summary>
    /// Serie semanal de carga de treino para os ultimos 12 ciclos semanais (segunda a domingo).
    /// </summary>
    public IList<SerieCargaSemanalDto> SerieCargaSemanal { get; set; } = new List<SerieCargaSemanalDto>();

    /// <summary>
    /// Serie semanal de pace medio (min/km) para os ultimos 12 ciclos semanais (segunda a domingo).
    /// Pode conter valor nulo quando nao houver distancia registrada na semana.
    /// </summary>
    public IList<SeriePaceSemanalDto> SeriePaceSemanal { get; set; } = new List<SeriePaceSemanalDto>();

    /// <summary>
    /// Lista de treinos contidos na janela de 12 semanas usada no dashboard.
    /// </summary>
    public IList<TreinoJanelaDto> TreinosJanela { get; set; } = new List<TreinoJanelaDto>();
}

public class SerieCargaSemanalDto
{
    public DateOnly SemanaInicio { get; set; }
    public DateOnly SemanaFim { get; set; }
    public int Valor { get; set; }
}

public class SeriePaceSemanalDto
{
    public DateOnly SemanaInicio { get; set; }
    public DateOnly SemanaFim { get; set; }
    public double? ValorMinPorKm { get; set; }
}

public class TreinoJanelaDto
{
    public Guid Id { get; set; }
    public DateOnly Data { get; set; }
    public TipoDeTreino Tipo { get; set; }
    public int DuracaoMinutos { get; set; }
    public double DistanciaKm { get; set; }
    public int Rpe { get; set; }
    public int Carga { get; set; }
    public double? PaceMinPorKm { get; set; }
}
