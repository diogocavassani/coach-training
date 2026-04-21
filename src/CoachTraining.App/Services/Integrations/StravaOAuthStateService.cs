using System.Text.Json;
using CoachTraining.App.Abstractions.Security;

namespace CoachTraining.App.Services.Integrations;

public class StravaOAuthStateService
{
    private readonly ISecretProtector _secretProtector;

    public StravaOAuthStateService(ISecretProtector secretProtector)
    {
        _secretProtector = secretProtector ?? throw new ArgumentNullException(nameof(secretProtector));
    }

    public string Proteger(StravaOAuthState state)
    {
        return _secretProtector.Protect(JsonSerializer.Serialize(state));
    }

    public StravaOAuthState? Desproteger(string? stateProtegido)
    {
        if (string.IsNullOrWhiteSpace(stateProtegido))
        {
            return null;
        }

        try
        {
            var json = _secretProtector.Unprotect(stateProtegido);
            return JsonSerializer.Deserialize<StravaOAuthState>(json);
        }
        catch
        {
            return null;
        }
    }
}
