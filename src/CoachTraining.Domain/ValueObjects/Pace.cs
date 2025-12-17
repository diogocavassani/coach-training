using System;

namespace CoachTraining.Domain.ValueObjects;

public readonly struct Pace
{
    // Minutes per kilometer (e.g. 4.5 = 4m30s/km)
    public double MinPerKm { get; }

    public Pace(double minPerKm)
    {
        if (minPerKm <= 0) throw new ArgumentOutOfRangeException(nameof(minPerKm));
        MinPerKm = minPerKm;
    }

    public override string ToString() => MinPerKm.ToString("0.00");
}
