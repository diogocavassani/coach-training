namespace CoachTraining.App.DTOs;

public class SalvarProvaAlvoDto
{
    public DateOnly DataProva { get; set; }
    public double DistanciaKm { get; set; }
    public string? Objetivo { get; set; }
}
