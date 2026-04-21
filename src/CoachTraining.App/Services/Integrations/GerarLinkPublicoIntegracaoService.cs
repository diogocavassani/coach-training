using System.Security.Cryptography;
using System.Text;
using CoachTraining.App.Abstractions.Integrations;
using CoachTraining.App.Abstractions.Persistence;
using CoachTraining.App.Abstractions.Security;
using CoachTraining.App.DTOs.Integrations;
using CoachTraining.Domain.Entities;

namespace CoachTraining.App.Services.Integrations;

public class GerarLinkPublicoIntegracaoService
{
    private readonly IAtletaRepository _atletaRepository;
    private readonly ILinkPublicoIntegracaoRepository _linkRepository;
    private readonly ISecretProtector _secretProtector;
    private readonly IPublicLinkUrlBuilder _publicLinkUrlBuilder;

    public GerarLinkPublicoIntegracaoService(
        IAtletaRepository atletaRepository,
        ILinkPublicoIntegracaoRepository linkRepository,
        ISecretProtector secretProtector,
        IPublicLinkUrlBuilder publicLinkUrlBuilder)
    {
        _atletaRepository = atletaRepository ?? throw new ArgumentNullException(nameof(atletaRepository));
        _linkRepository = linkRepository ?? throw new ArgumentNullException(nameof(linkRepository));
        _secretProtector = secretProtector ?? throw new ArgumentNullException(nameof(secretProtector));
        _publicLinkUrlBuilder = publicLinkUrlBuilder ?? throw new ArgumentNullException(nameof(publicLinkUrlBuilder));
    }

    public LinkPublicoIntegracaoDto GerarOuObter(Guid atletaId, Guid professorId)
    {
        ValidarAtleta(atletaId, professorId);

        var existente = _linkRepository.ObterAtivoPorAtletaId(atletaId);
        if (existente != null)
        {
            var tokenExistente = _secretProtector.Unprotect(existente.TokenProtegido);
            return CriarDto(atletaId, tokenExistente);
        }

        return CriarNovo(atletaId);
    }

    public LinkPublicoIntegracaoDto Regenerar(Guid atletaId, Guid professorId)
    {
        ValidarAtleta(atletaId, professorId);

        var tokenNovo = GerarTokenPublico();
        var tokenHash = GerarHash(tokenNovo);
        var tokenProtegido = _secretProtector.Protect(tokenNovo);
        var existente = _linkRepository.ObterAtivoPorAtletaId(atletaId);

        if (existente == null)
        {
            var novoLink = LinkPublicoIntegracao.Criar(atletaId, tokenHash);
            _linkRepository.Salvar(novoLink, tokenProtegido);
            return CriarDto(atletaId, tokenNovo);
        }

        existente.Link.Regenerar(tokenHash, DateTime.UtcNow);
        _linkRepository.Salvar(existente.Link, tokenProtegido);

        return CriarDto(atletaId, tokenNovo);
    }

    private LinkPublicoIntegracaoDto CriarNovo(Guid atletaId)
    {
        var token = GerarTokenPublico();
        var tokenHash = GerarHash(token);
        var link = LinkPublicoIntegracao.Criar(atletaId, tokenHash);
        _linkRepository.Salvar(link, _secretProtector.Protect(token));
        return CriarDto(atletaId, token);
    }

    private LinkPublicoIntegracaoDto CriarDto(Guid atletaId, string tokenPublico)
    {
        return new LinkPublicoIntegracaoDto
        {
            AtletaId = atletaId,
            TokenPublico = tokenPublico,
            UrlPublica = _publicLinkUrlBuilder.BuildConnectionUrl(tokenPublico)
        };
    }

    private void ValidarAtleta(Guid atletaId, Guid professorId)
    {
        if (atletaId == Guid.Empty)
        {
            throw new ArgumentException("AtletaId invalido.", nameof(atletaId));
        }

        if (professorId == Guid.Empty)
        {
            throw new ArgumentException("ProfessorId invalido.", nameof(professorId));
        }

        _ = _atletaRepository.ObterPorId(atletaId, professorId)
            ?? throw new UnauthorizedAccessException("Atleta nao encontrado para o professor autenticado.");
    }

    public static string GerarHash(string tokenPublico)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(tokenPublico));
        return Convert.ToHexString(bytes);
    }

    private static string GerarTokenPublico()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(32))
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');
    }
}
