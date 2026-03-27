namespace CoachTraining.Infra.Persistence.Models;

public class AtletaModel
{
    public Guid Id { get; set; }
    public Guid ProfessorId { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? ObservacoesClinicas { get; set; }
    public string? NivelEsportivo { get; set; }
    public ProfessorModel Professor { get; set; } = null!;
    public ICollection<SessaoDeTreinoModel> SessoesDeTreino { get; set; } = new List<SessaoDeTreinoModel>();
    public ProvaAlvoModel? ProvaAlvo { get; set; }
}
