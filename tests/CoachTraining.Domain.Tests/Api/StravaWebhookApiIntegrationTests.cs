using System.Net;
using System.Net.Http.Json;

namespace CoachTraining.Tests.Api;

public class StravaWebhookApiIntegrationTests : IClassFixture<ApiWebApplicationFactory>
{
    private readonly ApiWebApplicationFactory _factory;

    public StravaWebhookApiIntegrationTests(ApiWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GetWebhookHandshake_DeveEcoarHubChallenge()
    {
        using var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/integrations/strava/webhook/strava-hook-secret?hub.verify_token=strava-verify-token&hub.challenge=abc123&hub.mode=subscribe");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var payload = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
        Assert.NotNull(payload);
        Assert.Equal("abc123", payload!["hub.challenge"]);
    }

    [Fact]
    public async Task PostWebhook_DeveResponderOkRapidamente()
    {
        using var client = _factory.CreateClient();

        var response = await client.PostAsJsonAsync(
            "/api/integrations/strava/webhook/strava-hook-secret",
            new
            {
                object_type = "activity",
                object_id = 12345,
                aspect_type = "create",
                owner_id = 987,
                updates = new { }
            });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
