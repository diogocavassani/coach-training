using System.Net.Http.Headers;
using System.Net.Http.Json;
using CoachTraining.App.DTOs;
using CoachTraining.App.DTOs.Integrations;

namespace CoachTraining.Tests.Api;

internal static class ApiTestData
{
    public static async Task<ProfessorDto> CadastrarProfessorAsync(HttpClient client, string email)
    {
        var response = await client.PostAsJsonAsync("/professores", new CriarProfessorDto
        {
            Nome = $"Nome {Guid.NewGuid():N}",
            Email = email,
            Senha = "123456"
        });

        response.EnsureSuccessStatusCode();
        var dto = await response.Content.ReadFromJsonAsync<ProfessorDto>();
        return dto ?? throw new InvalidOperationException("Resposta de cadastro sem corpo.");
    }

    public static async Task<string> LoginAsync(HttpClient client, string email, string senha)
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

    public static async Task<AtletaDto> CadastrarAtletaAsync(
        HttpClient client,
        string token,
        string nome,
        string? email = null,
        string? observacoesClinicas = null,
        string? nivelEsportivo = "Intermediario")
    {
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/atleta")
        {
            Content = JsonContent.Create(new CriarAtletaDto
            {
                Nome = nome,
                Email = email,
                ObservacoesClinicas = observacoesClinicas,
                NivelEsportivo = nivelEsportivo
            })
        };
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var atleta = await response.Content.ReadFromJsonAsync<AtletaDto>();
        return atleta ?? throw new InvalidOperationException("Resposta de cadastro de atleta sem corpo.");
    }

    public static async Task<LinkPublicoIntegracaoDto> GerarLinkIntegracaoAsync(HttpClient client, string token, Guid atletaId)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"/api/atletas/{atletaId}/integracoes/link");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var dto = await response.Content.ReadFromJsonAsync<LinkPublicoIntegracaoDto>();
        return dto ?? throw new InvalidOperationException("Resposta de link de integracao sem corpo.");
    }
}
