namespace CoachTraining.App.DTOs.Integrations;

public class LinkPublicoIntegracaoDto
{
    public Guid AtletaId { get; set; }
    public string TokenPublico { get; set; } = string.Empty;
    public string UrlPublica { get; set; } = string.Empty;
}
