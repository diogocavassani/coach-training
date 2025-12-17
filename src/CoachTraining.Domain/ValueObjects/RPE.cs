using System;

namespace CoachTraining.Domain.ValueObjects;

public readonly struct RPE
{
    public int Valor { get; }

    public RPE(int valor)
    {
        if (valor < 1 || valor > 10)
            throw new ArgumentOutOfRangeException(nameof(valor), "RPE deve estar entre 1 e 10");
        Valor = valor;
    }

    public override string ToString() => Valor.ToString();
}
