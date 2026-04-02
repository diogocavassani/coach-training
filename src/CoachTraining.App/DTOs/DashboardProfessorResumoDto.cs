using CoachTraining.Domain.Enums;

namespace CoachTraining.App.DTOs;

public class DashboardProfessorResumoDto
{
    public int TotalAtletas { get; set; }
    public int AtletasEmAtencao { get; set; }
    public int AtletasEmRisco { get; set; }
    public int AtletasEmTaper { get; set; }
    public int TreinosRegistradosNaSemana { get; set; }
    public int AtletasComPlanejamentoConfigurado { get; set; }
    public DateTime DataUltimaAtualizacao { get; set; }
    public IList<DashboardProfessorAtletaPrioritarioDto> AtletasPrioritarios { get; set; } = new List<DashboardProfessorAtletaPrioritarioDto>();
    public IList<DashboardProfessorTreinoRecenteDto> TreinosRecentes { get; set; } = new List<DashboardProfessorTreinoRecenteDto>();
}

public class DashboardProfessorAtletaPrioritarioDto
{
    public Guid AtletaId { get; set; }
    public string Nome { get; set; } = string.Empty;
    public StatusDeRisco StatusRisco { get; set; }
    public bool EmJanelaDeTaper { get; set; }
    public DateOnly? ProximaProva { get; set; }
    public int CargaSemanal { get; set; }
    public double? AderenciaPlanejamentoPercentual { get; set; }
}

public class DashboardProfessorTreinoRecenteDto
{
    public Guid AtletaId { get; set; }
    public string NomeAtleta { get; set; } = string.Empty;
    public DateOnly Data { get; set; }
    public TipoDeTreino Tipo { get; set; }
    public int Carga { get; set; }
}
