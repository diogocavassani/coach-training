using CoachTraining.App.Abstractions.Integrations;
using CoachTraining.App.Abstractions.Persistence;
using CoachTraining.App.Abstractions.Security;
using CoachTraining.App.DTOs.Integrations;

namespace CoachTraining.App.Services.Integrations;

public class ConsultarIntegracoesAtletaService
{
    private readonly IAtletaRepository _atletaRepository;
    private readonly ILinkPublicoIntegracaoRepository _linkRepository;
    private readonly IConexaoWearableRepository _conexaoRepository;
    private readonly ISecretProtector _secretProtector;
    private readonly IPublicLinkUrlBuilder _publicLinkUrlBuilder;

    public ConsultarIntegracoesAtletaService(
        IAtletaRepository atletaRepository,
        ILinkPublicoIntegracaoRepository linkRepository,
        IConexaoWearableRepository conexaoRepository,
        ISecretProtector secretProtector,
        IPublicLinkUrlBuilder publicLinkUrlBuilder)
    {
        _atletaRepository = atletaRepository ?? throw new ArgumentNullException(nameof(atletaRepository));
        _linkRepository = linkRepository ?? throw new ArgumentNullException(nameof(linkRepository));
        _conexaoRepository = conexaoRepository ?? throw new ArgumentNullException(nameof(conexaoRepository));
        _secretProtector = secretProtector ?? throw new ArgumentNullException(nameof(secretProtector));
        _publicLinkUrlBuilder = publicLinkUrlBuilder ?? throw new ArgumentNullException(nameof(publicLinkUrlBuilder));
    }

    public AtletaIntegracoesResumoDto Consultar(Guid atletaId, Guid professorId)
    {
        if (_atletaRepository.ObterPorId(atletaId, professorId) == null)
        {
            throw new UnauthorizedAccessException("Atleta nao encontrado para o professor autenticado.");
        }

        var link = _linkRepository.ObterAtivoPorAtletaId(atletaId);
        var tokenPublico = link == null ? null : _secretProtector.Unprotect(link.TokenProtegido);

        return new AtletaIntegracoesResumoDto
        {
            AtletaId = atletaId,
            TokenPublicoAtual = tokenPublico,
            UrlPublicaAtual = tokenPublico == null ? null : _publicLinkUrlBuilder.BuildConnectionUrl(tokenPublico),
            Provedores = IntegracaoProviderCatalog.CriarStatus(_conexaoRepository.ListarPorAtletaId(atletaId))
        };
    }
}
