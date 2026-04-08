using CoachTraining.Domain.Enums;

namespace CoachTraining.DemoSeed.Contracts;

public sealed record DemoSessaoSeed(
    DateOnly Data,
    TipoDeTreino Tipo,
    int DuracaoMinutos,
    double DistanciaKm,
    int Rpe);
