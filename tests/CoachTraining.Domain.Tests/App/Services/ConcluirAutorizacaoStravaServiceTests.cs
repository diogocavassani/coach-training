using CoachTraining.App.Abstractions.Integrations;
using CoachTraining.App.Abstractions.Persistence;
using CoachTraining.App.Abstractions.Security;
using CoachTraining.App.Services.Integrations;
using CoachTraining.Domain.Entities;
using CoachTraining.Domain.Enums;

namespace CoachTraining.Tests.App.Services;

public class ConcluirAutorizacaoStravaServiceTests
{
    private sealed class LinkRepositoryFake : ILinkPublicoIntegracaoRepository
    {
        private readonly Dictionary<string, LinkPublicoIntegracao> _linksPorHash = new(StringComparer.Ordinal);

        public LinkPublicoIntegracaoData? ObterAtivoPorAtletaId(Guid atletaId) => null;

        public LinkPublicoIntegracao? ObterPorTokenHash(string tokenHash)
            => _linksPorHash.TryGetValue(tokenHash, out var link) ? link : null;

        public void Salvar(LinkPublicoIntegracao link, string tokenProtegido)
        {
            _linksPorHash[link.TokenHash] = link;
        }
    }

    private sealed class ConexaoRepositoryFake : IConexaoWearableRepository
    {
        public List<ConexaoWearable> Itens { get; } = [];

        public IReadOnlyList<ConexaoWearable> ListarPorAtletaId(Guid atletaId)
            => Itens.Where(item => item.AtletaId == atletaId).ToList();

        public ConexaoWearable? ObterPorAtletaIdEProvedor(Guid atletaId, ProvedorIntegracao provedor)
            => Itens.FirstOrDefault(item => item.AtletaId == atletaId && item.Provedor == provedor);

        public ConexaoWearable? ObterPorExternalAthleteId(ProvedorIntegracao provedor, string externalAthleteId)
            => Itens.FirstOrDefault(item => item.Provedor == provedor && item.ExternalAthleteId == externalAthleteId);

        public void Salvar(ConexaoWearable conexao)
        {
            var existente = Itens.FindIndex(item => item.Id == conexao.Id);
            if (existente >= 0)
            {
                Itens[existente] = conexao;
                return;
            }

            Itens.Add(conexao);
        }
    }

    private sealed class CredencialRepositoryFake : ICredencialWearableRepository
    {
        public List<CredencialWearable> Itens { get; } = [];

        public CredencialWearable? ObterPorConexaoWearableId(Guid conexaoWearableId)
            => Itens.FirstOrDefault(item => item.ConexaoWearableId == conexaoWearableId);

        public void Salvar(CredencialWearable credencial)
        {
            var existente = Itens.FindIndex(item => item.ConexaoWearableId == credencial.ConexaoWearableId);
            if (existente >= 0)
            {
                Itens[existente] = credencial;
                return;
            }

            Itens.Add(credencial);
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
        public string BuildStravaReturnUrl(string status, string? reason = null) => $"https://coachtraining.test/conectar/strava/retorno?status={status}&reason={reason}";
    }

    private sealed class WearableProviderFake : IWearableProvider
    {
        public ProvedorIntegracao Provedor => ProvedorIntegracao.Strava;

        public string BuildAuthorizationUrl(string redirectUri, string state)
            => $"https://www.strava.com/oauth/authorize?redirect_uri={Uri.EscapeDataString(redirectUri)}&state={Uri.EscapeDataString(state)}";

        public Task<WearableTokenExchangeResult> ExchangeAuthorizationCodeAsync(string code, CancellationToken cancellationToken)
            => Task.FromResult(new WearableTokenExchangeResult("987", "access", "refresh", DateTime.UtcNow.AddHours(6), "activity:read"));

        public Task<WearableTokenExchangeResult> RefreshAccessTokenAsync(string refreshToken, CancellationToken cancellationToken)
            => Task.FromResult(new WearableTokenExchangeResult("987", "access", "refresh", DateTime.UtcNow.AddHours(6), "activity:read"));

        public Task<WearableActivityDto> GetActivityAsync(string accessToken, string externalActivityId, CancellationToken cancellationToken)
            => Task.FromResult(new WearableActivityDto(externalActivityId, "987", "Run", DateTime.UtcNow, 1000, 300, 320));
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
    public async Task Concluir_DevePersistirConexaoECredenciaisEDevolverRedirectDeSucesso()
    {
        var protector = new SecretProtectorFake();
        var tokenPublico = "token-opaco";
        var tokenHash = GerarLinkPublicoIntegracaoService.GerarHash(tokenPublico);
        var link = LinkPublicoIntegracao.Criar(Guid.NewGuid(), tokenHash);
        var linkRepository = new LinkRepositoryFake();
        linkRepository.Salvar(link, protector.Protect(tokenPublico));
        var conexaoRepo = new ConexaoRepositoryFake();
        var credencialRepo = new CredencialRepositoryFake();
        var stateService = new StravaOAuthStateService(protector);
        var service = new ConcluirAutorizacaoStravaService(
            linkRepository,
            conexaoRepo,
            credencialRepo,
            new WearableProviderRegistryFake(new WearableProviderFake()),
            protector,
            stateService,
            new PublicLinkUrlBuilderFake());
        var state = stateService.Proteger(new StravaOAuthState(link.TokenHash, DateTime.UtcNow.AddMinutes(10), Guid.NewGuid().ToString("N")));

        var result = await service.ConcluirAsync("code-ok", "activity:read", state, null, CancellationToken.None);

        Assert.Contains("status=success", result.RedirectUrl);
        Assert.Single(conexaoRepo.Itens);
        Assert.Single(credencialRepo.Itens);
        Assert.Equal(link.AtletaId, conexaoRepo.Itens[0].AtletaId);
        Assert.Equal(StatusConexaoIntegracao.Conectado, conexaoRepo.Itens[0].Status);
    }
}
