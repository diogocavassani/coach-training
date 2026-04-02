namespace CoachTraining.App.DTOs;

public class ProvaAlvoDto
{
    public Guid Id { get; set; }
    public Guid AtletaId { get; set; }
    public DateOnly DataProva { get; set; }
    public double DistanciaKm { get; set; }
    public string? Objetivo { get; set; }
}
