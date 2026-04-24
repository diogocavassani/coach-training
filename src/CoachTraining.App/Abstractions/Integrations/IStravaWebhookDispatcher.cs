namespace CoachTraining.App.Abstractions.Integrations;

public interface IStravaWebhookDispatcher
{
    void Dispatch(Guid eventoId);
}
