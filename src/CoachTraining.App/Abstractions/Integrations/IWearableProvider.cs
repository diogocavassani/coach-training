using CoachTraining.Domain.Enums;

namespace CoachTraining.App.Abstractions.Integrations;

public sealed record WearableTokenExchangeResult(
    string ExternalAthleteId,
    string AccessToken,
    string RefreshToken,
    DateTime ExpiresAtUtc,
    string ScopesConcedidos);

public interface IWearableProvider
{
    ProvedorIntegracao Provedor { get; }
    string BuildAuthorizationUrl(string redirectUri, string state);
    Task<WearableTokenExchangeResult> ExchangeAuthorizationCodeAsync(string code, CancellationToken cancellationToken);
}
