namespace CoachTraining.Infra.Persistence.Models;

public class ProvaAlvoModel
{
    public Guid Id { get; set; }
    public Guid AtletaId { get; set; }
    public DateOnly DataProva { get; set; }
    public double DistanciaKm { get; set; }
    public string? Objetivo { get; set; }
    public AtletaModel Atleta { get; set; } = null!;
}
