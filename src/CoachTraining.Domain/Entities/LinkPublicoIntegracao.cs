namespace CoachTraining.Domain.Entities;

public class LinkPublicoIntegracao
{
    private LinkPublicoIntegracao(
        Guid atletaId,
        string tokenHash,
        DateTime criadoEmUtc,
        Guid? id = null)
    {
        if (atletaId == Guid.Empty)
        {
            throw new ArgumentException("AtletaId obrigatorio.", nameof(atletaId));
        }

        AtletaId = atletaId;
        TokenHash = ValidarTokenHash(tokenHash);
        CriadoEmUtc = criadoEmUtc;
        Id = id ?? Guid.NewGuid();
        Ativo = true;
    }

    public Guid Id { get; private set; }
    public Guid AtletaId { get; private set; }
    public string TokenHash { get; private set; }
    public bool Ativo { get; private set; }
    public DateTime CriadoEmUtc { get; private set; }
    public DateTime? RegeneradoEmUtc { get; private set; }
    public DateTime? RevogadoEmUtc { get; private set; }

    public static LinkPublicoIntegracao Criar(Guid atletaId, string tokenHash)
        => new(atletaId, tokenHash, DateTime.UtcNow);

    public void Regenerar(string novoTokenHash, DateTime quando)
    {
        TokenHash = ValidarTokenHash(novoTokenHash);
        Ativo = true;
        RegeneradoEmUtc = quando;
        RevogadoEmUtc = null;
    }

    public void Revogar(DateTime quando)
    {
        Ativo = false;
        RevogadoEmUtc = quando;
    }

    private static string ValidarTokenHash(string tokenHash)
    {
        if (string.IsNullOrWhiteSpace(tokenHash))
        {
            throw new ArgumentException("Token hash obrigatorio.", nameof(tokenHash));
        }

        return tokenHash.Trim();
    }
}
