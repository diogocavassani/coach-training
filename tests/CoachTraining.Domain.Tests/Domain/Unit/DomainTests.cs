using System;
using Xunit;
using CoachTraining.Domain.ValueObjects;
using CoachTraining.Domain.Entities;
using CoachTraining.Domain.Enums;

namespace CoachTraining.Tests.Domain.Unit;

public class DomainTests
{
    [Fact]
    public void Rpe_ValidRange_AllowsValue()
    {
        var rpe = new RPE(7);
        Assert.Equal(7, rpe.Valor);
    }

    [Fact]
    public void Rpe_Invalid_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new RPE(0));
        Assert.Throws<ArgumentOutOfRangeException>(() => new RPE(11));
    }

    [Fact]
    public void CargaTreino_FromDuracaoAndRpe_CalculatesCorrectly()
    {
        var rpe = new RPE(8);
        var carga = CargaTreino.FromDuracaoAndRpe(60, rpe);
        Assert.Equal(480, carga.Valor);
    }

    [Fact]
    public void SessaoDeTreino_CalcularCarga_Works()
    {
        var rpe = new RPE(6);
        var sessao = new SessaoDeTreino(Guid.NewGuid(), DateOnly.FromDateTime(DateTime.UtcNow), TipoDeTreino.Longo, 90, 20.0, rpe);
        var carga = sessao.CalcularCarga();
        Assert.Equal(540, carga.Valor);
    }

    [Fact]
    public void Professor_DeveCriarComDadosValidos()
    {
        var professor = new Professor(
            nome: "Professor Teste",
            email: "Professor@Teste.com",
            senhaHash: "hash-seguro");

        Assert.NotEqual(Guid.Empty, professor.Id);
        Assert.Equal("Professor Teste", professor.Nome);
        Assert.Equal("professor@teste.com", professor.Email);
        Assert.Equal("hash-seguro", professor.SenhaHash);
    }

    [Fact]
    public void Professor_ComEmailInvalido_DeveLancarExcecao()
    {
        Assert.Throws<ArgumentException>(
            () => new Professor("Professor", "email-invalido", "hash-seguro"));
    }

    [Fact]
    public void Professor_ComNomeVazio_DeveLancarExcecao()
    {
        Assert.Throws<ArgumentException>(
            () => new Professor(" ", "professor@teste.com", "hash-seguro"));
    }

    [Fact]
    public void Professor_ComSenhaHashVazia_DeveLancarExcecao()
    {
        Assert.Throws<ArgumentException>(
            () => new Professor("Professor", "professor@teste.com", " "));
    }
}
