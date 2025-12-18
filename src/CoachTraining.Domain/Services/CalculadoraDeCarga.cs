using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using CoachTraining.Domain.Entities;
using CoachTraining.Domain.ValueObjects;

namespace CoachTraining.Domain.Services;

public static class CalculadoraDeCarga
{
    // Calcula a carga de uma sessão (Duração * RPE)
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

    // Agrega cargas por semana (ano, semana) usando Calendar.GetWeekOfYear
    public static IDictionary<(int Year, int Week), CargaTreino> AgregarCargaSemanal(IEnumerable<SessaoDeTreino> sessoes, CalendarWeekRule weekRule = CalendarWeekRule.FirstFourDayWeek, DayOfWeek firstDayOfWeek = DayOfWeek.Monday)
    {
        var calendar = CultureInfo.InvariantCulture.Calendar;

        return sessoes
            .GroupBy(s => {
                var dt = s.Data.ToDateTime(TimeOnly.MinValue);
                var week = calendar.GetWeekOfYear(dt, weekRule, firstDayOfWeek);
                return (Year: dt.Year, Week: week);
            })
            .ToDictionary(g => g.Key, g => new CargaTreino(g.Sum(s => CalcularCargaSessao(s).Valor)));
    }

    // Calcula carga aguda (última semana) e carga crônica (média das últimas 4 semanas)
    // Retorna (aguda, cronica)
    public static (CargaTreino Aguda, CargaTreino Cronica) CalcularCargaAgudaECronica(IEnumerable<SessaoDeTreino> sessoes, DateOnly referencia)
    {
        var semanal = AgregarCargaSemanal(sessoes);

        var calendar = CultureInfo.InvariantCulture.Calendar;
        var refDt = referencia.ToDateTime(TimeOnly.MinValue);
        var refWeek = calendar.GetWeekOfYear(refDt, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
        var refYear = refDt.Year;

        // carga aguda = soma da semana de referência
        CargaTreino aguda;
        if (!semanal.TryGetValue((refYear, refWeek), out aguda))
            aguda = new CargaTreino(0);

        // obter últimas 4 semanas (incluindo a semana de referência)
        var lastWeeks = new List<CargaTreino>();
        for (int i = 0; i < 4; i++)
        {
            var week = refWeek - i;
            var year = refYear;
            // ajustar ano/semanas quando necessário
            var w = week;
            var y = year;
            while (w <= 0)
            {
                y -= 1;
                var lastYearWeeks = CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(new DateTime(y,12,31), CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
                w += lastYearWeeks;
            }

            if (!semanal.TryGetValue((y, w), out var val))
                val = new CargaTreino(0);
            lastWeeks.Add(val);
        }

        var cronicaValor = lastWeeks.Any(c => c.Valor == 0) ? 0 : (int)Math.Round(lastWeeks.Average(c => c.Valor));
        var cronica = new CargaTreino(cronicaValor);

        return (aguda, cronica);
    }
}
