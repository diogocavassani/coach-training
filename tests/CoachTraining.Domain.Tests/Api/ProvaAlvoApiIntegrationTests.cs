using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using CoachTraining.App.DTOs;

namespace CoachTraining.Tests.Api;

public class ProvaAlvoApiIntegrationTests : IClassFixture<ApiWebApplicationFactory>
{
    private readonly ApiWebApplicationFactory _factory;

    public ProvaAlvoApiIntegrationTests(ApiWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task ProfessorAutenticado_DeveSalvarEConsultarProvaAlvoDoAtleta()
    {
        using var client = _factory.CreateClient();
        var professor = await CadastrarProfessorAsync(client, $"professor.prova.{Guid.NewGuid():N}@teste.com");
        var token = await LoginAsync(client, professor.Email, "123456");
        var atleta = await CadastrarAtletaAsync(client, token, "Atleta Prova");

        var payload = new
        {
            dataProva = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(28),
            distanciaKm = 21.1,
            objetivo = "Completar forte"
        };

        var salvarRequest = new HttpRequestMessage(HttpMethod.Put, $"/api/atleta/{atleta.Id}/prova-alvo")
        {
            Content = JsonContent.Create(payload)
        };
        salvarRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var salvarResponse = await client.SendAsync(salvarRequest);

        Assert.Equal(HttpStatusCode.OK, salvarResponse.StatusCode);

        var consultarRequest = new HttpRequestMessage(HttpMethod.Get, $"/api/atleta/{atleta.Id}/prova-alvo");
        consultarRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var consultarResponse = await client.SendAsync(consultarRequest);

        Assert.Equal(HttpStatusCode.OK, consultarResponse.StatusCode);

        var prova = await consultarResponse.Content.ReadFromJsonAsync<ProvaAlvoDto>();
        Assert.NotNull(prova);
        Assert.Equal(atleta.Id, prova!.AtletaId);
        Assert.Equal(payload.dataProva, prova.DataProva);
        Assert.Equal(payload.distanciaKm, prova.DistanciaKm);
        Assert.Equal(payload.objetivo, prova.Objetivo);
    }

    [Fact]
    public async Task ProfessorNaoDono_DoAtletaNaoDeveSalvarProvaAlvo()
    {
        using var client = _factory.CreateClient();
        var professorA = await CadastrarProfessorAsync(client, $"professor.a.prova.{Guid.NewGuid():N}@teste.com");
        var professorB = await CadastrarProfessorAsync(client, $"professor.b.prova.{Guid.NewGuid():N}@teste.com");
        var tokenA = await LoginAsync(client, professorA.Email, "123456");
        var tokenB = await LoginAsync(client, professorB.Email, "123456");
        var atletaA = await CadastrarAtletaAsync(client, tokenA, "Atleta Professor A");

        var salvarRequest = new HttpRequestMessage(HttpMethod.Put, $"/api/atleta/{atletaA.Id}/prova-alvo")
        {
            Content = JsonContent.Create(new
            {
                dataProva = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(10),
                distanciaKm = 10.0,
                objetivo = "Prova indevida"
            })
        };
        salvarRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenB);

        var salvarResponse = await client.SendAsync(salvarRequest);

        Assert.Equal(HttpStatusCode.Forbidden, salvarResponse.StatusCode);
    }

    private static async Task<ProfessorDto> CadastrarProfessorAsync(HttpClient client, string email)
    {
        var response = await client.PostAsJsonAsync("/professores", new CriarProfessorDto
        {
            Nome = $"Professor {Guid.NewGuid():N}",
            Email = email,
            Senha = "123456"
        });

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<ProfessorDto>()
            ?? throw new InvalidOperationException("Resposta de cadastro de professor sem corpo.");
    }

    private static async Task<string> LoginAsync(HttpClient client, string email, string senha)
    {
        var response = await client.PostAsJsonAsync("/auth/login", new LoginDto
        {
            Email = email,
            Senha = senha
        });

        response.EnsureSuccessStatusCode();
        var dto = await response.Content.ReadFromJsonAsync<LoginResponseDto>();
        return dto?.Token ?? throw new InvalidOperationException("Resposta de login sem token.");
    }

    private static async Task<AtletaDto> CadastrarAtletaAsync(HttpClient client, string token, string nome)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/atleta")
        {
            Content = JsonContent.Create(new CriarAtletaDto
            {
                Nome = nome
            })
        };
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<AtletaDto>()
            ?? throw new InvalidOperationException("Resposta de cadastro de atleta sem corpo.");
    }
}
