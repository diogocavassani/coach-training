namespace CoachTraining.Infra.Persistence.Models;

public class SessaoDeTreinoModel
{
    public Guid Id { get; set; }
    public Guid AtletaId { get; set; }
    public DateOnly Data { get; set; }
    public int Tipo { get; set; }
    public int DuracaoMinutos { get; set; }
    public double DistanciaKm { get; set; }
    public int Rpe { get; set; }
    public AtletaModel Atleta { get; set; } = null!;
}
