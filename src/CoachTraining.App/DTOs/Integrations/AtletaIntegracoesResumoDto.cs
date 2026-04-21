namespace CoachTraining.App.DTOs.Integrations;

public class AtletaIntegracoesResumoDto
{
    public Guid AtletaId { get; set; }
    public string? TokenPublicoAtual { get; set; }
    public string? UrlPublicaAtual { get; set; }
    public IReadOnlyList<ProviderIntegrationStatusDto> Provedores { get; set; } = [];
}
