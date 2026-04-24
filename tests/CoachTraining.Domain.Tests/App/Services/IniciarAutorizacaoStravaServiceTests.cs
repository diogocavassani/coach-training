using CoachTraining.App.Abstractions.Integrations;
using CoachTraining.App.Abstractions.Persistence;
using CoachTraining.App.Abstractions.Security;
using CoachTraining.App.DTOs.Integrations;
using CoachTraining.App.Services.Integrations;
using CoachTraining.Domain.Entities;
using CoachTraining.Domain.Enums;

namespace CoachTraining.Tests.App.Services;

public class IniciarAutorizacaoStravaServiceTests
{
    private sealed class LinkRepositoryFake : ILinkPublicoIntegracaoRepository
    {
        private readonly Dictionary<string, LinkPublicoIntegracao> _linksPorHash = new(StringComparer.Ordinal);
        private LinkPublicoIntegracaoData? _linkAtivo;

        public LinkPublicoIntegracaoData? ObterAtivoPorAtletaId(Guid atletaId)
            => _linkAtivo?.Link.AtletaId == atletaId ? _linkAtivo : null;

        public LinkPublicoIntegracao? ObterPorTokenHash(string tokenHash)
            => _linksPorHash.TryGetValue(tokenHash, out var link) ? link : null;

        public void Salvar(LinkPublicoIntegracao link, string tokenProtegido)
        {
            _linkAtivo = new LinkPublicoIntegracaoData(link, tokenProtegido);
            _linksPorHash[link.TokenHash] = link;
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
        public string BuildStravaCallbackUrl() => "https://api.coachtraining.test/public/integracoes/strava/callback";
        public string BuildStravaReturnUrl(string status, string? reason = null) => $"https://coachtraining.test/conectar/strava/retorno?status={status}";
    }

    private sealed class WearableProviderFake : IWearableProvider
    {
        public ProvedorIntegracao Provedor => ProvedorIntegracao.Strava;

        public string BuildAuthorizationUrl(string redirectUri, string state)
            => $"https://www.strava.com/oauth/authorize?redirect_uri={Uri.EscapeDataString(redirectUri)}&scope=activity:read&state={Uri.EscapeDataString(state)}";

        public Task<WearableTokenExchangeResult> ExchangeAuthorizationCodeAsync(string code, CancellationToken cancellationToken)
            => Task.FromResult(new WearableTokenExchangeResult("123", "access", "refresh", DateTime.UtcNow.AddHours(6), "activity:read"));

        public Task<WearableTokenExchangeResult> RefreshAccessTokenAsync(string refreshToken, CancellationToken cancellationToken)
            => Task.FromResult(new WearableTokenExchangeResult("123", "access", "refresh", DateTime.UtcNow.AddHours(6), "activity:read"));

        public Task<WearableActivityDto> GetActivityAsync(string accessToken, string externalActivityId, CancellationToken cancellationToken)
            => Task.FromResult(new WearableActivityDto(externalActivityId, "123", "Run", DateTime.UtcNow, 1000, 300, 320));
    }

    private sealed class WearableProviderRegistryFake : IWearableProviderRegistry
    {
        private readonly IWearableProvider _provider;

        public WearableProviderRegistryFake(IWearableProvider provider)
        {
            _provider = provider;
        }

        public IWearableProvider GetRequired(ProvedorIntegracao provedor) => _provider;
    }

    [Fact]
    public void Iniciar_DeveMontarUrlOAuthDoStravaComStateAssinado()
    {
        var protector = new SecretProtectorFake();
        var linkRepository = new LinkRepositoryFake();
        var tokenPublico = "token-opaco";
        var link = LinkPublicoIntegracao.Criar(Guid.NewGuid(), GerarLinkPublicoIntegracaoService.GerarHash(tokenPublico));
        linkRepository.Salvar(link, protector.Protect(tokenPublico));
        var service = new IniciarAutorizacaoStravaService(
            linkRepository,
            new WearableProviderRegistryFake(new WearableProviderFake()),
            new StravaOAuthStateService(protector),
            new PublicLinkUrlBuilderFake());

        var response = service.Iniciar(tokenPublico);

        Assert.Contains("https://www.strava.com/oauth/authorize", response.AuthorizationUrl);
        Assert.Contains("scope=activity:read", response.AuthorizationUrl);
        Assert.Contains("state=", response.AuthorizationUrl);
    }
}
