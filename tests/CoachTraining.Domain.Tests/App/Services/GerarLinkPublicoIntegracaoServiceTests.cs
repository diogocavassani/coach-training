using CoachTraining.App.Abstractions.Integrations;
using CoachTraining.App.Abstractions.Persistence;
using CoachTraining.App.Abstractions.Security;
using CoachTraining.App.DTOs.Integrations;
using CoachTraining.App.Services.Integrations;
using CoachTraining.Domain.Entities;

namespace CoachTraining.Tests.App.Services;

public class GerarLinkPublicoIntegracaoServiceTests
{
    private sealed class AtletaRepositoryFake : IAtletaRepository
    {
        private readonly Atleta _atleta;

        public AtletaRepositoryFake(Atleta atleta)
        {
            _atleta = atleta;
        }

        public void Adicionar(Atleta atleta)
        {
        }

        public void AtualizarPlanejamentoBase(Guid atletaId, Guid professorId, int treinosPlanejadosPorSemana)
        {
        }

        public Atleta? ObterPorId(Guid atletaId, Guid professorId)
            => _atleta.Id == atletaId && _atleta.ProfessorId == professorId ? _atleta : null;

        public IReadOnlyList<Atleta> ListarPorProfessor(Guid professorId)
            => _atleta.ProfessorId == professorId ? [_atleta] : [];
    }

    private sealed class LinkRepositoryFake : ILinkPublicoIntegracaoRepository
    {
        private LinkPublicoIntegracaoData? _link;

        public LinkPublicoIntegracaoData? ObterAtivoPorAtletaId(Guid atletaId)
            => _link?.Link.AtletaId == atletaId && _link.Link.Ativo ? _link : null;

        public LinkPublicoIntegracao? ObterPorTokenHash(string tokenHash)
            => _link?.Link.TokenHash == tokenHash ? _link.Link : null;

        public void Salvar(LinkPublicoIntegracao link, string tokenProtegido)
        {
            _link = new LinkPublicoIntegracaoData(link, tokenProtegido);
        }
    }

    private sealed class SecretProtectorFake : ISecretProtector
    {
        public string Protect(string plaintext) => $"protected::{plaintext}";

        public string Unprotect(string protectedValue) => protectedValue.Replace("protected::", string.Empty, StringComparison.Ordinal);
    }

    private sealed class PublicLinkUrlBuilderFake : IPublicLinkUrlBuilder
    {
        public string BuildConnectionUrl(string tokenPublico) => $"https://coachtraining.test/conectar/{tokenPublico}";
    }

    [Fact]
    public void GerarOuObter_DeveCriarNovoTokenQuandoNaoExisteLinkAtivo()
    {
        var professorId = Guid.NewGuid();
        var atleta = new Atleta("Atleta Link", professorId, id: Guid.NewGuid());
        var service = new GerarLinkPublicoIntegracaoService(
            new AtletaRepositoryFake(atleta),
            new LinkRepositoryFake(),
            new SecretProtectorFake(),
            new PublicLinkUrlBuilderFake());

        var resultado = service.GerarOuObter(atleta.Id, professorId);

        Assert.Equal(atleta.Id, resultado.AtletaId);
        Assert.False(string.IsNullOrWhiteSpace(resultado.TokenPublico));
        Assert.False(string.IsNullOrWhiteSpace(resultado.UrlPublica));
    }

    [Fact]
    public void GerarOuObter_DeveReutilizarTokenExistenteQuandoLinkAtivoJaExiste()
    {
        var professorId = Guid.NewGuid();
        var atleta = new Atleta("Atleta Link", professorId, id: Guid.NewGuid());
        var repo = new LinkRepositoryFake();
        var protector = new SecretProtectorFake();
        var existente = LinkPublicoIntegracao.Criar(atleta.Id, "hash");
        repo.Salvar(existente, protector.Protect("token-existente"));
        var service = new GerarLinkPublicoIntegracaoService(
            new AtletaRepositoryFake(atleta),
            repo,
            protector,
            new PublicLinkUrlBuilderFake());

        var resultado = service.GerarOuObter(atleta.Id, professorId);

        Assert.Equal("token-existente", resultado.TokenPublico);
        Assert.Contains("token-existente", resultado.UrlPublica);
    }

    [Fact]
    public void Regenerar_DeveSubstituirTokenExistente()
    {
        var professorId = Guid.NewGuid();
        var atleta = new Atleta("Atleta Link", professorId, id: Guid.NewGuid());
        var repo = new LinkRepositoryFake();
        var protector = new SecretProtectorFake();
        var existente = LinkPublicoIntegracao.Criar(atleta.Id, "hash");
        repo.Salvar(existente, protector.Protect("token-antigo"));
        var service = new GerarLinkPublicoIntegracaoService(
            new AtletaRepositoryFake(atleta),
            repo,
            protector,
            new PublicLinkUrlBuilderFake());

        var resultado = service.Regenerar(atleta.Id, professorId);

        Assert.NotEqual("token-antigo", resultado.TokenPublico);
        Assert.False(string.IsNullOrWhiteSpace(resultado.TokenPublico));
    }
}
