namespace CoachTraining.Infra.Persistence.Models;

public class ConexaoWearableModel
{
    public Guid Id { get; set; }
    public Guid AtletaId { get; set; }
    public int Provedor { get; set; }
    public int Status { get; set; }
    public string ExternalAthleteId { get; set; } = string.Empty;
    public string ScopesConcedidos { get; set; } = string.Empty;
    public DateTime ConectadoEmUtc { get; set; }
    public DateTime? DesconectadoEmUtc { get; set; }
    public DateTime? UltimaSincronizacaoEmUtc { get; set; }
    public string? UltimoErro { get; set; }
    public AtletaModel Atleta { get; set; } = null!;
    public CredencialWearableModel? Credencial { get; set; }
}
