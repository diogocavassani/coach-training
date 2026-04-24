using CoachTraining.App.Abstractions.Integrations;
using CoachTraining.App.Services.Integrations;
using Microsoft.Extensions.DependencyInjection;

namespace CoachTraining.Infra.Integrations.Strava;

public class InProcessStravaWebhookDispatcher : IStravaWebhookDispatcher
{
    private readonly IServiceScopeFactory _scopeFactory;

    public InProcessStravaWebhookDispatcher(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));
    }

    public void Dispatch(Guid eventoId)
    {
        _ = Task.Run(async () =>
        {
            using var scope = _scopeFactory.CreateScope();
            var processor = scope.ServiceProvider.GetRequiredService<ProcessarEventoStravaService>();
            await processor.ProcessarAsync(eventoId, CancellationToken.None);
        });
    }
}
