using CoachTraining.App.Abstractions.Integrations;
using CoachTraining.App.Abstractions.Persistence;
using CoachTraining.App.DTOs.Integrations;
using CoachTraining.Domain.Enums;

namespace CoachTraining.App.Services.Integrations;

public class IniciarAutorizacaoStravaService
{
    private readonly ILinkPublicoIntegracaoRepository _linkRepository;
    private readonly IWearableProviderRegistry _providerRegistry;
    private readonly StravaOAuthStateService _stateService;
    private readonly IPublicLinkUrlBuilder _publicLinkUrlBuilder;

    public IniciarAutorizacaoStravaService(
        ILinkPublicoIntegracaoRepository linkRepository,
        IWearableProviderRegistry providerRegistry,
        StravaOAuthStateService stateService,
        IPublicLinkUrlBuilder publicLinkUrlBuilder)
    {
        _linkRepository = linkRepository ?? throw new ArgumentNullException(nameof(linkRepository));
        _providerRegistry = providerRegistry ?? throw new ArgumentNullException(nameof(providerRegistry));
        _stateService = stateService ?? throw new ArgumentNullException(nameof(stateService));
        _publicLinkUrlBuilder = publicLinkUrlBuilder ?? throw new ArgumentNullException(nameof(publicLinkUrlBuilder));
    }

    public IniciarAutorizacaoResponseDto Iniciar(string tokenPublico)
    {
        if (string.IsNullOrWhiteSpace(tokenPublico))
        {
            throw new ArgumentException("Token publico obrigatorio.", nameof(tokenPublico));
        }

        var tokenHash = GerarLinkPublicoIntegracaoService.GerarHash(tokenPublico.Trim());
        var link = _linkRepository.ObterPorTokenHash(tokenHash);
        if (link == null || !link.Ativo)
        {
            throw new KeyNotFoundException("Link publico nao encontrado.");
        }

        var provider = _providerRegistry.GetRequired(ProvedorIntegracao.Strava);
        var state = _stateService.Proteger(
            new StravaOAuthState(
                TokenHash: tokenHash,
                ExpiresAtUtc: DateTime.UtcNow.AddMinutes(10),
                Nonce: Guid.NewGuid().ToString("N")));

        return new IniciarAutorizacaoResponseDto
        {
            AuthorizationUrl = provider.BuildAuthorizationUrl(_publicLinkUrlBuilder.BuildStravaCallbackUrl(), state)
        };
    }
}
