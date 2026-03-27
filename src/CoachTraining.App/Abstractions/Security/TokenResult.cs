namespace CoachTraining.App.Abstractions.Security;

public sealed class TokenResult
{
    public string Token { get; init; } = string.Empty;
    public DateTime ExpiraEmUtc { get; init; }
}
