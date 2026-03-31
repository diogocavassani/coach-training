using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using CoachTraining.App.DTOs;
using CoachTraining.Domain.Enums;

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

    [Fact]
    public async Task ListagemDeAtletas_DeveRetornarSomenteAtletasDoProfessorAutenticado()
    {
        using var client = _factory.CreateClient();
        var professorA = await CadastrarProfessorAsync(client, "professor.list.a@teste.com");
        var professorB = await CadastrarProfessorAsync(client, "professor.list.b@teste.com");

        var tokenA = await LoginAsync(client, professorA.Email, "123456");
        var tokenB = await LoginAsync(client, professorB.Email, "123456");

        var atletaA = await CadastrarAtletaAsync(
            client,
            tokenA,
            "Atleta Professor A",
            email: "atleta.a@teste.com",
            observacoesClinicas: "Sem restricoes",
            nivelEsportivo: "Intermediario");
        await CadastrarAtletaAsync(
            client,
            tokenB,
            "Atleta Professor B",
            email: "atleta.b@teste.com",
            observacoesClinicas: "Com acompanhamento",
            nivelEsportivo: "Avancado");

        var listarAtletasRequest = new HttpRequestMessage(HttpMethod.Get, "/api/atleta");
        listarAtletasRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenA);

        var response = await client.SendAsync(listarAtletasRequest);
        response.EnsureSuccessStatusCode();

        var atletas = await response.Content.ReadFromJsonAsync<List<AtletaDto>>();
        Assert.NotNull(atletas);
        Assert.Single(atletas!);
        Assert.Equal(atletaA.Id, atletas[0].Id);
        Assert.Equal(professorA.Id, atletas[0].ProfessorId);
        Assert.Equal("atleta.a@teste.com", atletas[0].Email);
        Assert.Equal("Sem restricoes", atletas[0].ObservacoesClinicas);
        Assert.Equal("Intermediario", atletas[0].NivelEsportivo);
    }

    [Fact]
    public async Task DashboardAtleta_DeveRetornarPayloadCompleto_ParaProfessorDonoDoAtleta()
    {
        using var client = _factory.CreateClient();
        var professor = await CadastrarProfessorAsync(client, $"professor.dashboard.{Guid.NewGuid():N}@teste.com");
        var token = await LoginAsync(client, professor.Email, "123456");
        var atleta = await CadastrarAtletaAsync(client, token, "Atleta Dashboard", email: "atleta.dashboard@teste.com");

        await CadastrarTreinoAsync(
            client,
            token,
            atleta.Id,
            DateOnly.FromDateTime(DateTime.UtcNow).AddDays(-2),
            TipoDeTreino.Ritmo,
            50,
            8.0,
            6);

        await CadastrarTreinoAsync(
            client,
            token,
            atleta.Id,
            DateOnly.FromDateTime(DateTime.UtcNow).AddDays(-9),
            TipoDeTreino.Longo,
            90,
            15.0,
            7);

        var request = new HttpRequestMessage(HttpMethod.Get, $"/api/dashboard/atleta/{atleta.Id}");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await client.SendAsync(request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var dashboard = await response.Content.ReadFromJsonAsync<DashboardAtletaDto>();
        Assert.NotNull(dashboard);
        Assert.Equal(atleta.Id, dashboard!.AtletaId);
        Assert.Equal(12, dashboard.SerieCargaSemanal.Count);
        Assert.Equal(12, dashboard.SeriePaceSemanal.Count);
        Assert.True(dashboard.TreinosJanela.Count >= 2);
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

    private static async Task<AtletaDto> CadastrarAtletaAsync(
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

    private static async Task CadastrarTreinoAsync(
        HttpClient client,
        string token,
        Guid atletaId,
        DateOnly data,
        TipoDeTreino tipo,
        int duracaoMinutos,
        double distanciaKm,
        int rpe)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/treinos")
        {
            Content = JsonContent.Create(new CadastrarSessaoDeTreinoDto
            {
                AtletaId = atletaId,
                Data = data,
                Tipo = tipo,
                DuracaoMinutos = duracaoMinutos,
                DistanciaKm = distanciaKm,
                Rpe = rpe
            })
        };
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();
    }
}
