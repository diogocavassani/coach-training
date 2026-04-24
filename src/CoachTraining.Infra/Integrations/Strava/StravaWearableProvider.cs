using System.Globalization;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
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
            ExternalAthleteId: payload.Athlete?.Id.ToString(CultureInfo.InvariantCulture) ?? string.Empty,
            AccessToken: payload.AccessToken,
            RefreshToken: payload.RefreshToken,
            ExpiresAtUtc: DateTimeOffset.FromUnixTimeSeconds(payload.ExpiresAt).UtcDateTime,
            ScopesConcedidos: "activity:read");
    }

    public async Task<WearableTokenExchangeResult> RefreshAccessTokenAsync(string refreshToken, CancellationToken cancellationToken)
    {
        var response = await _httpClient.PostAsJsonAsync(
            _options.TokenEndpoint,
            new
            {
                client_id = _options.ClientId,
                client_secret = _options.ClientSecret,
                refresh_token = refreshToken,
                grant_type = "refresh_token"
            },
            cancellationToken);

        response.EnsureSuccessStatusCode();
        var payload = await response.Content.ReadFromJsonAsync<StravaTokenResponse>(cancellationToken: cancellationToken)
            ?? throw new InvalidOperationException("Resposta do Strava sem payload.");

        return new WearableTokenExchangeResult(
            ExternalAthleteId: string.Empty,
            AccessToken: payload.AccessToken,
            RefreshToken: payload.RefreshToken,
            ExpiresAtUtc: DateTimeOffset.FromUnixTimeSeconds(payload.ExpiresAt).UtcDateTime,
            ScopesConcedidos: "activity:read");
    }

    public async Task<WearableActivityDto> GetActivityAsync(string accessToken, string externalActivityId, CancellationToken cancellationToken)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_options.ActivitiesBaseUrl}/activities/{externalActivityId}");
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
        
        var response = await _httpClient.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();
        
        var payload = await response.Content.ReadFromJsonAsync<StravaActivityResponse>(cancellationToken: cancellationToken)
            ?? throw new InvalidOperationException("Resposta do Strava sem payload.");

        return new WearableActivityDto(
            ExternalActivityId: payload.Id.ToString(CultureInfo.InvariantCulture),
            ExternalAthleteId: payload.Athlete?.Id.ToString(CultureInfo.InvariantCulture) ?? string.Empty,
            SportType: payload.SportType,
            StartDateUtc: payload.StartDate,
            DistanceMeters: payload.Distance,
            MovingTimeSeconds: payload.MovingTime,
            ElapsedTimeSeconds: payload.ElapsedTime);
    }

    private sealed class StravaTokenResponse
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; } = string.Empty;
        [JsonPropertyName("refresh_token")]
        public string RefreshToken { get; set; } = string.Empty;
        [JsonPropertyName("expires_at")]
        public long ExpiresAt { get; set; }
        [JsonPropertyName("athlete")]
        public StravaAthleteResponse? Athlete { get; set; }
    }

    private sealed class StravaAthleteResponse
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }
    }

    private sealed class StravaActivityResponse
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }
        
        [JsonPropertyName("athlete")]
        public StravaAthleteResponse? Athlete { get; set; }
        
        [JsonPropertyName("sport_type")]
        public string SportType { get; set; } = string.Empty;
        
        [JsonPropertyName("start_date")]
        public DateTime StartDate { get; set; }
        
        [JsonPropertyName("distance")]
        public double Distance { get; set; }
        
        [JsonPropertyName("moving_time")]
        public int MovingTime { get; set; }
        
        [JsonPropertyName("elapsed_time")]
        public int ElapsedTime { get; set; }
    }
}
