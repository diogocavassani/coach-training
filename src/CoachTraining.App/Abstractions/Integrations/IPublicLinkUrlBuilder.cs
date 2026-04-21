namespace CoachTraining.App.Abstractions.Integrations;

public interface IPublicLinkUrlBuilder
{
    string BuildConnectionUrl(string tokenPublico);
    string BuildStravaCallbackUrl();
    string BuildStravaReturnUrl(string status, string? reason = null);
}
