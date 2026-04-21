using CoachTraining.Domain.Enums;

namespace CoachTraining.Domain.Entities;

public class AtividadeImportada
{
    public AtividadeImportada(
        ProvedorIntegracao provedor,
        Guid conexaoWearableId,
        string externalActivityId,
        Guid sessaoDeTreinoId,
        DateTime importadoEmUtc,
        string? payloadResumoJson = null,
        Guid? id = null)
    {
        if (conexaoWearableId == Guid.Empty)
        {
            throw new ArgumentException("ConexaoWearableId obrigatorio.", nameof(conexaoWearableId));
        }

        if (sessaoDeTreinoId == Guid.Empty)
        {
            throw new ArgumentException("SessaoDeTreinoId obrigatorio.", nameof(sessaoDeTreinoId));
        }

        if (string.IsNullOrWhiteSpace(externalActivityId))
        {
            throw new ArgumentException("ExternalActivityId obrigatorio.", nameof(externalActivityId));
        }

        Id = id ?? Guid.NewGuid();
        Provedor = provedor;
        ConexaoWearableId = conexaoWearableId;
        ExternalActivityId = externalActivityId.Trim();
        SessaoDeTreinoId = sessaoDeTreinoId;
        ImportadoEmUtc = importadoEmUtc;
        PayloadResumoJson = payloadResumoJson;
    }

    public Guid Id { get; private set; }
    public ProvedorIntegracao Provedor { get; private set; }
    public Guid ConexaoWearableId { get; private set; }
    public string ExternalActivityId { get; private set; }
    public Guid SessaoDeTreinoId { get; private set; }
    public DateTime ImportadoEmUtc { get; private set; }
    public string? PayloadResumoJson { get; private set; }
}
