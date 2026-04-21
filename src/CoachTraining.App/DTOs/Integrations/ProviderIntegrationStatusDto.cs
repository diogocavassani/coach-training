namespace CoachTraining.App.DTOs.Integrations;

public class ProviderIntegrationStatusDto
{
    public string ProviderKey { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public bool Enabled { get; set; }
    public DateTime? LastSyncAtUtc { get; set; }
}
