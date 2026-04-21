using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using CoachTraining.App.DTOs;
using CoachTraining.App.DTOs.Integrations;

namespace CoachTraining.Tests.Api;

public class AtletaIntegracoesApiIntegrationTests : IClassFixture<ApiWebApplicationFactory>
{
    private readonly ApiWebApplicationFactory _factory;

    public AtletaIntegracoesApiIntegrationTests(ApiWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task PostApiAtletasIntegracoesLink_DeveRetornarLinkPublicoParaProfessorAutenticado()
    {
        using var client = _factory.CreateClient();
        var professor = await ApiTestData.CadastrarProfessorAsync(client, $"prof.integracao.{Guid.NewGuid():N}@teste.com");
        var token = await ApiTestData.LoginAsync(client, professor.Email, "123456");
        var atleta = await ApiTestData.CadastrarAtletaAsync(client, token, "Atleta Integracao");

        var request = new HttpRequestMessage(HttpMethod.Post, $"/api/atletas/{atleta.Id}/integracoes/link");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await client.SendAsync(request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var payload = await response.Content.ReadFromJsonAsync<LinkPublicoIntegracaoDto>();
        Assert.NotNull(payload);
        Assert.Contains("/conectar/", payload!.UrlPublica);
        Assert.False(string.IsNullOrWhiteSpace(payload.TokenPublico));
    }

    [Fact]
    public async Task GetApiAtletasIntegracoes_DeveRetornarStatusDoProvedorELinkAtual()
    {
        using var client = _factory.CreateClient();
        var professor = await ApiTestData.CadastrarProfessorAsync(client, $"prof.integracao.list.{Guid.NewGuid():N}@teste.com");
        var token = await ApiTestData.LoginAsync(client, professor.Email, "123456");
        var atleta = await ApiTestData.CadastrarAtletaAsync(client, token, "Atleta Integracao");
        await ApiTestData.GerarLinkIntegracaoAsync(client, token, atleta.Id);

        var request = new HttpRequestMessage(HttpMethod.Get, $"/api/atletas/{atleta.Id}/integracoes");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await client.SendAsync(request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var payload = await response.Content.ReadFromJsonAsync<AtletaIntegracoesResumoDto>();
        Assert.NotNull(payload);
        Assert.Contains("/conectar/", payload!.UrlPublicaAtual);
        Assert.Contains(payload.Provedores, p => p.ProviderKey == "strava");
    }
}
