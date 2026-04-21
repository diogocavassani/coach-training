namespace CoachTraining.Domain.Entities;

public class CredencialWearable
{
    public CredencialWearable(
        Guid conexaoWearableId,
        string accessTokenProtegido,
        string refreshTokenProtegido,
        DateTime expiresAtUtc,
        DateTime atualizadoEmUtc,
        Guid? id = null)
    {
        if (conexaoWearableId == Guid.Empty)
        {
            throw new ArgumentException("ConexaoWearableId obrigatorio.", nameof(conexaoWearableId));
        }

        if (string.IsNullOrWhiteSpace(accessTokenProtegido))
        {
            throw new ArgumentException("Access token protegido obrigatorio.", nameof(accessTokenProtegido));
        }

        if (string.IsNullOrWhiteSpace(refreshTokenProtegido))
        {
            throw new ArgumentException("Refresh token protegido obrigatorio.", nameof(refreshTokenProtegido));
        }

        Id = id ?? Guid.NewGuid();
        ConexaoWearableId = conexaoWearableId;
        AccessTokenProtegido = accessTokenProtegido.Trim();
        RefreshTokenProtegido = refreshTokenProtegido.Trim();
        ExpiresAtUtc = expiresAtUtc;
        AtualizadoEmUtc = atualizadoEmUtc;
    }

    public Guid Id { get; private set; }
    public Guid ConexaoWearableId { get; private set; }
    public string AccessTokenProtegido { get; private set; }
    public string RefreshTokenProtegido { get; private set; }
    public DateTime ExpiresAtUtc { get; private set; }
    public DateTime AtualizadoEmUtc { get; private set; }
}
