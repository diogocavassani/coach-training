using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using CoachTraining.Domain.Entities;
using CoachTraining.Domain.ValueObjects;

namespace CoachTraining.Domain.Services;

public static class CalculadoraDeCarga
{
    // Calcula a carga de uma sessao (Duracao * RPE)
    public static CargaTreino CalcularCargaSessao(SessaoDeTreino sessao)
    {
        return CargaTreino.FromDuracaoAndRpe(sessao.DuracaoMinutos, sessao.Rpe);
    }

    // Agrega cargas por dia (Data -> Carga total do dia)
    public static IDictionary<DateOnly, CargaTreino> AgregarCargaDiaria(IEnumerable<SessaoDeTreino> sessoes)
    {
        return sessoes
            .GroupBy(s => s.Data)
            .ToDictionary(g => g.Key, g => new CargaTreino(g.Sum(s => CalcularCargaSessao(s).Valor)));
    }

    // Agrega cargas por semana ISO (ano, semana)
    public static IDictionary<(int Year, int Week), CargaTreino> AgregarCargaSemanal(IEnumerable<SessaoDeTreino> sessoes)
    {
        return sessoes
            .GroupBy(s => ObterAnoSemanaIso(s.Data))
            .ToDictionary(g => g.Key, g => new CargaTreino(g.Sum(s => CalcularCargaSessao(s).Valor)));
    }

    // Calcula carga aguda (ultima semana) e carga cronica (media das ultimas 4 semanas)
    // Retorna (aguda, cronica)
    public static (CargaTreino Aguda, CargaTreino Cronica) CalcularCargaAgudaECronica(IEnumerable<SessaoDeTreino> sessoes, DateOnly referencia)
    {
        var semanal = AgregarCargaSemanal(sessoes);
        return CalcularCargaAgudaECronica(semanal, referencia);
    }

    public static (CargaTreino Aguda, CargaTreino Cronica) CalcularCargaAgudaECronica(
        IDictionary<(int Year, int Week), CargaTreino> cargaSemanal,
        DateOnly referencia)
    {
        var (refYear, refWeek) = ObterAnoSemanaIso(referencia);

        var aguda = cargaSemanal.TryGetValue((refYear, refWeek), out var cargaAguda)
            ? cargaAguda
            : new CargaTreino(0);

        var lastWeeks = new List<CargaTreino>(capacity: 4);
        for (int i = 0; i < 4; i++)
        {
            var (year, week) = ObterAnoSemanaIso(referencia.AddDays(-7 * i));
            if (!cargaSemanal.TryGetValue((year, week), out var val))
                val = new CargaTreino(0);
            lastWeeks.Add(val);
        }

        var cronicaValor = lastWeeks.Any(c => c.Valor == 0) ? 0 : (int)Math.Round(lastWeeks.Average(c => c.Valor));
        var cronica = new CargaTreino(cronicaValor);

        return (aguda, cronica);
    }

    private static (int Year, int Week) ObterAnoSemanaIso(DateOnly data)
    {
        var dt = data.ToDateTime(TimeOnly.MinValue);
        return (ISOWeek.GetYear(dt), ISOWeek.GetWeekOfYear(dt));
    }
}
