using System.Net;
using System.Net.Http.Json;
using CoachTraining.App.DTOs.Integrations;

namespace CoachTraining.Tests.Api;

public class PublicIntegracoesApiIntegrationTests : IClassFixture<ApiWebApplicationFactory>
{
    private readonly ApiWebApplicationFactory _factory;

    public PublicIntegracoesApiIntegrationTests(ApiWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GetPublicIntegracoes_DeveRetornarProvedoresSemExporDadosDoAtleta()
    {
        using var client = _factory.CreateClient();
        var professor = await ApiTestData.CadastrarProfessorAsync(client, $"prof.publico.{Guid.NewGuid():N}@teste.com");
        var token = await ApiTestData.LoginAsync(client, professor.Email, "123456");
        var atleta = await ApiTestData.CadastrarAtletaAsync(
            client,
            token,
            "Atleta Publico",
            email: "atleta.publico@teste.com",
            observacoesClinicas: "Nao deve aparecer");
        var link = await ApiTestData.GerarLinkIntegracaoAsync(client, token, atleta.Id);

        var response = await client.GetAsync($"/public/integracoes/{link.TokenPublico}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var payload = await response.Content.ReadFromJsonAsync<PublicIntegrationPageDto>();
        Assert.NotNull(payload);
        Assert.Equal("Conectar aplicativos ao CoachTraining", payload!.Titulo);
        Assert.DoesNotContain("@", payload.Descricao);
        Assert.DoesNotContain("Nao deve aparecer", payload.Descricao);
        Assert.Contains(payload.Provedores, p => p.ProviderKey == "strava");
    }

    [Fact]
    public async Task GetPublicIntegracoes_ComTokenInvalido_DeveRetornarNotFound()
    {
        using var client = _factory.CreateClient();

        var response = await client.GetAsync("/public/integracoes/token-invalido");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
