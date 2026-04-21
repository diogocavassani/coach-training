namespace CoachTraining.Infra.Persistence.Models;

public class AtividadeImportadaModel
{
    public Guid Id { get; set; }
    public int Provedor { get; set; }
    public Guid ConexaoWearableId { get; set; }
    public string ExternalActivityId { get; set; } = string.Empty;
    public Guid SessaoDeTreinoId { get; set; }
    public DateTime ImportadoEmUtc { get; set; }
    public string? PayloadResumoJson { get; set; }
    public ConexaoWearableModel ConexaoWearable { get; set; } = null!;
}
