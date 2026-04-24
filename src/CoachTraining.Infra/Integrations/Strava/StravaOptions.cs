using CoachTraining.App.Abstractions.Integrations;

namespace CoachTraining.Infra.Integrations.Strava;

public class StravaOptions : IStravaWebhookOptions
{
    public const string SectionName = "Strava";

    public int ClientId { get; set; } = 1;
    public string ClientSecret { get; set; } = "change-me";
    public string TokenEndpoint { get; set; } = "https://www.strava.com/oauth/token";
    public string ActivitiesBaseUrl { get; set; } = "https://www.strava.com/api/v3";
    public string WebhookVerifyToken { get; set; } = "strava-verify-token";
    public string WebhookSecretPathSegment { get; set; } = "strava-hook-secret";
}
