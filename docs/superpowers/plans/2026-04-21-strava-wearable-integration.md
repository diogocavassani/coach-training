# Strava Wearable Integration Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Build the reusable public wearable connection flow, secure Strava OAuth integration, webhook-driven automatic activity import, teacher UI, public student UI, and implementation docs.

**Architecture:** The backend keeps training sessions as the canonical domain model and introduces a generic integration module around public connection links, provider connections, protected credentials, and imported activity tracking. Strava is the first `IWearableProvider` implementation, with webhook processing split from HTTP acknowledgement so retries do not create duplicate sessions. The frontend adds a teacher-facing integration panel inside the authenticated athlete area plus a public flow under `/conectar/*` that works without login.

**Tech Stack:** ASP.NET Core 10, C# 12, EF Core, xUnit, Angular 19, Angular Material

---

## File Structure

### Backend domain and application

- Create: `src/CoachTraining.Domain/Enums/OrigemTreino.cs`
- Create: `src/CoachTraining.Domain/Enums/ProvedorIntegracao.cs`
- Create: `src/CoachTraining.Domain/Enums/StatusConexaoIntegracao.cs`
- Create: `src/CoachTraining.Domain/Entities/LinkPublicoIntegracao.cs`
- Create: `src/CoachTraining.Domain/Entities/ConexaoWearable.cs`
- Create: `src/CoachTraining.Domain/Entities/CredencialWearable.cs`
- Create: `src/CoachTraining.Domain/Entities/EventoWebhookRecebido.cs`
- Create: `src/CoachTraining.Domain/Entities/AtividadeImportada.cs`
- Modify: `src/CoachTraining.Domain/Entities/SessaoDeTreino.cs`
- Create: `src/CoachTraining.App/Abstractions/Persistence/ILinkPublicoIntegracaoRepository.cs`
- Create: `src/CoachTraining.App/Abstractions/Persistence/IConexaoWearableRepository.cs`
- Create: `src/CoachTraining.App/Abstractions/Persistence/IEventoWebhookRepository.cs`
- Create: `src/CoachTraining.App/Abstractions/Persistence/IAtividadeImportadaRepository.cs`
- Create: `src/CoachTraining.App/Abstractions/Integrations/IWearableProvider.cs`
- Create: `src/CoachTraining.App/Abstractions/Integrations/IWearableProviderRegistry.cs`
- Create: `src/CoachTraining.App/Abstractions/Security/ISecretProtector.cs`
- Create: `src/CoachTraining.App/DTOs/Integrations/*.cs`
- Create: `src/CoachTraining.App/Services/Integrations/*.cs`

### Infrastructure

- Create: `src/CoachTraining.Infra/Integrations/Strava/*.cs`
- Create: `src/CoachTraining.Infra/Persistence/Models/LinkPublicoIntegracaoModel.cs`
- Create: `src/CoachTraining.Infra/Persistence/Models/ConexaoWearableModel.cs`
- Create: `src/CoachTraining.Infra/Persistence/Models/CredencialWearableModel.cs`
- Create: `src/CoachTraining.Infra/Persistence/Models/EventoWebhookRecebidoModel.cs`
- Create: `src/CoachTraining.Infra/Persistence/Models/AtividadeImportadaModel.cs`
- Create: `src/CoachTraining.Infra/Persistence/Repositories/*Integracao*.cs`
- Modify: `src/CoachTraining.Infra/Persistence/Models/SessaoDeTreinoModel.cs`
- Modify: `src/CoachTraining.Infra/Persistence/CoachTrainingDbContext.cs`
- Create: `src/CoachTraining.Infra/Persistence/Migrations/*StravaWearableIntegration*.cs`
- Modify: `src/CoachTraining.Infra/DependencyInjection.cs`

### API

- Create: `src/CoachTraining.Api/Controllers/AtletaIntegracoesController.cs`
- Create: `src/CoachTraining.Api/Controllers/PublicIntegracoesController.cs`
- Create: `src/CoachTraining.Api/Controllers/StravaWebhookController.cs`
- Modify: `src/CoachTraining.Api/Program.cs`
- Modify: `src/CoachTraining.Api/appsettings.json`
- Modify: `src/CoachTraining.Api/appsettings.Development.json`

### Frontend

- Create: `frontend/src/app/features/integrations/models/integration.model.ts`
- Create: `frontend/src/app/features/integrations/services/teacher-integrations-api.service.ts`
- Create: `frontend/src/app/features/integrations/services/public-integrations-api.service.ts`
- Create: `frontend/src/app/features/integrations/components/student-integrations-panel.component.ts`
- Create: `frontend/src/app/features/integrations/components/student-integrations-panel.component.html`
- Create: `frontend/src/app/features/integrations/components/student-integrations-panel.component.css`
- Create: `frontend/src/app/features/integrations/pages/public-integration-page.component.ts`
- Create: `frontend/src/app/features/integrations/pages/public-integration-page.component.html`
- Create: `frontend/src/app/features/integrations/pages/public-integration-page.component.css`
- Create: `frontend/src/app/features/integrations/pages/strava-callback-page.component.ts`
- Create: `frontend/src/app/features/integrations/pages/strava-callback-page.component.html`
- Create: `frontend/src/app/features/integrations/pages/strava-callback-page.component.css`
- Modify: `frontend/src/app/app.routes.ts`
- Modify: `frontend/src/app/features/dashboard/pages/student-dashboard-page.component.ts`
- Modify: `frontend/src/app/features/dashboard/pages/student-dashboard-page.component.html`

### Tests

- Create: `tests/CoachTraining.Domain.Tests/Domain/Unit/LinkPublicoIntegracaoTests.cs`
- Create: `tests/CoachTraining.Domain.Tests/Domain/Unit/ConexaoWearableTests.cs`
- Create: `tests/CoachTraining.Domain.Tests/App/Services/GerarLinkPublicoIntegracaoServiceTests.cs`
- Create: `tests/CoachTraining.Domain.Tests/App/Services/IniciarAutorizacaoStravaServiceTests.cs`
- Create: `tests/CoachTraining.Domain.Tests/App/Services/ProcessarEventoStravaServiceTests.cs`
- Create: `tests/CoachTraining.Domain.Tests/Api/AtletaIntegracoesApiIntegrationTests.cs`
- Create: `tests/CoachTraining.Domain.Tests/Api/PublicIntegracoesApiIntegrationTests.cs`
- Create: `tests/CoachTraining.Domain.Tests/Api/StravaWebhookApiIntegrationTests.cs`
- Modify: `frontend/src/app/app.routes.spec.ts`
- Create: `frontend/src/app/features/integrations/services/teacher-integrations-api.service.spec.ts`
- Create: `frontend/src/app/features/integrations/services/public-integrations-api.service.spec.ts`
- Create: `frontend/src/app/features/integrations/pages/public-integration-page.component.spec.ts`
- Create: `frontend/src/app/features/integrations/pages/strava-callback-page.component.spec.ts`

### Documentation

- Create: `docs/apis/integracoes-wearables-api.md`
- Create: `docs/flows/fluxo-integracao-strava.md`
- Modify: `docs/architecture/overview.md`

## Task 1: Domain and persistence foundation

**Files:**
- Create: `src/CoachTraining.Domain/Enums/OrigemTreino.cs`
- Create: `src/CoachTraining.Domain/Enums/ProvedorIntegracao.cs`
- Create: `src/CoachTraining.Domain/Enums/StatusConexaoIntegracao.cs`
- Create: `src/CoachTraining.Domain/Entities/LinkPublicoIntegracao.cs`
- Create: `src/CoachTraining.Domain/Entities/ConexaoWearable.cs`
- Create: `src/CoachTraining.Domain/Entities/CredencialWearable.cs`
- Create: `src/CoachTraining.Domain/Entities/EventoWebhookRecebido.cs`
- Create: `src/CoachTraining.Domain/Entities/AtividadeImportada.cs`
- Modify: `src/CoachTraining.Domain/Entities/SessaoDeTreino.cs`
- Create: `src/CoachTraining.Infra/Persistence/Models/LinkPublicoIntegracaoModel.cs`
- Create: `src/CoachTraining.Infra/Persistence/Models/ConexaoWearableModel.cs`
- Create: `src/CoachTraining.Infra/Persistence/Models/CredencialWearableModel.cs`
- Create: `src/CoachTraining.Infra/Persistence/Models/EventoWebhookRecebidoModel.cs`
- Create: `src/CoachTraining.Infra/Persistence/Models/AtividadeImportadaModel.cs`
- Modify: `src/CoachTraining.Infra/Persistence/Models/SessaoDeTreinoModel.cs`
- Modify: `src/CoachTraining.Infra/Persistence/CoachTrainingDbContext.cs`
- Create: `src/CoachTraining.Infra/Persistence/Migrations/*StravaWearableIntegration*.cs`
- Test: `tests/CoachTraining.Domain.Tests/Domain/Unit/LinkPublicoIntegracaoTests.cs`
- Test: `tests/CoachTraining.Domain.Tests/Domain/Unit/ConexaoWearableTests.cs`
- Test: `tests/CoachTraining.Domain.Tests/Domain/Unit/SessaoDeTreinoTests.cs`

- [ ] **Step 1: Write the failing domain tests**

```csharp
[Fact]
public void RegenerarToken_DeveInvalidarTokenAnteriorEManterNovoLinkAtivo()
{
    var link = LinkPublicoIntegracao.Criar(Guid.NewGuid(), "hash-antigo");

    link.Regenerar("hash-novo", DateTime.UtcNow);

    Assert.Equal("hash-novo", link.TokenHash);
    Assert.True(link.Ativo);
    Assert.NotNull(link.RegeneradoEmUtc);
}

[Fact]
public void RegistrarImportacao_DevePermitirSomenteUmaAtividadePorProvedorEExternalId()
{
    var atividade = new AtividadeImportada(
        provedor: ProvedorIntegracao.Strava,
        conexaoWearableId: Guid.NewGuid(),
        externalActivityId: "12345",
        sessaoDeTreinoId: Guid.NewGuid(),
        importadoEmUtc: DateTime.UtcNow);

    Assert.Equal(ProvedorIntegracao.Strava, atividade.Provedor);
    Assert.Equal("12345", atividade.ExternalActivityId);
}

[Fact]
public void CriarSessaoImportada_DeveRegistrarOrigemDoTreino()
{
    var sessao = new SessaoDeTreino(
        atletaId: Guid.NewGuid(),
        data: DateOnly.FromDateTime(DateTime.UtcNow),
        tipo: TipoDeTreino.Leve,
        duracaoMinutos: 45,
        distanciaKm: 8,
        rpe: new RPE(5),
        origem: OrigemTreino.Strava);

    Assert.Equal(OrigemTreino.Strava, sessao.Origem);
}
```

- [ ] **Step 2: Run the focused tests to verify they fail**

Run: `dotnet test tests/CoachTraining.Domain.Tests/CoachTraining.Tests.csproj --filter "LinkPublicoIntegracaoTests|ConexaoWearableTests|CriarSessaoImportada_DeveRegistrarOrigemDoTreino"`

Expected: FAIL because the integration entities, enum types, and `SessaoDeTreino.Origem` do not exist yet.

- [ ] **Step 3: Implement the minimal domain types and persistence models**

```csharp
public enum OrigemTreino
{
    Manual = 0,
    Strava = 1
}

public enum ProvedorIntegracao
{
    Strava = 0,
    Garmin = 1,
    Polar = 2
}

public sealed class LinkPublicoIntegracao
{
    private LinkPublicoIntegracao(Guid atletaId, string tokenHash, DateTime criadoEmUtc)
    {
        Id = Guid.NewGuid();
        AtletaId = atletaId;
        TokenHash = tokenHash;
        CriadoEmUtc = criadoEmUtc;
        Ativo = true;
    }

    public Guid Id { get; }
    public Guid AtletaId { get; }
    public string TokenHash { get; private set; }
    public bool Ativo { get; private set; }
    public DateTime CriadoEmUtc { get; }
    public DateTime? RegeneradoEmUtc { get; private set; }
    public DateTime? RevogadoEmUtc { get; private set; }

    public static LinkPublicoIntegracao Criar(Guid atletaId, string tokenHash)
        => new(atletaId, tokenHash, DateTime.UtcNow);

    public void Regenerar(string novoTokenHash, DateTime quando)
    {
        TokenHash = string.IsNullOrWhiteSpace(novoTokenHash)
            ? throw new ArgumentException("Token hash obrigatorio.", nameof(novoTokenHash))
            : novoTokenHash;
        Ativo = true;
        RegeneradoEmUtc = quando;
        RevogadoEmUtc = null;
    }

    public void Revogar(DateTime quando)
    {
        Ativo = false;
        RevogadoEmUtc = quando;
    }
}
```

- [ ] **Step 4: Map EF Core models and migration**

```csharp
modelBuilder.Entity<LinkPublicoIntegracaoModel>(builder =>
{
    builder.ToTable("links_publicos_integracao");
    builder.HasKey(x => x.Id);
    builder.Property(x => x.TokenHash).HasMaxLength(200).IsRequired();
    builder.Property(x => x.Ativo).IsRequired();
    builder.HasIndex(x => x.TokenHash).IsUnique();
    builder.HasIndex(x => new { x.AtletaId, x.Ativo })
        .HasFilter("\"Ativo\" = true")
        .IsUnique();
});

modelBuilder.Entity<SessaoDeTreinoModel>(builder =>
{
    builder.Property(x => x.OrigemTreino).IsRequired().HasDefaultValue((int)OrigemTreino.Manual);
});
```

- [ ] **Step 5: Run the focused tests again**

Run: `dotnet test tests/CoachTraining.Domain.Tests/CoachTraining.Tests.csproj --filter "LinkPublicoIntegracaoTests|ConexaoWearableTests|CriarSessaoImportada_DeveRegistrarOrigemDoTreino"`

Expected: PASS.

- [ ] **Step 6: Commit**

```bash
git add src/CoachTraining.Domain src/CoachTraining.Infra/Persistence tests/CoachTraining.Domain.Tests/Domain
git commit -m "feat: add wearable integration domain foundation"
```

## Task 2: Teacher APIs and public link resolution

**Files:**
- Create: `src/CoachTraining.App/Abstractions/Persistence/ILinkPublicoIntegracaoRepository.cs`
- Create: `src/CoachTraining.App/Abstractions/Persistence/IConexaoWearableRepository.cs`
- Create: `src/CoachTraining.App/DTOs/Integrations/AtletaIntegracaoResumoDto.cs`
- Create: `src/CoachTraining.App/DTOs/Integrations/LinkPublicoIntegracaoDto.cs`
- Create: `src/CoachTraining.App/DTOs/Integrations/PublicIntegrationPageDto.cs`
- Create: `src/CoachTraining.App/Services/Integrations/GerarLinkPublicoIntegracaoService.cs`
- Create: `src/CoachTraining.App/Services/Integrations/ConsultarIntegracoesAtletaService.cs`
- Create: `src/CoachTraining.App/Services/Integrations/ResolverPaginaPublicaIntegracaoService.cs`
- Create: `src/CoachTraining.Infra/Persistence/Repositories/LinkPublicoIntegracaoRepository.cs`
- Create: `src/CoachTraining.Infra/Persistence/Repositories/ConexaoWearableRepository.cs`
- Create: `src/CoachTraining.Api/Controllers/AtletaIntegracoesController.cs`
- Create: `src/CoachTraining.Api/Controllers/PublicIntegracoesController.cs`
- Modify: `src/CoachTraining.Infra/DependencyInjection.cs`
- Modify: `src/CoachTraining.Api/Program.cs`
- Test: `tests/CoachTraining.Domain.Tests/App/Services/GerarLinkPublicoIntegracaoServiceTests.cs`
- Test: `tests/CoachTraining.Domain.Tests/Api/AtletaIntegracoesApiIntegrationTests.cs`
- Test: `tests/CoachTraining.Domain.Tests/Api/PublicIntegracoesApiIntegrationTests.cs`

- [ ] **Step 1: Write the failing service and API tests**

```csharp
[Fact]
public void GerarOuObterLink_DeveCriarTokenOpacoParaAtletaDoProfessor()
{
    var resultado = service.GerarOuObter(atletaId, professorId);

    Assert.StartsWith("https://coachtraining.com/conectar/", resultado.UrlPublica);
    Assert.False(string.IsNullOrWhiteSpace(resultado.TokenPublico));
}

[Fact]
public async Task PostApiAtletasIntegracoesLink_DeveRetornarLinkPublicoParaProfessorAutenticado()
{
    var response = await client.SendAsync(request);

    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
}

[Fact]
public async Task GetPublicIntegracoes_DeveRetornarProvedoresSemExporDadosDoAtleta()
{
    var response = await client.GetAsync($"/public/integracoes/{token}");
    var payload = await response.Content.ReadFromJsonAsync<PublicIntegrationPageDto>();

    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    Assert.DoesNotContain(payload!.Titulo, "@");
    Assert.Contains(payload.Provedores, p => p.ProviderKey == "strava");
}
```

- [ ] **Step 2: Run the focused tests to verify they fail**

Run: `dotnet test tests/CoachTraining.Domain.Tests/CoachTraining.Tests.csproj --filter "GerarOuObterLink|AtletaIntegracoesApiIntegrationTests|PublicIntegracoesApiIntegrationTests"`

Expected: FAIL because the repositories, DTOs, services, and controllers do not exist yet.

- [ ] **Step 3: Implement repositories and application services**

```csharp
public sealed class GerarLinkPublicoIntegracaoService
{
    public LinkPublicoIntegracaoDto GerarOuObter(Guid atletaId, Guid professorId)
    {
        var atleta = _atletaRepository.ObterPorId(atletaId, professorId)
            ?? throw new UnauthorizedAccessException("Atleta nao encontrado para o professor autenticado.");

        var existente = _linkRepository.ObterAtivoPorAtletaId(atleta.Id);
        if (existente != null)
        {
            return _urlFactory.CriarDto(existente, tokenPublico: null);
        }

        var tokenPublico = _tokenGenerator.GerarTokenPublico();
        var tokenHash = _tokenHasher.Hash(tokenPublico);
        var link = LinkPublicoIntegracao.Criar(atleta.Id, tokenHash);
        _linkRepository.Adicionar(link);

        return _urlFactory.CriarDto(link, tokenPublico);
    }
}
```

- [ ] **Step 4: Expose authenticated and public endpoints**

```csharp
[ApiController]
[Route("api/atletas/{atletaId:guid}/integracoes")]
[Authorize]
public sealed class AtletaIntegracoesController : ControllerBase
{
    [HttpPost("link")]
    public IActionResult GerarOuObterLink(Guid atletaId) { ... }

    [HttpPost("link/regenerar")]
    public IActionResult RegenerarLink(Guid atletaId) { ... }

    [HttpGet]
    public IActionResult Consultar(Guid atletaId) { ... }
}

[ApiController]
[Route("public/integracoes")]
public sealed class PublicIntegracoesController : ControllerBase
{
    [HttpGet("{token}")]
    [AllowAnonymous]
    public IActionResult ResolverPagina(string token) { ... }
}
```

- [ ] **Step 5: Run the focused tests again**

Run: `dotnet test tests/CoachTraining.Domain.Tests/CoachTraining.Tests.csproj --filter "GerarOuObterLink|AtletaIntegracoesApiIntegrationTests|PublicIntegracoesApiIntegrationTests"`

Expected: PASS.

- [ ] **Step 6: Commit**

```bash
git add src/CoachTraining.App src/CoachTraining.Api src/CoachTraining.Infra tests/CoachTraining.Domain.Tests/App tests/CoachTraining.Domain.Tests/Api
git commit -m "feat: add public integration link services and apis"
```

## Task 3: Strava OAuth provider and callback flow

**Files:**
- Create: `src/CoachTraining.App/Abstractions/Integrations/IWearableProvider.cs`
- Create: `src/CoachTraining.App/Abstractions/Integrations/IWearableProviderRegistry.cs`
- Create: `src/CoachTraining.App/Abstractions/Security/ISecretProtector.cs`
- Create: `src/CoachTraining.App/DTOs/Integrations/IniciarAutorizacaoResponseDto.cs`
- Create: `src/CoachTraining.App/DTOs/Integrations/ConcluirAutorizacaoResultDto.cs`
- Create: `src/CoachTraining.App/Services/Integrations/IniciarAutorizacaoStravaService.cs`
- Create: `src/CoachTraining.App/Services/Integrations/ConcluirAutorizacaoStravaService.cs`
- Create: `src/CoachTraining.Infra/Integrations/Strava/StravaOptions.cs`
- Create: `src/CoachTraining.Infra/Integrations/Strava/StravaWearableProvider.cs`
- Create: `src/CoachTraining.Infra/Integrations/Strava/DataProtectionSecretProtector.cs`
- Modify: `src/CoachTraining.Api/Controllers/PublicIntegracoesController.cs`
- Modify: `src/CoachTraining.Api/Program.cs`
- Modify: `src/CoachTraining.Api/appsettings.json`
- Modify: `src/CoachTraining.Api/appsettings.Development.json`
- Test: `tests/CoachTraining.Domain.Tests/App/Services/IniciarAutorizacaoStravaServiceTests.cs`
- Test: `tests/CoachTraining.Domain.Tests/Api/PublicIntegracoesApiIntegrationTests.cs`

- [ ] **Step 1: Write the failing authorization tests**

```csharp
[Fact]
public void IniciarAutorizacao_DeveMontarUrlOAuthDoStravaComStateAssinado()
{
    var response = service.Iniciar(tokenPublico);

    Assert.Contains("https://www.strava.com/oauth/authorize", response.AuthorizationUrl);
    Assert.Contains("scope=activity%3Aread", response.AuthorizationUrl);
    Assert.Contains("state=", response.AuthorizationUrl);
}

[Fact]
public async Task Callback_DevePersistirConexaoEProtejerCredenciais()
{
    var response = await client.GetAsync("/public/integracoes/strava/callback?code=abc&scope=activity:read&state=valido");

    Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
}
```

- [ ] **Step 2: Run the focused tests to verify they fail**

Run: `dotnet test tests/CoachTraining.Domain.Tests/CoachTraining.Tests.csproj --filter "IniciarAutorizacao|Callback_DevePersistirConexao"`

Expected: FAIL because there is no provider contract, no Strava provider, and no callback implementation.

- [ ] **Step 3: Implement provider contract and Strava provider**

```csharp
public interface IWearableProvider
{
    ProvedorIntegracao Provedor { get; }
    string BuildAuthorizationUrl(string redirectUri, string state);
    Task<ProviderTokenExchangeResult> ExchangeAuthorizationCodeAsync(string code, CancellationToken cancellationToken);
    Task<ProviderTokenExchangeResult> RefreshAccessTokenAsync(string refreshToken, CancellationToken cancellationToken);
    Task<ProviderActivityDto> GetActivityAsync(string accessToken, string externalActivityId, CancellationToken cancellationToken);
}

public sealed class StravaWearableProvider : IWearableProvider
{
    public string BuildAuthorizationUrl(string redirectUri, string state)
    {
        var query = HttpUtility.ParseQueryString(string.Empty);
        query["client_id"] = _options.ClientId.ToString(CultureInfo.InvariantCulture);
        query["redirect_uri"] = redirectUri;
        query["response_type"] = "code";
        query["approval_prompt"] = "auto";
        query["scope"] = "activity:read";
        query["state"] = state;
        return $"https://www.strava.com/oauth/authorize?{query}";
    }
}
```

- [ ] **Step 4: Implement start and callback endpoints**

```csharp
[HttpPost("{token}/strava/autorizar")]
[AllowAnonymous]
public IActionResult IniciarAutorizacaoStrava(string token)
{
    var response = _iniciarAutorizacaoStravaService.Iniciar(token);
    return Ok(response);
}

[HttpGet("strava/callback")]
[AllowAnonymous]
public async Task<IActionResult> CallbackStrava([FromQuery] string? code, [FromQuery] string? scope, [FromQuery] string? state, [FromQuery] string? error)
{
    var resultado = await _concluirAutorizacaoStravaService.ConcluirAsync(code, scope, state, error, HttpContext.RequestAborted);
    return Redirect(resultado.RedirectUrl);
}
```

- [ ] **Step 5: Run the focused tests again**

Run: `dotnet test tests/CoachTraining.Domain.Tests/CoachTraining.Tests.csproj --filter "IniciarAutorizacao|Callback_DevePersistirConexao"`

Expected: PASS.

- [ ] **Step 6: Commit**

```bash
git add src/CoachTraining.App src/CoachTraining.Api src/CoachTraining.Infra tests/CoachTraining.Domain.Tests
git commit -m "feat: add strava oauth authorization flow"
```

## Task 4: Webhook ingestion and automatic Strava import

**Files:**
- Create: `src/CoachTraining.App/Abstractions/Persistence/IEventoWebhookRepository.cs`
- Create: `src/CoachTraining.App/Abstractions/Persistence/IAtividadeImportadaRepository.cs`
- Create: `src/CoachTraining.App/Services/Integrations/ReceberWebhookStravaService.cs`
- Create: `src/CoachTraining.App/Services/Integrations/ProcessarEventoStravaService.cs`
- Create: `src/CoachTraining.Api/Controllers/StravaWebhookController.cs`
- Create: `src/CoachTraining.Infra/Persistence/Repositories/EventoWebhookRecebidoRepository.cs`
- Create: `src/CoachTraining.Infra/Persistence/Repositories/AtividadeImportadaRepository.cs`
- Modify: `src/CoachTraining.App/Services/CadastrarSessaoDeTreinoService.cs`
- Modify: `src/CoachTraining.Infra/Persistence/Repositories/SessaoDeTreinoRepository.cs`
- Test: `tests/CoachTraining.Domain.Tests/App/Services/ProcessarEventoStravaServiceTests.cs`
- Test: `tests/CoachTraining.Domain.Tests/Api/StravaWebhookApiIntegrationTests.cs`

- [ ] **Step 1: Write the failing webhook tests**

```csharp
[Fact]
public async Task ValidacaoWebhook_DeveEcoarHubChallenge()
{
    var response = await client.GetAsync("/api/integrations/strava/webhook/segredo?hub.verify_token=verify&hub.challenge=123&hub.mode=subscribe");

    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    Assert.Equal("{\"hub.challenge\":\"123\"}", await response.Content.ReadAsStringAsync());
}

[Fact]
public void ProcessarActivityCreate_DeveCriarSessaoManualComOrigemStravaEUmaImportacao()
{
    processador.Processar(payload);

    Assert.Single(sessaoRepo.Itens);
    Assert.Equal(OrigemTreino.Strava, sessaoRepo.Itens[0].Origem);
    Assert.Single(importadaRepo.Itens);
}

[Fact]
public void ReprocessarMesmaAtividade_DeveSerIdempotente()
{
    processador.Processar(payload);
    processador.Processar(payload);

    Assert.Single(sessaoRepo.Itens);
}
```

- [ ] **Step 2: Run the focused tests to verify they fail**

Run: `dotnet test tests/CoachTraining.Domain.Tests/CoachTraining.Tests.csproj --filter "ValidacaoWebhook|ProcessarActivityCreate|ReprocessarMesmaAtividade"`

Expected: FAIL because the webhook controller and processing services do not exist.

- [ ] **Step 3: Implement webhook HTTP layer and event persistence**

```csharp
[ApiController]
[Route("api/integrations/strava/webhook/{secret}")]
public sealed class StravaWebhookController : ControllerBase
{
    [HttpGet]
    [AllowAnonymous]
    public IActionResult Validar(string secret, [FromQuery(Name = "hub.verify_token")] string verifyToken, [FromQuery(Name = "hub.challenge")] string challenge)
    {
        if (!_service.ValidarHandshake(secret, verifyToken))
        {
            return Unauthorized();
        }

        return Ok(new Dictionary<string, string> { ["hub.challenge"] = challenge });
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Receber(string secret, [FromBody] JsonElement payload)
    {
        await _service.ReceberAsync(secret, payload, HttpContext.RequestAborted);
        return Ok();
    }
}
```

- [ ] **Step 4: Implement asynchronous-safe processor and idempotent import**

```csharp
if (_atividadeImportadaRepository.Existe(ProvedorIntegracao.Strava, evento.ObjectId.ToString(CultureInfo.InvariantCulture)))
{
    _eventoWebhookRepository.MarcarComoIgnorado(eventoId, "atividade ja importada");
    return;
}

var atividade = await _provider.GetActivityAsync(accessToken, externalActivityId, cancellationToken);
var sessao = new SessaoDeTreino(
    atletaId: conexao.AtletaId,
    data: DateOnly.FromDateTime(atividade.StartDateUtc),
    tipo: _mapper.MapearTipo(atividade.SportType),
    duracaoMinutos: atividade.MovingTimeSeconds / 60,
    distanciaKm: atividade.DistanceMeters / 1000d,
    rpe: new RPE(5),
    origem: OrigemTreino.Strava);

_sessaoDeTreinoRepository.Adicionar(sessao);
_atividadeImportadaRepository.Adicionar(new AtividadeImportada(...));
```

- [ ] **Step 5: Run the focused tests again**

Run: `dotnet test tests/CoachTraining.Domain.Tests/CoachTraining.Tests.csproj --filter "ValidacaoWebhook|ProcessarActivityCreate|ReprocessarMesmaAtividade"`

Expected: PASS.

- [ ] **Step 6: Commit**

```bash
git add src/CoachTraining.App src/CoachTraining.Api src/CoachTraining.Infra tests/CoachTraining.Domain.Tests
git commit -m "feat: import strava activities from webhooks"
```

## Task 5: Teacher integration panel and public Angular flow

**Files:**
- Create: `frontend/src/app/features/integrations/models/integration.model.ts`
- Create: `frontend/src/app/features/integrations/services/teacher-integrations-api.service.ts`
- Create: `frontend/src/app/features/integrations/services/public-integrations-api.service.ts`
- Create: `frontend/src/app/features/integrations/components/student-integrations-panel.component.ts`
- Create: `frontend/src/app/features/integrations/components/student-integrations-panel.component.html`
- Create: `frontend/src/app/features/integrations/components/student-integrations-panel.component.css`
- Create: `frontend/src/app/features/integrations/pages/public-integration-page.component.ts`
- Create: `frontend/src/app/features/integrations/pages/public-integration-page.component.html`
- Create: `frontend/src/app/features/integrations/pages/public-integration-page.component.css`
- Create: `frontend/src/app/features/integrations/pages/strava-callback-page.component.ts`
- Create: `frontend/src/app/features/integrations/pages/strava-callback-page.component.html`
- Create: `frontend/src/app/features/integrations/pages/strava-callback-page.component.css`
- Modify: `frontend/src/app/app.routes.ts`
- Modify: `frontend/src/app/features/dashboard/pages/student-dashboard-page.component.ts`
- Modify: `frontend/src/app/features/dashboard/pages/student-dashboard-page.component.html`
- Test: `frontend/src/app/features/integrations/services/teacher-integrations-api.service.spec.ts`
- Test: `frontend/src/app/features/integrations/services/public-integrations-api.service.spec.ts`
- Test: `frontend/src/app/features/integrations/pages/public-integration-page.component.spec.ts`
- Test: `frontend/src/app/features/integrations/pages/strava-callback-page.component.spec.ts`

- [ ] **Step 1: Write the failing frontend tests**

```typescript
it('loads public providers from the token route', () => {
  service.obterPagina('token-opaco').subscribe((result) => {
    expect(result.providers[0].providerKey).toBe('strava');
  });

  const req = httpMock.expectOne('/public/integracoes/token-opaco');
  req.flush({
    title: 'Conectar aplicativos ao CoachTraining',
    providers: [{ providerKey: 'strava', displayName: 'Strava', status: 'not_connected', enabled: true }]
  });
});

it('renders copy link and regenerate actions for the teacher panel', () => {
  component.integracoes = {
    publicLinkUrl: 'https://coachtraining.com/conectar/abc',
    providers: []
  };

  fixture.detectChanges();

  expect(fixture.nativeElement.textContent).toContain('Copiar link');
  expect(fixture.nativeElement.textContent).toContain('Regenerar link');
});
```

- [ ] **Step 2: Run the focused frontend tests to verify they fail**

Run: `npm test -- --watch=false --include src/app/features/integrations/**/*.spec.ts`

Expected: FAIL because the integration services, pages, and panel component do not exist.

- [ ] **Step 3: Implement shared models, services, and routes**

```typescript
export interface ProviderIntegrationStatus {
  providerKey: 'strava' | 'garmin' | 'polar';
  displayName: string;
  status: 'not_connected' | 'connected' | 'authorization_error' | 'reconnect_required' | 'disconnected';
  enabled: boolean;
  lastSyncAtUtc?: string;
}

export const routes: Routes = [
  { path: 'conectar/:token', loadComponent: () => import('./features/integrations/pages/public-integration-page.component').then(m => m.PublicIntegrationPageComponent) },
  { path: 'conectar/strava/retorno', loadComponent: () => import('./features/integrations/pages/strava-callback-page.component').then(m => m.StravaCallbackPageComponent) }
];
```

- [ ] **Step 4: Mount the teacher panel inside the athlete dashboard**

```typescript
imports: [
  CommonModule,
  RouterLink,
  StudentIntegrationsPanelComponent
]
```

```html
<app-student-integrations-panel [atletaId]="atletaId"></app-student-integrations-panel>
```

- [ ] **Step 5: Build the public page and callback page**

```typescript
iniciar(providerKey: 'strava'): void {
  if (!this.token) {
    return;
  }

  this.publicIntegrationsApiService.iniciarStrava(this.token).subscribe({
    next: (response) => window.location.assign(response.authorizationUrl),
    error: () => this.errorMessage = 'Nao foi possivel iniciar a autorizacao com o Strava.'
  });
}
```

- [ ] **Step 6: Run the focused frontend tests and build**

Run: `npm test -- --watch=false --include src/app/features/integrations/**/*.spec.ts`

Expected: PASS.

Run: `npm run build`

Expected: PASS.

- [ ] **Step 7: Commit**

```bash
git add frontend/src/app
git commit -m "feat: add teacher and public integration frontend flows"
```

## Task 6: Implementation documentation and full verification

**Files:**
- Create: `docs/apis/integracoes-wearables-api.md`
- Create: `docs/flows/fluxo-integracao-strava.md`
- Modify: `docs/architecture/overview.md`
- Modify: `README.md`

- [ ] **Step 1: Write the implementation docs**

```markdown
# API de Integracoes com Wearables

## Endpoints privados do professor
- `GET /api/atletas/{id}/integracoes`
- `POST /api/atletas/{id}/integracoes/link`
- `POST /api/atletas/{id}/integracoes/link/regenerar`

## Endpoints publicos
- `GET /public/integracoes/{token}`
- `POST /public/integracoes/{token}/strava/autorizar`
- `GET /public/integracoes/strava/callback`

## Webhook
- `GET /api/integrations/strava/webhook/{secret}`
- `POST /api/integrations/strava/webhook/{secret}`
```

- [ ] **Step 2: Run backend verification**

Run: `dotnet test tests/CoachTraining.Domain.Tests/CoachTraining.Tests.csproj`

Expected: PASS with 0 failures.

- [ ] **Step 3: Run frontend verification**

Run: `npm test -- --watch=false --browsers=ChromeHeadless`

Expected: PASS.

Run: `npm run build`

Expected: PASS.

- [ ] **Step 4: Run final git review**

Run: `git status --short`

Expected: only the intended implementation and docs files are staged or ready to stage.

- [ ] **Step 5: Commit**

```bash
git add docs README.md
git commit -m "docs: document strava wearable integration implementation"
```
