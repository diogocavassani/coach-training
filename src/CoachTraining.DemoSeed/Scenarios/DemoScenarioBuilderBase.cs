using CoachTraining.DemoSeed.Contracts;
using CoachTraining.Domain.Enums;

namespace CoachTraining.DemoSeed.Scenarios;

public abstract class DemoScenarioBuilderBase
{
    protected DemoScenarioBuilderBase(DateOnly referencia)
    {
        Referencia = referencia;
        SegundaAtual = referencia.AddDays(-(((int)referencia.DayOfWeek + 6) % 7));
    }

    protected DateOnly Referencia { get; }
    protected DateOnly SegundaAtual { get; }

    protected DateOnly EmSemana(int semanasAtras, DayOfWeek dia)
    {
        var deslocamento = ((int)dia + 6) % 7;
        var data = SegundaAtual.AddDays(-(7 * semanasAtras) + deslocamento);
        
        // Garantir que não ultrapasse a referência
        if (data > Referencia)
        {
            data = Referencia;
        }

        return data;
    }

    protected DemoSessaoSeed Sessao(int semanasAtras, DayOfWeek dia, TipoDeTreino tipo, int duracao, double distancia, int rpe)
        => new(EmSemana(semanasAtras, dia), tipo, duracao, distancia, rpe);

    protected IReadOnlyList<DemoSessaoSeed> BlocoSemanal(
        IEnumerable<int> semanasAtras,
        params (DayOfWeek Dia, TipoDeTreino Tipo, int Duracao, double Distancia, int Rpe)[] sessoes)
    {
        var resultado = new List<DemoSessaoSeed>();

        foreach (var semana in semanasAtras)
        {
            foreach (var sessao in sessoes)
            {
                resultado.Add(Sessao(semana, sessao.Dia, sessao.Tipo, sessao.Duracao, sessao.Distancia, sessao.Rpe));
            }
        }

        return resultado.OrderBy(sessao => sessao.Data).ToList();
    }

    protected IReadOnlyList<DemoSessaoSeed> Combinar(params IReadOnlyList<DemoSessaoSeed>[] blocos)
        => blocos.SelectMany(bloco => bloco).OrderBy(sessao => sessao.Data).ToList();

    public abstract DemoScenarioSeed Build();
}
