using CoachTraining.Domain.Entities;

namespace CoachTraining.Tests.Domain.Unit;

public class LinkPublicoIntegracaoTests
{
    [Fact]
    public void Criar_DeveIniciarLinkComoAtivo()
    {
        var atletaId = Guid.NewGuid();

        var link = LinkPublicoIntegracao.Criar(atletaId, "hash-inicial");

        Assert.Equal(atletaId, link.AtletaId);
        Assert.Equal("hash-inicial", link.TokenHash);
        Assert.True(link.Ativo);
        Assert.Null(link.RegeneradoEmUtc);
        Assert.Null(link.RevogadoEmUtc);
    }

    [Fact]
    public void Regenerar_DeveInvalidarTokenAnteriorEManterLinkAtivo()
    {
        var link = LinkPublicoIntegracao.Criar(Guid.NewGuid(), "hash-antigo");
        var quando = DateTime.UtcNow;

        link.Regenerar("hash-novo", quando);

        Assert.Equal("hash-novo", link.TokenHash);
        Assert.True(link.Ativo);
        Assert.Equal(quando, link.RegeneradoEmUtc);
        Assert.Null(link.RevogadoEmUtc);
    }

    [Fact]
    public void Revogar_DeveMarcarLinkComoInativo()
    {
        var link = LinkPublicoIntegracao.Criar(Guid.NewGuid(), "hash-antigo");
        var quando = DateTime.UtcNow;

        link.Revogar(quando);

        Assert.False(link.Ativo);
        Assert.Equal(quando, link.RevogadoEmUtc);
    }
}
