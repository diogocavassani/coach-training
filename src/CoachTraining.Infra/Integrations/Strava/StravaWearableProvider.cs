using System.Globalization;
using System.Net.Http.Json;
using CoachTraining.App.Abstractions.Integrations;
using CoachTraining.Domain.Enums;
using Microsoft.Extensions.Options;

namespace CoachTraining.Infra.Integrations.Strava;

public class StravaWearableProvider : IWearableProvider
{
    private readonly HttpClient _httpClient;
    private readonly StravaOptions _options;

    public StravaWearableProvider(HttpClient httpClient, IOptions<StravaOptions> options)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
    }

    public ProvedorIntegracao Provedor => ProvedorIntegracao.Strava;

    public string BuildAuthorizationUrl(string redirectUri, string state)
    {
        var query = new Dictionary<string, string?>
        {
            ["client_id"] = _options.ClientId.ToString(CultureInfo.InvariantCulture),
            ["redirect_uri"] = redirectUri,
            ["response_type"] = "code",
            ["approval_prompt"] = "auto",
            ["scope"] = "activity:read",
            ["state"] = state
        };

        var queryString = string.Join(
            "&",
            query.Select(item => $"{Uri.EscapeDataString(item.Key)}={Uri.EscapeDataString(item.Value ?? string.Empty)}"));

        return $"https://www.strava.com/oauth/authorize?{queryString}";
    }

    public async Task<WearableTokenExchangeResult> ExchangeAuthorizationCodeAsync(string code, CancellationToken cancellationToken)
    {
        var response = await _httpClient.PostAsJsonAsync(
            _options.TokenEndpoint,
            new
            {
                client_id = _options.ClientId,
                client_secret = _options.ClientSecret,
                code,
                grant_type = "authorization_code"
            },
            cancellationToken);

        response.EnsureSuccessStatusCode();
        var payload = await response.Content.ReadFromJsonAsync<StravaTokenResponse>(cancellationToken: cancellationToken)
            ?? throw new InvalidOperationException("Resposta do Strava sem payload.");

        return new WearableTokenExchangeResult(
            ExternalAthleteId: payload.Athlete.Id.ToString(CultureInfo.InvariantCulture),
            AccessToken: payload.AccessToken,
            RefreshToken: payload.RefreshToken,
            ExpiresAtUtc: DateTimeOffset.FromUnixTimeSeconds(payload.ExpiresAt).UtcDateTime,
            ScopesConcedidos: "activity:read");
    }

    private sealed class StravaTokenResponse
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public long ExpiresAt { get; set; }
        public StravaAthleteResponse Athlete { get; set; } = new();
    }

    private sealed class StravaAthleteResponse
    {
        public long Id { get; set; }
    }
}
