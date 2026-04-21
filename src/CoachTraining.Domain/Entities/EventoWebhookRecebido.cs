using CoachTraining.Domain.Enums;

namespace CoachTraining.Domain.Entities;

public class EventoWebhookRecebido
{
    public EventoWebhookRecebido(
        ProvedorIntegracao provedor,
        string objectType,
        string objectId,
        string ownerId,
        string aspectType,
        string payloadJson,
        string fingerprint,
        DateTime recebidoEmUtc,
        Guid? id = null)
    {
        if (string.IsNullOrWhiteSpace(objectType))
        {
            throw new ArgumentException("ObjectType obrigatorio.", nameof(objectType));
        }

        if (string.IsNullOrWhiteSpace(objectId))
        {
            throw new ArgumentException("ObjectId obrigatorio.", nameof(objectId));
        }

        if (string.IsNullOrWhiteSpace(ownerId))
        {
            throw new ArgumentException("OwnerId obrigatorio.", nameof(ownerId));
        }

        if (string.IsNullOrWhiteSpace(aspectType))
        {
            throw new ArgumentException("AspectType obrigatorio.", nameof(aspectType));
        }

        if (string.IsNullOrWhiteSpace(payloadJson))
        {
            throw new ArgumentException("PayloadJson obrigatorio.", nameof(payloadJson));
        }

        if (string.IsNullOrWhiteSpace(fingerprint))
        {
            throw new ArgumentException("Fingerprint obrigatorio.", nameof(fingerprint));
        }

        Id = id ?? Guid.NewGuid();
        Provedor = provedor;
        ObjectType = objectType.Trim();
        ObjectId = objectId.Trim();
        OwnerId = ownerId.Trim();
        AspectType = aspectType.Trim();
        PayloadJson = payloadJson;
        Fingerprint = fingerprint.Trim();
        RecebidoEmUtc = recebidoEmUtc;
    }

    public Guid Id { get; private set; }
    public ProvedorIntegracao Provedor { get; private set; }
    public string ObjectType { get; private set; }
    public string ObjectId { get; private set; }
    public string OwnerId { get; private set; }
    public string AspectType { get; private set; }
    public string PayloadJson { get; private set; }
    public string Fingerprint { get; private set; }
    public DateTime RecebidoEmUtc { get; private set; }
    public DateTime? ProcessadoEmUtc { get; private set; }
    public string? StatusProcessamento { get; private set; }
    public string? ErroProcessamento { get; private set; }

    public void MarcarComoProcessado(DateTime quando)
    {
        ProcessadoEmUtc = quando;
        StatusProcessamento = "Processado";
        ErroProcessamento = null;
    }

    public void MarcarComoFalho(DateTime quando, string erro)
    {
        ProcessadoEmUtc = quando;
        StatusProcessamento = "Falhou";
        ErroProcessamento = erro;
    }
}
