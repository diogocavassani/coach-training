using CoachTraining.Domain.Enums;

namespace CoachTraining.App.Abstractions.Integrations;

public sealed record WearableTokenExchangeResult(
    string ExternalAthleteId,
    string AccessToken,
    string RefreshToken,
    DateTime ExpiresAtUtc,
    string ScopesConcedidos);

public sealed record WearableActivityDto(
    string ExternalActivityId,
    string ExternalAthleteId,
    string SportType,
    DateTime StartDateUtc,
    double DistanceMeters,
    int MovingTimeSeconds,
    int ElapsedTimeSeconds);

public interface IWearableProvider
{
    ProvedorIntegracao Provedor { get; }
    string BuildAuthorizationUrl(string redirectUri, string state);
    Task<WearableTokenExchangeResult> ExchangeAuthorizationCodeAsync(string code, CancellationToken cancellationToken);
    Task<WearableTokenExchangeResult> RefreshAccessTokenAsync(string refreshToken, CancellationToken cancellationToken);
    Task<WearableActivityDto> GetActivityAsync(string accessToken, string externalActivityId, CancellationToken cancellationToken);
}
