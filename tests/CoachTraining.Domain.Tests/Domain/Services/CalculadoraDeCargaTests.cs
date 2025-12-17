using System;
using System.Collections.Generic;
using CoachTraining.Domain.Entities;
using CoachTraining.Domain.Enums;
using CoachTraining.Domain.Services;
using CoachTraining.Domain.ValueObjects;
using Xunit;

namespace CoachTraining.Tests.Domain.Services;

public class CalculadoraDeCargaTests
{
    [Fact]
    public void CalcularCargaSessao_ReturnsExpected()
    {
        var sessao = new SessaoDeTreino(DateOnly.FromDateTime(new DateTime(2025,12,01)), TipoDeTreino.Longo, 60, 12.0, new RPE(7));
        var carga = CalculadoraDeCarga.CalcularCargaSessao(sessao);
        Assert.Equal(420, carga.Valor);
    }

    [Fact]
    public void AgregarCargaSemanal_CalculaTotaisCorretamente()
    {
        var sessoes = new List<SessaoDeTreino>
        {
            new SessaoDeTreino(DateOnly.FromDateTime(new DateTime(2025,11,24)), TipoDeTreino.Leve, 30, 5, new RPE(5)), // semana X
            new SessaoDeTreino(DateOnly.FromDateTime(new DateTime(2025,11,25)), TipoDeTreino.Intervalado, 45, 8, new RPE(6)), // same week
            new SessaoDeTreino(DateOnly.FromDateTime(new DateTime(2025,12,01)), TipoDeTreino.Longo, 60, 12, new RPE(7)) // next week
        };

        var semanal = CalculadoraDeCarga.AgregarCargaSemanal(sessoes);

        Assert.True(semanal.Count >= 2);
        var calendar = System.Globalization.CultureInfo.InvariantCulture.Calendar;
        var w1 = calendar.GetWeekOfYear(new DateTime(2025,11,24), System.Globalization.CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
        var w2 = calendar.GetWeekOfYear(new DateTime(2025,12,01), System.Globalization.CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);

        Assert.Equal(30*5 + 45*6, semanal[(2025,w1)].Valor);
        Assert.Equal(60*7, semanal[(2025,w2)].Valor);
    }

    [Fact]
    public void CalcularCargaAgudaECronica_CalculatesCorrectly()
    {
        var sessoes = new List<SessaoDeTreino>();

        var baseDate = new DateTime(2025,12,14);

        void AddSessionFor(DateTime dt, int total)
        {
            var sess = new SessaoDeTreino(DateOnly.FromDateTime(dt), TipoDeTreino.Longo, 1, 0.1, new RPE(Math.Max(1, Math.Min(10, total))));
            if (total <= 10)
            {
                sessoes.Add(sess);
            }
            else
            {
                int remaining = total;
                while (remaining > 0)
                {
                    var r = Math.Min(10, remaining);
                    sessoes.Add(new SessaoDeTreino(DateOnly.FromDateTime(dt), TipoDeTreino.Longo, 1, 0.1, new RPE(r)));
                    remaining -= r;
                }
            }
        }

        AddSessionFor(baseDate.AddDays(-21), 100);
        AddSessionFor(baseDate.AddDays(-14), 200);
        AddSessionFor(baseDate.AddDays(-7), 300);
        AddSessionFor(baseDate, 400);

        var (aguda, cronica) = CalculadoraDeCarga.CalcularCargaAgudaECronica(sessoes, DateOnly.FromDateTime(baseDate));

        Assert.Equal(400, aguda.Valor);
        Assert.Equal((int)Math.Round((100+200+300+400)/4.0), cronica.Valor);
    }
}
