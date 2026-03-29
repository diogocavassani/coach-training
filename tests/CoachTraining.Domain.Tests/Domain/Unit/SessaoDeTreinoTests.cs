using CoachTraining.Domain.Entities;
using CoachTraining.Domain.Enums;
using CoachTraining.Domain.ValueObjects;

namespace CoachTraining.Tests.Domain.Unit;

public class SessaoDeTreinoTests
{
    [Fact]
    public void Deve_CriarSessao_QuandoDadosValidos()
    {
        var atletaId = Guid.NewGuid();
        var hoje = DateOnly.FromDateTime(DateTime.UtcNow);

        var sessao = new SessaoDeTreino(atletaId, hoje, TipoDeTreino.Longo, 60, 10.5, new RPE(7));

        Assert.Equal(atletaId, sessao.AtletaId);
        Assert.Equal(60, sessao.DuracaoMinutos);
        Assert.Equal(7, sessao.Rpe.Valor);
    }

    [Fact]
    public void Deve_Falhar_QuandoDuracaoInvalida()
    {
        var atletaId = Guid.NewGuid();
        var hoje = DateOnly.FromDateTime(DateTime.UtcNow);

        Assert.Throws<ArgumentOutOfRangeException>(() => new SessaoDeTreino(atletaId, hoje, TipoDeTreino.Leve, 0, 1.0, new RPE(5)));
    }

    [Fact]
    public void Deve_Falhar_QuandoRpeInvalido()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new RPE(11));
    }

    [Fact]
    public void Deve_Falhar_QuandoAtletaIdVazio()
    {
        var hoje = DateOnly.FromDateTime(DateTime.UtcNow);

        Assert.Throws<ArgumentException>(() => new SessaoDeTreino(Guid.Empty, hoje, TipoDeTreino.Leve, 30, 5, new RPE(5)));
    }
}
