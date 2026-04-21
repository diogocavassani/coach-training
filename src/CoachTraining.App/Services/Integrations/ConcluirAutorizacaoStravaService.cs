using CoachTraining.App.Abstractions.Integrations;
using CoachTraining.App.Abstractions.Persistence;
using CoachTraining.App.Abstractions.Security;
using CoachTraining.App.DTOs.Integrations;
using CoachTraining.Domain.Entities;
using CoachTraining.Domain.Enums;

namespace CoachTraining.App.Services.Integrations;

public class ConcluirAutorizacaoStravaService
{
    private readonly ILinkPublicoIntegracaoRepository _linkRepository;
    private readonly IConexaoWearableRepository _conexaoRepository;
    private readonly ICredencialWearableRepository _credencialRepository;
    private readonly IWearableProviderRegistry _providerRegistry;
    private readonly ISecretProtector _secretProtector;
    private readonly StravaOAuthStateService _stateService;
    private readonly IPublicLinkUrlBuilder _publicLinkUrlBuilder;

    public ConcluirAutorizacaoStravaService(
        ILinkPublicoIntegracaoRepository linkRepository,
        IConexaoWearableRepository conexaoRepository,
        ICredencialWearableRepository credencialRepository,
        IWearableProviderRegistry providerRegistry,
        ISecretProtector secretProtector,
        StravaOAuthStateService stateService,
        IPublicLinkUrlBuilder publicLinkUrlBuilder)
    {
        _linkRepository = linkRepository ?? throw new ArgumentNullException(nameof(linkRepository));
        _conexaoRepository = conexaoRepository ?? throw new ArgumentNullException(nameof(conexaoRepository));
        _credencialRepository = credencialRepository ?? throw new ArgumentNullException(nameof(credencialRepository));
        _providerRegistry = providerRegistry ?? throw new ArgumentNullException(nameof(providerRegistry));
        _secretProtector = secretProtector ?? throw new ArgumentNullException(nameof(secretProtector));
        _stateService = stateService ?? throw new ArgumentNullException(nameof(stateService));
        _publicLinkUrlBuilder = publicLinkUrlBuilder ?? throw new ArgumentNullException(nameof(publicLinkUrlBuilder));
    }

    public async Task<ConcluirAutorizacaoResultDto> ConcluirAsync(
        string? code,
        string? scope,
        string? stateProtegido,
        string? error,
        CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(error))
        {
            return new ConcluirAutorizacaoResultDto
            {
                RedirectUrl = _publicLinkUrlBuilder.BuildStravaReturnUrl("error", error)
            };
        }

        if (string.IsNullOrWhiteSpace(code))
        {
            return new ConcluirAutorizacaoResultDto
            {
                RedirectUrl = _publicLinkUrlBuilder.BuildStravaReturnUrl("error", "missing_code")
            };
        }

        var state = _stateService.Desproteger(stateProtegido);
        if (state == null || state.ExpiresAtUtc < DateTime.UtcNow)
        {
            return new ConcluirAutorizacaoResultDto
            {
                RedirectUrl = _publicLinkUrlBuilder.BuildStravaReturnUrl("error", "invalid_state")
            };
        }

        var link = _linkRepository.ObterPorTokenHash(state.TokenHash);
        if (link == null || !link.Ativo)
        {
            return new ConcluirAutorizacaoResultDto
            {
                RedirectUrl = _publicLinkUrlBuilder.BuildStravaReturnUrl("error", "invalid_link")
            };
        }

        var provider = _providerRegistry.GetRequired(ProvedorIntegracao.Strava);
        var tokenResult = await provider.ExchangeAuthorizationCodeAsync(code.Trim(), cancellationToken);
        var agora = DateTime.UtcNow;
        var conexaoExistente = _conexaoRepository.ObterPorAtletaIdEProvedor(link.AtletaId, ProvedorIntegracao.Strava);

        if (conexaoExistente == null)
        {
            conexaoExistente = new ConexaoWearable(
                atletaId: link.AtletaId,
                provedor: ProvedorIntegracao.Strava,
                status: StatusConexaoIntegracao.Conectado,
                externalAthleteId: tokenResult.ExternalAthleteId,
                scopesConcedidos: scope?.Trim() ?? tokenResult.ScopesConcedidos,
                conectadoEmUtc: agora);
        }
        else
        {
            conexaoExistente.RegistrarAutorizacao(tokenResult.ExternalAthleteId, scope?.Trim() ?? tokenResult.ScopesConcedidos, agora);
        }

        _conexaoRepository.Salvar(conexaoExistente);

        var credencialExistente = _credencialRepository.ObterPorConexaoWearableId(conexaoExistente.Id);
        var credencial = new CredencialWearable(
            conexaoWearableId: conexaoExistente.Id,
            accessTokenProtegido: _secretProtector.Protect(tokenResult.AccessToken),
            refreshTokenProtegido: _secretProtector.Protect(tokenResult.RefreshToken),
            expiresAtUtc: tokenResult.ExpiresAtUtc,
            atualizadoEmUtc: agora,
            id: credencialExistente?.Id);

        _credencialRepository.Salvar(credencial);

        return new ConcluirAutorizacaoResultDto
        {
            RedirectUrl = _publicLinkUrlBuilder.BuildStravaReturnUrl("success")
        };
    }
}
