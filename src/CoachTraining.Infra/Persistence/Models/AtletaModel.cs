namespace CoachTraining.Infra.Persistence.Models;

public class AtletaModel
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? ObservacoesClinicas { get; set; }
    public string? NivelEsportivo { get; set; }
}
