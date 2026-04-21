namespace CoachTraining.Infra.Persistence.Models;

public class EventoWebhookRecebidoModel
{
    public Guid Id { get; set; }
    public int Provedor { get; set; }
    public string ObjectType { get; set; } = string.Empty;
    public string ObjectId { get; set; } = string.Empty;
    public string OwnerId { get; set; } = string.Empty;
    public string AspectType { get; set; } = string.Empty;
    public string PayloadJson { get; set; } = string.Empty;
    public string Fingerprint { get; set; } = string.Empty;
    public DateTime RecebidoEmUtc { get; set; }
    public DateTime? ProcessadoEmUtc { get; set; }
    public string? StatusProcessamento { get; set; }
    public string? ErroProcessamento { get; set; }
}
