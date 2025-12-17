using System;
using CoachTraining.Domain.Enums;
using CoachTraining.Domain.Services;
using CoachTraining.Domain.ValueObjects;
using Xunit;

namespace CoachTraining.Domain.Tests;

public class AvaliadorDeRiscoTests
{
    [Fact]
    public void CalcularAcwr_RetornaValorCorreto()
    {
        var aguda = new CargaTreino(100);
        var cronica = new CargaTreino(100);
        
        var acwr = AvaliadorDeRisco.CalcularAcwr(aguda, cronica);
        
        Assert.Equal(1.0, acwr);
    }

    [Fact]
    public void CalcularAcwr_BaixoCargaAguda()
    {
        var aguda = new CargaTreino(50);
        var cronica = new CargaTreino(100);
        
        var acwr = AvaliadorDeRisco.CalcularAcwr(aguda, cronica);
        
        Assert.Equal(0.5, acwr);
    }

    [Fact]
    public void CalcularAcwr_AltoCargaAguda()
    {
        var aguda = new CargaTreino(250);
        var cronica = new CargaTreino(100);
        
        var acwr = AvaliadorDeRisco.CalcularAcwr(aguda, cronica);
        
        Assert.Equal(2.5, acwr);
    }

    [Fact]
    public void CalcularDeltaPercentual_Aumento()
    {
        var atual = new CargaTreino(110);
        var anterior = new CargaTreino(100);
        
        var delta = AvaliadorDeRisco.CalcularDeltaPercentual(atual, anterior);
        
        Assert.Equal(10.0, delta);
    }

    [Fact]
    public void CalcularDeltaPercentual_Diminuicao()
    {
        var atual = new CargaTreino(80);
        var anterior = new CargaTreino(100);
        
        var delta = AvaliadorDeRisco.CalcularDeltaPercentual(atual, anterior);
        
        Assert.Equal(-20.0, delta);
    }

    [Fact]
    public void AvaliarRiscoAcwr_Baixo_RetornaAtencao()
    {
        var risco = AvaliadorDeRisco.AvaliarRiscoAcwr(0.7);
        Assert.Equal(StatusDeRisco.Atencao, risco);
    }

    [Fact]
    public void AvaliarRiscoAcwr_ZoneSegura_RetornaNormal()
    {
        var risco = AvaliadorDeRisco.AvaliarRiscoAcwr(1.0);
        Assert.Equal(StatusDeRisco.Normal, risco);

        risco = AvaliadorDeRisco.AvaliarRiscoAcwr(0.8);
        Assert.Equal(StatusDeRisco.Normal, risco);

        risco = AvaliadorDeRisco.AvaliarRiscoAcwr(1.3);
        Assert.Equal(StatusDeRisco.Normal, risco);
    }

    [Fact]
    public void AvaliarRiscoAcwr_Alto_RetornaRisco()
    {
        var risco = AvaliadorDeRisco.AvaliarRiscoAcwr(1.5);
        Assert.Equal(StatusDeRisco.Risco, risco);

        risco = AvaliadorDeRisco.AvaliarRiscoAcwr(2.0);
        Assert.Equal(StatusDeRisco.Risco, risco);
    }

    [Fact]
    public void AvaliarRiscoProgressao_MenorQue20_RetornaNormal()
    {
        var risco = AvaliadorDeRisco.AvaliarRiscoProgressao(10.0);
        Assert.Equal(StatusDeRisco.Normal, risco);

        risco = AvaliadorDeRisco.AvaliarRiscoProgressao(-10.0);
        Assert.Equal(StatusDeRisco.Normal, risco);
    }

    [Fact]
    public void AvaliarRiscoProgressao_MaiorQue20_RetornaRisco()
    {
        var risco = AvaliadorDeRisco.AvaliarRiscoProgressao(25.0);
        Assert.Equal(StatusDeRisco.Risco, risco);

        risco = AvaliadorDeRisco.AvaliarRiscoProgressao(-25.0);
        Assert.Equal(StatusDeRisco.Risco, risco);
    }

    [Fact]
    public void AvaliarRiscoCombinado_AmbosNormais_RetornaNormal()
    {
        var risco = AvaliadorDeRisco.AvaliarRiscoCombinado(1.0, 10.0);
        Assert.Equal(StatusDeRisco.Normal, risco);
    }

    [Fact]
    public void AvaliarRiscoCombinado_UmRisco_RetornaRisco()
    {
        var risco = AvaliadorDeRisco.AvaliarRiscoCombinado(1.8, 10.0);
        Assert.Equal(StatusDeRisco.Risco, risco);

        risco = AvaliadorDeRisco.AvaliarRiscoCombinado(1.0, 25.0);
        Assert.Equal(StatusDeRisco.Risco, risco);
    }

    [Fact]
    public void AvaliarRiscoCombinado_UmaAtencao_RetornaAtencao()
    {
        var risco = AvaliadorDeRisco.AvaliarRiscoCombinado(0.7, 10.0);
        Assert.Equal(StatusDeRisco.Atencao, risco);
    }
}
