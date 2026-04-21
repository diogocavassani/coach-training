namespace CoachTraining.Infra.Integrations.Strava;

public class StravaOptions
{
    public const string SectionName = "Strava";

    public int ClientId { get; set; } = 1;
    public string ClientSecret { get; set; } = "change-me";
    public string TokenEndpoint { get; set; } = "https://www.strava.com/oauth/token";
}
