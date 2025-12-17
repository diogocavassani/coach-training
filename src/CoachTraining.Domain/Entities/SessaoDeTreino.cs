using System;
using CoachTraining.Domain.Enums;
using CoachTraining.Domain.ValueObjects;

namespace CoachTraining.Domain.Entities;

public class SessaoDeTreino
{
    public Guid Id { get; private set; }
    public DateOnly Data { get; private set; }
    public TipoDeTreino Tipo { get; private set; }
    public int DuracaoMinutos { get; private set; }
    public double DistanciaKm { get; private set; }
    public RPE Rpe { get; private set; }

    public SessaoDeTreino(DateOnly data, TipoDeTreino tipo, int duracaoMinutos, double distanciaKm, RPE rpe)
    {
        if (duracaoMinutos < 0) throw new ArgumentOutOfRangeException(nameof(duracaoMinutos));
        if (distanciaKm < 0) throw new ArgumentOutOfRangeException(nameof(distanciaKm));

        Id = Guid.NewGuid();
        Data = data;
        Tipo = tipo;
        DuracaoMinutos = duracaoMinutos;
        DistanciaKm = distanciaKm;
        Rpe = rpe;
    }

    public CargaTreino CalcularCarga() => CargaTreino.FromDuracaoAndRpe(DuracaoMinutos, Rpe);
}
