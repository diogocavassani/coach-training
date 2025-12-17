using System;

namespace CoachTraining.Domain.ValueObjects;

public readonly struct CargaTreino
{
    public int Valor { get; }

    public CargaTreino(int valor)
    {
        if (valor < 0) throw new ArgumentOutOfRangeException(nameof(valor), "Carga deve ser >= 0");
        Valor = valor;
    }

    public static CargaTreino FromDuracaoAndRpe(int duracaoMinutos, RPE rpe)
    {
        if (duracaoMinutos < 0) throw new ArgumentOutOfRangeException(nameof(duracaoMinutos));
        return new CargaTreino(duracaoMinutos * rpe.Valor);
    }

    public override string ToString() => Valor.ToString();
}
