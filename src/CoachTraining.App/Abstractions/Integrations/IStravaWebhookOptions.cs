namespace CoachTraining.App.Abstractions.Integrations;

public interface IStravaWebhookOptions
{
    string WebhookVerifyToken { get; }
    string WebhookSecretPathSegment { get; }
}
