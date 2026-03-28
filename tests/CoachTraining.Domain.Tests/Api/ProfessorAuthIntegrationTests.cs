using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using CoachTraining.App.DTOs;

namespace CoachTraining.Tests.Api;

public class ProfessorAuthIntegrationTests : IClassFixture<ApiWebApplicationFactory>
{
    private readonly ApiWebApplicationFactory _factory;

    public ProfessorAuthIntegrationTests(ApiWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task CadastroProfessor_DeveRetornarCreated_EEmailDuplicadoDeveRetornarConflict()
    {
        using var client = _factory.CreateClient();
        var request = new CriarProfessorDto
        {
            Nome = "Professor Teste",
            Email = "professor.duplicate@teste.com",
            Senha = "123456"
        };

        var primeiraResposta = await client.PostAsJsonAsync("/professores", request);
        var segundaResposta = await client.PostAsJsonAsync("/professores", request);

        Assert.Equal(HttpStatusCode.Created, primeiraResposta.StatusCode);
        Assert.Equal(HttpStatusCode.Conflict, segundaResposta.StatusCode);
    }

    [Fact]
    public async Task Login_DeveRetornarToken_QuandoCredenciaisValidas_EUnauthorizedQuandoInvalidas()
    {
        using var client = _factory.CreateClient();
        var professor = await CadastrarProfessorAsync(client, "professor.login@teste.com");

        var loginSucesso = await client.PostAsJsonAsync("/auth/login", new LoginDto
        {
            Email = professor.Email,
            Senha = "123456"
        });

        var loginFalha = await client.PostAsJsonAsync("/auth/login", new LoginDto
        {
            Email = professor.Email,
            Senha = "senha-errada"
        });

        Assert.Equal(HttpStatusCode.OK, loginSucesso.StatusCode);
        Assert.Equal(HttpStatusCode.Unauthorized, loginFalha.StatusCode);

        var loginResponse = await loginSucesso.Content.ReadFromJsonAsync<LoginResponseDto>();
        Assert.NotNull(loginResponse);
        Assert.False(string.IsNullOrWhiteSpace(loginResponse!.Token));
        Assert.True(loginResponse.ExpiraEmUtc > DateTime.UtcNow);
    }

    [Fact]
    public async Task Login_DeveGerarTokenComClaimProfessorId()
    {
        using var client = _factory.CreateClient();
        var professor = await CadastrarProfessorAsync(client, "professor.claim@teste.com");
        var login = await client.PostAsJsonAsync("/auth/login", new LoginDto
        {
            Email = professor.Email,
            Senha = "123456"
        });

        login.EnsureSuccessStatusCode();
        var response = await login.Content.ReadFromJsonAsync<LoginResponseDto>();
        Assert.NotNull(response);

        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(response!.Token);
        var claimProfessorId = jwt.Claims.FirstOrDefault(c => c.Type == "professor_id")?.Value;

        Assert.Equal(professor.Id.ToString(), claimProfessorId);
    }

    [Fact]
    public async Task EndpointsProtegidosDevemRetornarUnauthorized_SemToken()
    {
        using var client = _factory.CreateClient();
        var response = await client.PostAsJsonAsync("/api/atleta", new CriarAtletaDto
        {
            Nome = "Atleta sem token"
        });

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Isolamento_DeveImpedirProfessorBAcessarDadosDoProfessorA()
    {
        using var client = _factory.CreateClient();
        var professorA = await CadastrarProfessorAsync(client, "professor.a@teste.com");
        var professorB = await CadastrarProfessorAsync(client, "professor.b@teste.com");

        var tokenA = await LoginAsync(client, professorA.Email, "123456");
        var tokenB = await LoginAsync(client, professorB.Email, "123456");

        var cadastrarAtletaRequest = new HttpRequestMessage(HttpMethod.Post, "/api/atleta")
        {
            Content = JsonContent.Create(new CriarAtletaDto
            {
                Nome = "Atleta do Professor A",
                NivelEsportivo = "Intermediario"
            })
        };
        cadastrarAtletaRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenA);

        var cadastroAtletaResponse = await client.SendAsync(cadastrarAtletaRequest);
        Assert.Equal(HttpStatusCode.Created, cadastroAtletaResponse.StatusCode);

        var atletaA = await cadastroAtletaResponse.Content.ReadFromJsonAsync<AtletaDto>();
        Assert.NotNull(atletaA);

        var obterAtletaProfessorA = new HttpRequestMessage(HttpMethod.Get, $"/api/atleta/{atletaA!.Id}");
        obterAtletaProfessorA.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenA);
        var respostaProfessorA = await client.SendAsync(obterAtletaProfessorA);
        Assert.Equal(HttpStatusCode.OK, respostaProfessorA.StatusCode);

        var obterAtletaProfessorB = new HttpRequestMessage(HttpMethod.Get, $"/api/atleta/{atletaA.Id}");
        obterAtletaProfessorB.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenB);
        var respostaProfessorB = await client.SendAsync(obterAtletaProfessorB);
        Assert.Equal(HttpStatusCode.NotFound, respostaProfessorB.StatusCode);

        var dashboardProfessorB = new HttpRequestMessage(HttpMethod.Get, $"/api/dashboard/atleta/{atletaA.Id}");
        dashboardProfessorB.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenB);
        var dashboardResponse = await client.SendAsync(dashboardProfessorB);
        Assert.Equal(HttpStatusCode.NotFound, dashboardResponse.StatusCode);
    }

    private static async Task<ProfessorDto> CadastrarProfessorAsync(HttpClient client, string email)
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

    private static async Task<string> LoginAsync(HttpClient client, string email, string senha)
    {
        var response = await client.PostAsJsonAsync("/auth/login", new LoginDto
        {
            Email = email,
            Senha = senha
        });

        response.EnsureSuccessStatusCode();
        var dto = await response.Content.ReadFromJsonAsync<LoginResponseDto>();
        if (dto == null || string.IsNullOrWhiteSpace(dto.Token))
        {
            throw new InvalidOperationException("Resposta de login sem token.");
        }

        return dto.Token;
    }
}
