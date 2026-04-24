using System.Text.Json;
using CoachTraining.App.Abstractions.Integrations;
using CoachTraining.App.Abstractions.Persistence;
using CoachTraining.Domain.Entities;
using CoachTraining.Domain.Enums;

namespace CoachTraining.App.Services.Integrations;

public class ReceberWebhookStravaService
{
    private readonly IEventoWebhookRepository _eventoRepository;
    private readonly IStravaWebhookDispatcher _dispatcher;
    private readonly IStravaWebhookOptions _options;

    public ReceberWebhookStravaService(
        IEventoWebhookRepository eventoRepository,
        IStravaWebhookDispatcher dispatcher,
        IStravaWebhookOptions options)
    {
        _eventoRepository = eventoRepository ?? throw new ArgumentNullException(nameof(eventoRepository));
        _dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));
        _options = options ?? throw new ArgumentNullException(nameof(options));
    }

    public bool ValidarHandshake(string secret, string verifyToken)
    {
        return string.Equals(secret, _options.WebhookSecretPathSegment, StringComparison.Ordinal)
            && string.Equals(verifyToken, _options.WebhookVerifyToken, StringComparison.Ordinal);
    }

    public Task<Guid> ReceberAsync(string secret, JsonElement payload, CancellationToken cancellationToken)
    {
        if (!string.Equals(secret, _options.WebhookSecretPathSegment, StringComparison.Ordinal))
        {
            throw new UnauthorizedAccessException("Webhook secret invalido.");
        }

        var evento = new EventoWebhookRecebido(
            provedor: ProvedorIntegracao.Strava,
            objectType: payload.GetProperty("object_type").GetString() ?? string.Empty,
            objectId: payload.GetProperty("object_id").ToString(),
            ownerId: payload.GetProperty("owner_id").ToString(),
            aspectType: payload.GetProperty("aspect_type").GetString() ?? string.Empty,
            payloadJson: payload.GetRawText(),
            fingerprint: $"{payload.GetProperty("object_type")}:"
                + $"{payload.GetProperty("aspect_type")}:"
                + $"{payload.GetProperty("object_id")}:"
                + $"{payload.GetProperty("owner_id")}",
            recebidoEmUtc: DateTime.UtcNow);

        var eventoId = _eventoRepository.Adicionar(evento);
        _dispatcher.Dispatch(eventoId);

        return Task.FromResult(eventoId);
    }
}
