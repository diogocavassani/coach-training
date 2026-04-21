namespace CoachTraining.App.DTOs.Integrations;

public class PublicIntegrationPageDto
{
    public string Titulo { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public IReadOnlyList<ProviderIntegrationStatusDto> Provedores { get; set; } = [];
}
