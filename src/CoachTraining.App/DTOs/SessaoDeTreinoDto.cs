using CoachTraining.Domain.Enums;

namespace CoachTraining.App.DTOs;

public class SessaoDeTreinoDto
{
    public Guid Id { get; set; }
    public Guid AtletaId { get; set; }
    public DateOnly Data { get; set; }
    public TipoDeTreino Tipo { get; set; }
    public int DuracaoMinutos { get; set; }
    public double DistanciaKm { get; set; }
    public int Rpe { get; set; }
}
