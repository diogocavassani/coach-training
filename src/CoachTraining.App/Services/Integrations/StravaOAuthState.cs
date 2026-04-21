namespace CoachTraining.App.Services.Integrations;

public sealed record StravaOAuthState(string TokenHash, DateTime ExpiresAtUtc, string Nonce);
