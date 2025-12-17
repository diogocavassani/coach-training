using System;

namespace CoachTraining.Domain.Entities;

public class ProvaAlvo
{
    public Guid Id { get; private set; }
    public DateOnly DataProva { get; private set; }
    public double DistanciaKm { get; private set; }
    public string? Objetivo { get; private set; }

    public ProvaAlvo(DateOnly dataProva, double distanciaKm, string? objetivo = null)
    {
        if (distanciaKm <= 0) throw new ArgumentOutOfRangeException(nameof(distanciaKm));
        Id = Guid.NewGuid();
        DataProva = dataProva;
        DistanciaKm = distanciaKm;
        Objetivo = objetivo?.Trim();
    }
}
