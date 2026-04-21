namespace CoachTraining.Infra.Persistence.Models;

public class CredencialWearableModel
{
    public Guid Id { get; set; }
    public Guid ConexaoWearableId { get; set; }
    public string AccessTokenProtegido { get; set; } = string.Empty;
    public string RefreshTokenProtegido { get; set; } = string.Empty;
    public DateTime ExpiresAtUtc { get; set; }
    public DateTime AtualizadoEmUtc { get; set; }
    public ConexaoWearableModel ConexaoWearable { get; set; } = null!;
}
