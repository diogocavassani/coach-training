namespace CoachTraining.App.Abstractions.Integrations;

public interface IPublicLinkUrlBuilder
{
    string BuildConnectionUrl(string tokenPublico);
}
