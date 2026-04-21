namespace CoachTraining.Infra.Persistence.Models;

public class LinkPublicoIntegracaoModel
{
    public Guid Id { get; set; }
    public Guid AtletaId { get; set; }
    public string TokenHash { get; set; } = string.Empty;
    public string TokenProtegido { get; set; } = string.Empty;
    public bool Ativo { get; set; }
    public DateTime CriadoEmUtc { get; set; }
    public DateTime? RegeneradoEmUtc { get; set; }
    public DateTime? RevogadoEmUtc { get; set; }
    public AtletaModel Atleta { get; set; } = null!;
}
