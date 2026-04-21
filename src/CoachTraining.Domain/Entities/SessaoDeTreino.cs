using System;
using CoachTraining.Domain.Enums;
using CoachTraining.Domain.ValueObjects;

namespace CoachTraining.Domain.Entities;

public class SessaoDeTreino
{
    public Guid Id { get; private set; }
    public Guid AtletaId { get; private set; }
    public DateOnly Data { get; private set; }
    public TipoDeTreino Tipo { get; private set; }
    public int DuracaoMinutos { get; private set; }
    public double DistanciaKm { get; private set; }
    public RPE Rpe { get; private set; }
    public OrigemTreino Origem { get; private set; }

    public SessaoDeTreino(
        Guid atletaId,
        DateOnly data,
        TipoDeTreino tipo,
        int duracaoMinutos,
        double distanciaKm,
        RPE rpe,
        OrigemTreino origem = OrigemTreino.Manual,
        Guid? id = null)
    {
        if (atletaId == Guid.Empty) throw new ArgumentException("AtletaId obrigatorio", nameof(atletaId));
        if (duracaoMinutos <= 0) throw new ArgumentOutOfRangeException(nameof(duracaoMinutos), "Duracao deve ser maior que zero");
        if (distanciaKm < 0) throw new ArgumentOutOfRangeException(nameof(distanciaKm), "Distancia nao pode ser negativa");
        if (data > DateOnly.FromDateTime(DateTime.UtcNow)) throw new ArgumentException("Data nao pode ser futura", nameof(data));

        Id = id ?? Guid.NewGuid();
        AtletaId = atletaId;
        Data = data;
        Tipo = tipo;
        DuracaoMinutos = duracaoMinutos;
        DistanciaKm = distanciaKm;
        Rpe = rpe;
        Origem = origem;
    }

    public CargaTreino CalcularCarga() => CargaTreino.FromDuracaoAndRpe(DuracaoMinutos, Rpe);
}
