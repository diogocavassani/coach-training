using System;
using System.Collections.Generic;
using CoachTraining.Domain.Entities;
using CoachTraining.Domain.Enums;
using CoachTraining.Domain.Services;
using CoachTraining.Domain.ValueObjects;
using Xunit;

namespace CoachTraining.Tests.Domain.Services;

public class ClassificadorDeFaseTests
{
    [Fact]
    public void ClassificarFase_CargaEstavel_RetornaBase()
    {
        var cargas = new List<CargaTreino>
        {
            new CargaTreino(100),
            new CargaTreino(100),
            new CargaTreino(100),
            new CargaTreino(100),
        };

        var fase = ClassificadorDeFase.ClassificarFase(cargas, DateOnly.FromDateTime(DateTime.UtcNow));

        Assert.Equal(FaseDoCiclo.Base, fase);
    }

    [Fact]
    public void ClassificarFase_CargaElevada_RetornaPico()
    {
        var cargas = new List<CargaTreino>
        {
            new CargaTreino(100),
            new CargaTreino(150),
            new CargaTreino(200),
            new CargaTreino(250),
        };

        var fase = ClassificadorDeFase.ClassificarFase(cargas, DateOnly.FromDateTime(DateTime.UtcNow));

        Assert.Equal(FaseDoCiclo.Pico, fase);
    }

    [Fact]
    public void IsInTaperWindow_7DiasAntes_RetornaTrue()
    {
        var hoje = DateOnly.FromDateTime(DateTime.UtcNow);
        var prova = hoje.AddDays(7);

        var emTaper = ClassificadorDeFase.IsInTaperWindow(hoje, prova);

        Assert.True(emTaper);
    }

    [Fact]
    public void IsInTaperWindow_21DiasAntes_RetornaTrue()
    {
        var hoje = DateOnly.FromDateTime(DateTime.UtcNow);
        var prova = hoje.AddDays(21);

        var emTaper = ClassificadorDeFase.IsInTaperWindow(hoje, prova);

        Assert.True(emTaper);
    }

    [Fact]
    public void IsInTaperWindow_6DiasAntes_RetornaFalse()
    {
        var hoje = DateOnly.FromDateTime(DateTime.UtcNow);
        var prova = hoje.AddDays(6);

        var emTaper = ClassificadorDeFase.IsInTaperWindow(hoje, prova);

        Assert.False(emTaper);
    }

    [Fact]
    public void IsInTaperWindow_22DiasAntes_RetornaFalse()
    {
        var hoje = DateOnly.FromDateTime(DateTime.UtcNow);
        var prova = hoje.AddDays(22);

        var emTaper = ClassificadorDeFase.IsInTaperWindow(hoje, prova);

        Assert.False(emTaper);
    }

    [Fact]
    public void ClassificarFase_EmTaperWindow_RetornaPolimento()
    {
        var hoje = DateOnly.FromDateTime(DateTime.UtcNow);
        var prova = new ProvaAlvo(hoje.AddDays(10), 42.0, "Maratona teste");
        
        var cargas = new List<CargaTreino>
        {
            new CargaTreino(100),
            new CargaTreino(120),
            new CargaTreino(140),
            new CargaTreino(160),
        };

        var fase = ClassificadorDeFase.ClassificarFase(cargas, hoje, prova);

        Assert.Equal(FaseDoCiclo.Polimento, fase);
    }

    [Fact]
    public void CalcularReducaoVolumeTaper_40Porcento_Calculado()
    {
        var antes = new CargaTreino(1000);
        var durante = new CargaTreino(600);

        var reducao = ClassificadorDeFase.CalcularReducaoVolumeTaper(antes, durante);

        Assert.Equal(0.4, reducao);
    }

    [Fact]
    public void CalcularReducaoVolumeTaper_50Porcento_Calculado()
    {
        var antes = new CargaTreino(1000);
        var durante = new CargaTreino(500);

        var reducao = ClassificadorDeFase.CalcularReducaoVolumeTaper(antes, durante);

        Assert.Equal(0.5, reducao);
    }

    [Fact]
    public void ValidarTaper_40Porcento_RetornaTrue()
    {
        var valido = ClassificadorDeFase.ValidarTaper(0.4);

        Assert.True(valido);
    }

    [Fact]
    public void ValidarTaper_50Porcento_RetornaTrue()
    {
        var valido = ClassificadorDeFase.ValidarTaper(0.5);

        Assert.True(valido);
    }

    [Fact]
    public void ValidarTaper_60Porcento_RetornaTrue()
    {
        var valido = ClassificadorDeFase.ValidarTaper(0.6);

        Assert.True(valido);
    }

    [Fact]
    public void ValidarTaper_30Porcento_RetornaFalse()
    {
        var valido = ClassificadorDeFase.ValidarTaper(0.3);

        Assert.False(valido);
    }

    [Fact]
    public void ValidarTaper_70Porcento_RetornaFalse()
    {
        var valido = ClassificadorDeFase.ValidarTaper(0.7);

        Assert.False(valido);
    }

    [Fact]
    public void AvaliarTaper_TaperCorreto_RetornaNormal()
    {
        var risco = ClassificadorDeFase.AvaliarTaper(0.5, emJanelaDeYaper: true);

        Assert.Equal(StatusDeRisco.Normal, risco);
    }

    [Fact]
    public void AvaliarTaper_SemReducao_RetornaRisco()
    {
        var risco = ClassificadorDeFase.AvaliarTaper(0.0, emJanelaDeYaper: true);

        Assert.Equal(StatusDeRisco.Risco, risco);
    }

    [Fact]
    public void AvaliarTaper_ReducaoInadequada_RetornaAtencao()
    {
        var risco = ClassificadorDeFase.AvaliarTaper(0.2, emJanelaDeYaper: true);

        Assert.Equal(StatusDeRisco.Atencao, risco);
    }

    [Fact]
    public void AvaliarTaper_ForaDaJanela_RetornaNormal()
    {
        var risco = ClassificadorDeFase.AvaliarTaper(0.0, emJanelaDeYaper: false);

        Assert.Equal(StatusDeRisco.Normal, risco);
    }
}
