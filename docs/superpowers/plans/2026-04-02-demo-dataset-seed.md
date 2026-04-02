# Demo Dataset Seed Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Build a deterministic `demo-v1` dataset seeder that recreates a closed presentation dataset in the real PostgreSQL database, with one demo professor and six athletes covering the approved coaching scenarios.

**Architecture:** Add a dedicated .NET console project that reuses the current App and Infra layers for writes, and uses a small cleanup routine over `CoachTrainingDbContext` to remove only records belonging to the demo dataset. Scenario builders generate 12-week histories relative to a reference date, the runner persists the profile through existing services, and a final report prints the exact login plus the expected headline for each athlete dashboard.

**Tech Stack:** .NET 10 console app, EF Core 10, Npgsql, existing `CoachTraining.App` services, existing `CoachTraining.Infra` repositories, xUnit

---

## File Structure

**Create**
- `src/CoachTraining.DemoSeed/CoachTraining.DemoSeed.csproj`
- `src/CoachTraining.DemoSeed/Program.cs`
- `src/CoachTraining.DemoSeed/appsettings.json`
- `src/CoachTraining.DemoSeed/appsettings.Development.json`
- `src/CoachTraining.DemoSeed/DemoSeedOptions.cs`
- `src/CoachTraining.DemoSeed/DemoSeedRunner.cs`
- `src/CoachTraining.DemoSeed/Contracts/DemoProfileDefinition.cs`
- `src/CoachTraining.DemoSeed/Contracts/DemoScenarioSeed.cs`
- `src/CoachTraining.DemoSeed/Contracts/DemoSessaoSeed.cs`
- `src/CoachTraining.DemoSeed/Contracts/DemoProvaAlvoSeed.cs`
- `src/CoachTraining.DemoSeed/Scenarios/DemoScenarioBuilderBase.cs`
- `src/CoachTraining.DemoSeed/Scenarios/BaseEstavelScenario.cs`
- `src/CoachTraining.DemoSeed/Scenarios/ConstrucaoSaudavelScenario.cs`
- `src/CoachTraining.DemoSeed/Scenarios/RiscoCargaAbruptaScenario.cs`
- `src/CoachTraining.DemoSeed/Scenarios/TaperBemExecutadoScenario.cs`
- `src/CoachTraining.DemoSeed/Scenarios/AderenciaBaixaScenario.cs`
- `src/CoachTraining.DemoSeed/Scenarios/DivergenciaCargaRendimentoScenario.cs`
- `src/CoachTraining.DemoSeed/Scenarios/DemoScenarioFactory.cs`
- `src/CoachTraining.DemoSeed/Reports/DemoSeedReport.cs`
- `src/CoachTraining.DemoSeed/Reports/DemoSeedReportFormatter.cs`
- `tests/CoachTraining.Domain.Tests/DemoSeed/DemoSeedOptionsTests.cs`
- `tests/CoachTraining.Domain.Tests/DemoSeed/DemoScenarioFactoryTests.cs`
- `tests/CoachTraining.Domain.Tests/DemoSeed/DemoSeedRunnerTests.cs`
- `tests/CoachTraining.Domain.Tests/DemoSeed/DemoSeedReportFormatterTests.cs`
- `docs/demo/demo-v1.md`

**Modify**
- `CoachTraining.sln`
- `tests/CoachTraining.Domain.Tests/CoachTraining.Tests.csproj`
- `README.md`
- `docs/setup/ambiente.md`

**Responsibilities**
- `DemoSeedOptions.cs`: parse and validate CLI arguments (`--profile`, `--reset-demo`, `--reset-all`, `--help`).
- `DemoScenarioBuilderBase.cs`: shared date/session helpers so all scenarios stay relative to the current date and visually plausible.
- `DemoScenarioFactory.cs`: build the single approved profile `demo-v1` and return the six approved scenarios.
- `DemoSeedRunner.cs`: ensure database exists, optionally clean the old demo dataset, recreate professor/athletes/history, and return a structured report.
- `DemoSeedReportFormatter.cs`: print a presentation-friendly summary that can be copied into demo notes.
- `docs/demo/demo-v1.md`: describe each athlete, expected dashboard interpretation, and the pre-demo execution checklist.

### Task 1: Scaffold the console project and CLI options

**Files:**
- Create: `src/CoachTraining.DemoSeed/CoachTraining.DemoSeed.csproj`
- Create: `src/CoachTraining.DemoSeed/appsettings.json`
- Create: `src/CoachTraining.DemoSeed/appsettings.Development.json`
- Create: `src/CoachTraining.DemoSeed/Program.cs`
- Create: `src/CoachTraining.DemoSeed/DemoSeedOptions.cs`
- Modify: `CoachTraining.sln`
- Modify: `tests/CoachTraining.Domain.Tests/CoachTraining.Tests.csproj`
- Test: `tests/CoachTraining.Domain.Tests/DemoSeed/DemoSeedOptionsTests.cs`

- [ ] **Step 1: Create the console project, wire references, and add it to the solution**

```xml
<!-- src/CoachTraining.DemoSeed/CoachTraining.DemoSeed.csproj -->
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net10.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>

  <ItemGroup>
    <ProjectReference Include="..\\CoachTraining.App\\CoachTraining.App.csproj" />
    <ProjectReference Include="..\\CoachTraining.Infra\\CoachTraining.Infra.csproj" />
    <ProjectReference Include="..\\CoachTraining.Domain\\CoachTraining.Domain.csproj" />
  </ItemGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.Extensions.Hosting" Version="10.0.0" />
  </ItemGroup>

  <ItemGroup>
    <None Update="appsettings.json">
      <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
    </None>
    <None Update="appsettings.Development.json">
      <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
    </None>
  </ItemGroup>
</Project>
```

```json
// src/CoachTraining.DemoSeed/appsettings.json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning"
    }
  }
}
```

```json
// src/CoachTraining.DemoSeed/appsettings.Development.json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=coachtraining;Username=coach;Password=coach"
  }
}
```

```xml
<!-- tests/CoachTraining.Domain.Tests/CoachTraining.Tests.csproj -->
<ItemGroup>
  <ProjectReference Include="..\\..\\src\\CoachTraining.DemoSeed\\CoachTraining.DemoSeed.csproj" />
</ItemGroup>
```

Run:

```bash
dotnet sln CoachTraining.sln add src/CoachTraining.DemoSeed/CoachTraining.DemoSeed.csproj
```

Expected: the solution starts listing `CoachTraining.DemoSeed` as a new project.

- [ ] **Step 2: Write the failing tests for argument parsing**

```csharp
// tests/CoachTraining.Domain.Tests/DemoSeed/DemoSeedOptionsTests.cs
using CoachTraining.DemoSeed;

namespace CoachTraining.Tests.DemoSeed;

public class DemoSeedOptionsTests
{
    [Fact]
    public void Parse_ComResetDemoEProfile_DeveRetornarOpcoesValidas()
    {
        var options = DemoSeedOptions.Parse(["--profile", "demo-v1", "--reset-demo"]);

        Assert.Equal("demo-v1", options.Profile);
        Assert.True(options.ResetDemo);
        Assert.False(options.ResetAll);
        Assert.False(options.HelpRequested);
    }

    [Fact]
    public void Parse_SemProfileExplicito_DeveUsarDemoV1()
    {
        var options = DemoSeedOptions.Parse(["--reset-demo"]);

        Assert.Equal("demo-v1", options.Profile);
    }

    [Fact]
    public void Parse_ComFlagDesconhecida_DeveLancarArgumentException()
    {
        var exception = Assert.Throws<ArgumentException>(() => DemoSeedOptions.Parse(["--unknown"]));

        Assert.Contains("--unknown", exception.Message);
    }
}
```

- [ ] **Step 3: Run the new test to confirm the failure**

Run:

```bash
dotnet test tests/CoachTraining.Domain.Tests/CoachTraining.Tests.csproj --filter "FullyQualifiedName~DemoSeedOptionsTests"
```

Expected: FAIL because `CoachTraining.DemoSeed` and `DemoSeedOptions.Parse` do not exist yet.

- [ ] **Step 4: Implement `DemoSeedOptions` and a minimal `Program` that prints help**

```csharp
// src/CoachTraining.DemoSeed/DemoSeedOptions.cs
namespace CoachTraining.DemoSeed;

public sealed record DemoSeedOptions(
    string Profile,
    bool ResetDemo,
    bool ResetAll,
    bool HelpRequested)
{
    public static DemoSeedOptions Parse(string[] args)
    {
        var profile = "demo-v1";
        var resetDemo = false;
        var resetAll = false;
        var helpRequested = false;

        for (var index = 0; index < args.Length; index++)
        {
            switch (args[index])
            {
                case "--profile":
                    if (index + 1 >= args.Length)
                    {
                        throw new ArgumentException("Missing value for --profile.");
                    }

                    profile = args[++index].Trim();
                    break;
                case "--reset-demo":
                    resetDemo = true;
                    break;
                case "--reset-all":
                    resetAll = true;
                    break;
                case "--help":
                case "-h":
                    helpRequested = true;
                    break;
                default:
                    throw new ArgumentException($"Unknown argument: {args[index]}");
            }
        }

        if (string.IsNullOrWhiteSpace(profile))
        {
            throw new ArgumentException("Profile cannot be empty.");
        }

        return new DemoSeedOptions(profile, resetDemo, resetAll, helpRequested);
    }
}
```

```csharp
// src/CoachTraining.DemoSeed/Program.cs
using CoachTraining.DemoSeed;

var options = DemoSeedOptions.Parse(args);

if (options.HelpRequested)
{
    Console.WriteLine("Usage: dotnet run --project src/CoachTraining.DemoSeed -- --profile demo-v1 --reset-demo");
    Console.WriteLine("Flags: --profile <name> | --reset-demo | --reset-all | --help");
    return;
}

Console.WriteLine($"Demo seeder scaffolded for profile '{options.Profile}'.");
```

- [ ] **Step 5: Run the targeted tests and a build**

Run:

```bash
dotnet test tests/CoachTraining.Domain.Tests/CoachTraining.Tests.csproj --filter "FullyQualifiedName~DemoSeedOptionsTests"
dotnet build CoachTraining.sln
```

Expected:
- first command PASS
- second command PASS with the new console project included

- [ ] **Step 6: Commit the scaffold**

```bash
git add CoachTraining.sln tests/CoachTraining.Domain.Tests/CoachTraining.Tests.csproj src/CoachTraining.DemoSeed tests/CoachTraining.Domain.Tests/DemoSeed/DemoSeedOptionsTests.cs
git commit -m "chore: scaffold demo seed project"
```

### Task 2: Build the deterministic `demo-v1` scenario catalog

**Files:**
- Create: `src/CoachTraining.DemoSeed/Contracts/DemoProfileDefinition.cs`
- Create: `src/CoachTraining.DemoSeed/Contracts/DemoScenarioSeed.cs`
- Create: `src/CoachTraining.DemoSeed/Contracts/DemoSessaoSeed.cs`
- Create: `src/CoachTraining.DemoSeed/Contracts/DemoProvaAlvoSeed.cs`
- Create: `src/CoachTraining.DemoSeed/Scenarios/DemoScenarioBuilderBase.cs`
- Create: `src/CoachTraining.DemoSeed/Scenarios/BaseEstavelScenario.cs`
- Create: `src/CoachTraining.DemoSeed/Scenarios/ConstrucaoSaudavelScenario.cs`
- Create: `src/CoachTraining.DemoSeed/Scenarios/RiscoCargaAbruptaScenario.cs`
- Create: `src/CoachTraining.DemoSeed/Scenarios/TaperBemExecutadoScenario.cs`
- Create: `src/CoachTraining.DemoSeed/Scenarios/AderenciaBaixaScenario.cs`
- Create: `src/CoachTraining.DemoSeed/Scenarios/DivergenciaCargaRendimentoScenario.cs`
- Create: `src/CoachTraining.DemoSeed/Scenarios/DemoScenarioFactory.cs`
- Test: `tests/CoachTraining.Domain.Tests/DemoSeed/DemoScenarioFactoryTests.cs`

- [ ] **Step 1: Write the failing tests for the approved scenarios**

```csharp
// tests/CoachTraining.Domain.Tests/DemoSeed/DemoScenarioFactoryTests.cs
using CoachTraining.DemoSeed.Scenarios;

namespace CoachTraining.Tests.DemoSeed;

public class DemoScenarioFactoryTests
{
    [Fact]
    public void CreateProfile_DemoV1_DeveRetornarSeisCenariosComEmailsUnicos()
    {
        var profile = DemoScenarioFactory.CreateProfile("demo-v1", new DateOnly(2026, 4, 2));

        Assert.Equal("demo.professor@coachtraining.local", profile.ProfessorEmail);
        Assert.Equal(6, profile.Cenarios.Count);
        Assert.Equal(6, profile.Cenarios.Select(cenario => cenario.Email).Distinct(StringComparer.OrdinalIgnoreCase).Count());
        Assert.All(profile.Cenarios, cenario =>
        {
            var semanas = cenario.Sessoes
                .Select(sessao => sessao.Data.AddDays(-(((int)sessao.Data.DayOfWeek + 6) % 7)))
                .Distinct()
                .Count();

            Assert.InRange(semanas, 10, 12);
        });
    }

    [Fact]
    public void CreateProfile_DemoV1_DeveConterTaperEAderenciaBaixa()
    {
        var profile = DemoScenarioFactory.CreateProfile("demo-v1", new DateOnly(2026, 4, 2));

        var taper = profile.Cenarios.Single(cenario => cenario.Slug == "taper-bem-executado");
        var aderenciaBaixa = profile.Cenarios.Single(cenario => cenario.Slug == "aderencia-baixa");

        Assert.NotNull(taper.ProvaAlvo);
        Assert.InRange(taper.ProvaAlvo!.DataProva.DayNumber - new DateOnly(2026, 4, 2).DayNumber, 7, 21);
        Assert.Equal(5, aderenciaBaixa.TreinosPlanejadosPorSemana);
        Assert.Equal(2, aderenciaBaixa.Sessoes.Count(sessao => sessao.Data >= new DateOnly(2026, 3, 27)));
        Assert.All(profile.Cenarios, cenario => Assert.DoesNotContain(cenario.Sessoes, sessao => sessao.Data > new DateOnly(2026, 4, 2)));
    }

    [Fact]
    public void CreateProfile_ProfileDesconhecido_DeveLancarInvalidOperationException()
    {
        var exception = Assert.Throws<InvalidOperationException>(() =>
            DemoScenarioFactory.CreateProfile("nao-existe", new DateOnly(2026, 4, 2)));

        Assert.Contains("nao-existe", exception.Message);
    }
}
```

- [ ] **Step 2: Run the targeted tests to verify the failure**

Run:

```bash
dotnet test tests/CoachTraining.Domain.Tests/CoachTraining.Tests.csproj --filter "FullyQualifiedName~DemoScenarioFactoryTests"
```

Expected: FAIL because the contracts and factory are not implemented.

- [ ] **Step 3: Implement the profile contracts, shared builder helpers, and the six scenarios**

```csharp
// src/CoachTraining.DemoSeed/Contracts/DemoProfileDefinition.cs
namespace CoachTraining.DemoSeed.Contracts;

public sealed record DemoProfileDefinition(
    string Profile,
    string ProfessorNome,
    string ProfessorEmail,
    string ProfessorSenha,
    IReadOnlyList<DemoScenarioSeed> Cenarios);
```

```csharp
// src/CoachTraining.DemoSeed/Contracts/DemoScenarioSeed.cs
namespace CoachTraining.DemoSeed.Contracts;

public sealed record DemoScenarioSeed(
    string Slug,
    string Nome,
    string Email,
    string NivelEsportivo,
    string? ObservacoesClinicas,
    int TreinosPlanejadosPorSemana,
    string InsightEsperado,
    DemoProvaAlvoSeed? ProvaAlvo,
    IReadOnlyList<DemoSessaoSeed> Sessoes);
```

```csharp
// src/CoachTraining.DemoSeed/Contracts/DemoSessaoSeed.cs
using CoachTraining.Domain.Enums;

namespace CoachTraining.DemoSeed.Contracts;

public sealed record DemoSessaoSeed(
    DateOnly Data,
    TipoDeTreino Tipo,
    int DuracaoMinutos,
    double DistanciaKm,
    int Rpe);
```

```csharp
// src/CoachTraining.DemoSeed/Contracts/DemoProvaAlvoSeed.cs
namespace CoachTraining.DemoSeed.Contracts;

public sealed record DemoProvaAlvoSeed(
    DateOnly DataProva,
    double DistanciaKm,
    string Objetivo);
```

```csharp
// src/CoachTraining.DemoSeed/Scenarios/DemoScenarioBuilderBase.cs
using CoachTraining.DemoSeed.Contracts;
using CoachTraining.Domain.Enums;

namespace CoachTraining.DemoSeed.Scenarios;

public abstract class DemoScenarioBuilderBase
{
    protected DemoScenarioBuilderBase(DateOnly referencia)
    {
        Referencia = referencia;
        SegundaAtual = referencia.AddDays(-(((int)referencia.DayOfWeek + 6) % 7));
    }

    protected DateOnly Referencia { get; }
    protected DateOnly SegundaAtual { get; }

    protected DateOnly EmSemana(int semanasAtras, DayOfWeek dia)
    {
        var deslocamento = ((int)dia + 6) % 7;
        return SegundaAtual.AddDays(-(7 * semanasAtras) + deslocamento);
    }

    protected DemoSessaoSeed Sessao(int semanasAtras, DayOfWeek dia, TipoDeTreino tipo, int duracao, double distancia, int rpe)
        => new(EmSemana(semanasAtras, dia), tipo, duracao, distancia, rpe);

    protected IReadOnlyList<DemoSessaoSeed> BlocoSemanal(
        IEnumerable<int> semanasAtras,
        params (DayOfWeek Dia, TipoDeTreino Tipo, int Duracao, double Distancia, int Rpe)[] sessoes)
    {
        var resultado = new List<DemoSessaoSeed>();

        foreach (var semana in semanasAtras)
        {
            foreach (var sessao in sessoes)
            {
                var data = EmSemana(semana, sessao.Dia);
                if (data > Referencia)
                {
                    continue;
                }

                resultado.Add(new DemoSessaoSeed(data, sessao.Tipo, sessao.Duracao, sessao.Distancia, sessao.Rpe));
            }
        }

        return resultado.OrderBy(sessao => sessao.Data).ToList();
    }

    protected IReadOnlyList<DemoSessaoSeed> Combinar(params IReadOnlyList<DemoSessaoSeed>[] blocos)
        => blocos.SelectMany(bloco => bloco).OrderBy(sessao => sessao.Data).ToList();

    public abstract DemoScenarioSeed Build();
}
```

```csharp
// src/CoachTraining.DemoSeed/Scenarios/BaseEstavelScenario.cs
using CoachTraining.DemoSeed.Contracts;
using CoachTraining.Domain.Enums;

namespace CoachTraining.DemoSeed.Scenarios;

public sealed class BaseEstavelScenario(DateOnly referencia) : DemoScenarioBuilderBase(referencia)
{
    public override DemoScenarioSeed Build()
        => new(
            "base-estavel",
            "Ana Souza - Base estavel",
            "demo.ana.souza@coachtraining.local",
            "Intermediaria",
            null,
            4,
            "Sem alerta critico e carga estavel.",
            null,
            BlocoSemanal(
                Enumerable.Range(0, 12),
                (DayOfWeek.Tuesday, TipoDeTreino.Leve, 45, 7.0, 4),
                (DayOfWeek.Thursday, TipoDeTreino.Ritmo, 50, 8.0, 5),
                (DayOfWeek.Saturday, TipoDeTreino.Longo, 75, 12.0, 6),
                (DayOfWeek.Sunday, TipoDeTreino.Leve, 35, 5.5, 3)));
}
```

```csharp
// src/CoachTraining.DemoSeed/Scenarios/ConstrucaoSaudavelScenario.cs
using CoachTraining.DemoSeed.Contracts;
using CoachTraining.Domain.Enums;

namespace CoachTraining.DemoSeed.Scenarios;

public sealed class ConstrucaoSaudavelScenario(DateOnly referencia) : DemoScenarioBuilderBase(referencia)
{
    public override DemoScenarioSeed Build()
        => new(
            "construcao-saudavel",
            "Bruno Lima - Construcao saudavel",
            "demo.bruno.lima@coachtraining.local",
            "Avancado",
            null,
            5,
            "Progressao controlada e sem risco alto.",
            null,
            Combinar(
                BlocoSemanal(
                    [11, 10, 9, 8],
                    (DayOfWeek.Monday, TipoDeTreino.Leve, 35, 6.0, 4),
                    (DayOfWeek.Tuesday, TipoDeTreino.Ritmo, 45, 8.0, 5),
                    (DayOfWeek.Thursday, TipoDeTreino.Intervalado, 50, 8.5, 6),
                    (DayOfWeek.Saturday, TipoDeTreino.Longo, 80, 13.5, 6),
                    (DayOfWeek.Sunday, TipoDeTreino.Leve, 30, 5.0, 3)),
                BlocoSemanal(
                    [7, 6, 5, 4],
                    (DayOfWeek.Monday, TipoDeTreino.Leve, 40, 6.5, 4),
                    (DayOfWeek.Tuesday, TipoDeTreino.Ritmo, 50, 8.5, 5),
                    (DayOfWeek.Thursday, TipoDeTreino.Intervalado, 55, 9.0, 6),
                    (DayOfWeek.Saturday, TipoDeTreino.Longo, 90, 15.0, 7),
                    (DayOfWeek.Sunday, TipoDeTreino.Leve, 35, 5.5, 3)),
                BlocoSemanal(
                    [3, 2, 1, 0],
                    (DayOfWeek.Monday, TipoDeTreino.Leve, 45, 7.0, 4),
                    (DayOfWeek.Tuesday, TipoDeTreino.Ritmo, 55, 9.0, 6),
                    (DayOfWeek.Thursday, TipoDeTreino.Intervalado, 60, 9.5, 7),
                    (DayOfWeek.Saturday, TipoDeTreino.Longo, 100, 16.5, 7),
                    (DayOfWeek.Sunday, TipoDeTreino.Leve, 40, 6.0, 3)));
}
```

```csharp
// src/CoachTraining.DemoSeed/Scenarios/RiscoCargaAbruptaScenario.cs
using CoachTraining.DemoSeed.Contracts;
using CoachTraining.Domain.Enums;

namespace CoachTraining.DemoSeed.Scenarios;

public sealed class RiscoCargaAbruptaScenario(DateOnly referencia) : DemoScenarioBuilderBase(referencia)
{
    public override DemoScenarioSeed Build()
        => new(
            "risco-carga-abrupta",
            "Carla Mendes - Risco por carga abrupta",
            "demo.carla.mendes@coachtraining.local",
            "Intermediaria",
            "Relata cansaco persistente apos aumento recente de volume.",
            4,
            "ACWR e delta semanal altos.",
            null,
            Combinar(
                BlocoSemanal(
                    [11, 10, 9, 8, 7, 6, 5, 4, 3, 2, 1],
                    (DayOfWeek.Tuesday, TipoDeTreino.Leve, 35, 5.5, 4),
                    (DayOfWeek.Thursday, TipoDeTreino.Ritmo, 40, 6.5, 5),
                    (DayOfWeek.Saturday, TipoDeTreino.Longo, 55, 9.0, 5)),
                BlocoSemanal(
                    [0],
                    (DayOfWeek.Monday, TipoDeTreino.Intervalado, 90, 12.0, 8),
                    (DayOfWeek.Wednesday, TipoDeTreino.Intervalado, 95, 12.5, 9),
                    (DayOfWeek.Thursday, TipoDeTreino.Ritmo, 80, 11.0, 8))));
}
```

```csharp
// src/CoachTraining.DemoSeed/Scenarios/TaperBemExecutadoScenario.cs
using CoachTraining.DemoSeed.Contracts;
using CoachTraining.Domain.Enums;

namespace CoachTraining.DemoSeed.Scenarios;

public sealed class TaperBemExecutadoScenario(DateOnly referencia) : DemoScenarioBuilderBase(referencia)
{
    public override DemoScenarioSeed Build()
        => new(
            "taper-bem-executado",
            "Diego Alves - Taper bem executado",
            "demo.diego.alves@coachtraining.local",
            "Avancado",
            null,
            5,
            "Taper adequado com reducao de 40-60%.",
            new DemoProvaAlvoSeed(Referencia.AddDays(10), 21.1, "Meia maratona sub-1h45"),
            Combinar(
                BlocoSemanal(
                    [11, 10, 9, 8, 7, 6, 5, 4, 3, 2, 1],
                    (DayOfWeek.Monday, TipoDeTreino.Leve, 50, 8.0, 4),
                    (DayOfWeek.Tuesday, TipoDeTreino.Ritmo, 60, 10.0, 6),
                    (DayOfWeek.Thursday, TipoDeTreino.Intervalado, 65, 10.5, 7),
                    (DayOfWeek.Saturday, TipoDeTreino.Longo, 120, 18.0, 7),
                    (DayOfWeek.Sunday, TipoDeTreino.Leve, 40, 6.0, 3)),
                BlocoSemanal(
                    [0],
                    (DayOfWeek.Monday, TipoDeTreino.Leve, 35, 5.5, 3),
                    (DayOfWeek.Wednesday, TipoDeTreino.Ritmo, 40, 6.5, 4))));
}
```

```csharp
// src/CoachTraining.DemoSeed/Scenarios/AderenciaBaixaScenario.cs
using CoachTraining.DemoSeed.Contracts;
using CoachTraining.Domain.Enums;

namespace CoachTraining.DemoSeed.Scenarios;

public sealed class AderenciaBaixaScenario(DateOnly referencia) : DemoScenarioBuilderBase(referencia)
{
    public override DemoScenarioSeed Build()
        => new(
            "aderencia-baixa",
            "Fernanda Rocha - Aderencia baixa",
            "demo.fernanda.rocha@coachtraining.local",
            "Intermediaria",
            "Dificuldade de encaixar treinos em semana de trabalho intenso.",
            5,
            "Baixa aderencia ao planejamento.",
            null,
            Combinar(
                BlocoSemanal(
                    [11, 10, 9, 8, 7, 6, 5, 4, 3, 2, 1],
                    (DayOfWeek.Monday, TipoDeTreino.Leve, 35, 5.5, 4),
                    (DayOfWeek.Tuesday, TipoDeTreino.Ritmo, 45, 7.5, 5),
                    (DayOfWeek.Thursday, TipoDeTreino.Intervalado, 50, 8.0, 6),
                    (DayOfWeek.Saturday, TipoDeTreino.Longo, 85, 13.0, 6),
                    (DayOfWeek.Sunday, TipoDeTreino.Leve, 30, 4.5, 3)),
                BlocoSemanal(
                    [0],
                    (DayOfWeek.Tuesday, TipoDeTreino.Leve, 35, 5.5, 4),
                    (DayOfWeek.Thursday, TipoDeTreino.Ritmo, 45, 7.0, 5))));
}
```

```csharp
// src/CoachTraining.DemoSeed/Scenarios/DivergenciaCargaRendimentoScenario.cs
using CoachTraining.DemoSeed.Contracts;
using CoachTraining.Domain.Enums;

namespace CoachTraining.DemoSeed.Scenarios;

public sealed class DivergenciaCargaRendimentoScenario(DateOnly referencia) : DemoScenarioBuilderBase(referencia)
{
    public override DemoScenarioSeed Build()
        => new(
            "divergencia-carga-rendimento",
            "Gustavo Nunes - Divergencia carga x rendimento",
            "demo.gustavo.nunes@coachtraining.local",
            "Avancado",
            null,
            5,
            "Carga sobe enquanto o pace medio piora.",
            null,
            Combinar(
                BlocoSemanal(
                    [11, 10, 9, 8],
                    (DayOfWeek.Monday, TipoDeTreino.Leve, 40, 8.0, 4),
                    (DayOfWeek.Wednesday, TipoDeTreino.Ritmo, 45, 9.0, 5),
                    (DayOfWeek.Saturday, TipoDeTreino.Longo, 90, 18.0, 6)),
                BlocoSemanal(
                    [7, 6, 5, 4],
                    (DayOfWeek.Monday, TipoDeTreino.Leve, 45, 8.2, 4),
                    (DayOfWeek.Wednesday, TipoDeTreino.Ritmo, 50, 9.0, 6),
                    (DayOfWeek.Saturday, TipoDeTreino.Longo, 95, 17.0, 6)),
                BlocoSemanal(
                    [3, 2, 1, 0],
                    (DayOfWeek.Monday, TipoDeTreino.Leve, 50, 8.0, 5),
                    (DayOfWeek.Wednesday, TipoDeTreino.Ritmo, 60, 8.4, 7),
                    (DayOfWeek.Thursday, TipoDeTreino.Intervalado, 65, 8.2, 7),
                    (DayOfWeek.Saturday, TipoDeTreino.Longo, 110, 14.0, 8))));
}
```

```csharp
// src/CoachTraining.DemoSeed/Scenarios/DemoScenarioFactory.cs
using CoachTraining.DemoSeed.Contracts;

namespace CoachTraining.DemoSeed.Scenarios;

public static class DemoScenarioFactory
{
    public static DemoProfileDefinition CreateProfile(string profile, DateOnly referencia)
        => profile switch
        {
            "demo-v1" => new DemoProfileDefinition(
                "demo-v1",
                "Professor Demo",
                "demo.professor@coachtraining.local",
                "Demo@123456",
                [
                    new BaseEstavelScenario(referencia).Build(),
                    new ConstrucaoSaudavelScenario(referencia).Build(),
                    new RiscoCargaAbruptaScenario(referencia).Build(),
                    new TaperBemExecutadoScenario(referencia).Build(),
                    new AderenciaBaixaScenario(referencia).Build(),
                    new DivergenciaCargaRendimentoScenario(referencia).Build()
                ]),
            _ => throw new InvalidOperationException($"Perfil de demo desconhecido: {profile}")
        };
}
```

- [ ] **Step 4: Run the targeted tests**

Run:

```bash
dotnet test tests/CoachTraining.Domain.Tests/CoachTraining.Tests.csproj --filter "FullyQualifiedName~DemoScenarioFactoryTests"
```

Expected: PASS with six scenarios, unique emails, one taper athlete, and the low-adherence scenario encoded.

- [ ] **Step 5: Commit the scenario catalog**

```bash
git add src/CoachTraining.DemoSeed/Contracts src/CoachTraining.DemoSeed/Scenarios tests/CoachTraining.Domain.Tests/DemoSeed/DemoScenarioFactoryTests.cs
git commit -m "feat: add deterministic demo scenarios"
```

### Task 3: Implement the database runner and demo-only cleanup

**Files:**
- Create: `src/CoachTraining.DemoSeed/DemoSeedRunner.cs`
- Test: `tests/CoachTraining.Domain.Tests/DemoSeed/DemoSeedRunnerTests.cs`

- [ ] **Step 1: Write the failing tests for seeding, reset, and determinism**

```csharp
// tests/CoachTraining.Domain.Tests/DemoSeed/DemoSeedRunnerTests.cs
using CoachTraining.App.Abstractions.Persistence;
using CoachTraining.App.Abstractions.Security;
using CoachTraining.App.Services;
using CoachTraining.DemoSeed;
using CoachTraining.Infra.Persistence;
using CoachTraining.Infra.Persistence.Repositories;
using CoachTraining.Infra.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CoachTraining.Tests.DemoSeed;

public class DemoSeedRunnerTests
{
    [Fact]
    public async Task RunAsync_ComResetDemo_DeveCriarProfessorSeisAtletasEHistorico()
    {
        await using var provider = BuildProvider(Guid.NewGuid().ToString("N"));
        var runner = provider.GetRequiredService<DemoSeedRunner>();

        var report = await runner.RunAsync(new DemoSeedOptions("demo-v1", true, false, false), new DateOnly(2026, 4, 2));
        var db = provider.GetRequiredService<CoachTrainingDbContext>();

        Assert.Equal("demo.professor@coachtraining.local", report.ProfessorEmail);
        Assert.Equal(6, report.Atletas.Count);
        Assert.Equal(6, db.Atletas.Count());
        Assert.True(db.SessoesDeTreino.Count() >= 60);
    }

    [Fact]
    public async Task RunAsync_ExecutadoDuasVezesComResetDemo_NaoDeveDuplicarDataset()
    {
        await using var provider = BuildProvider(Guid.NewGuid().ToString("N"));
        var runner = provider.GetRequiredService<DemoSeedRunner>();

        await runner.RunAsync(new DemoSeedOptions("demo-v1", true, false, false), new DateOnly(2026, 4, 2));
        await runner.RunAsync(new DemoSeedOptions("demo-v1", true, false, false), new DateOnly(2026, 4, 2));

        var db = provider.GetRequiredService<CoachTrainingDbContext>();
        Assert.Equal(1, db.Professores.Count());
        Assert.Equal(6, db.Atletas.Count());
    }

    private static ServiceProvider BuildProvider(string databaseName)
    {
        var services = new ServiceCollection();

        services.AddDbContext<CoachTrainingDbContext>(options => options.UseInMemoryDatabase(databaseName));
        services.AddScoped<IAtletaRepository, AtletaRepository>();
        services.AddScoped<IProfessorRepository, ProfessorRepository>();
        services.AddScoped<ISessaoDeTreinoRepository, SessaoDeTreinoRepository>();
        services.AddScoped<IProvaAlvoRepository, ProvaAlvoRepository>();
        services.AddScoped<IPasswordHasher, BCryptPasswordHasher>();
        services.AddScoped<CadastroProfessorService>();
        services.AddScoped<CadastroAtletaService>();
        services.AddScoped<CadastrarSessaoDeTreinoService>();
        services.AddScoped<GerenciarProvaAlvoService>();
        services.AddScoped<GerenciarPlanejamentoBaseService>();
        services.AddScoped<DemoSeedRunner>();

        return services.BuildServiceProvider();
    }
}
```

- [ ] **Step 2: Run the targeted tests to verify the failure**

Run:

```bash
dotnet test tests/CoachTraining.Domain.Tests/CoachTraining.Tests.csproj --filter "FullyQualifiedName~DemoSeedRunnerTests"
```

Expected: FAIL because `DemoSeedRunner` does not exist yet.

- [ ] **Step 3: Implement the runner using existing services and demo-only cleanup**

```csharp
// src/CoachTraining.DemoSeed/DemoSeedRunner.cs
using CoachTraining.App.DTOs;
using CoachTraining.App.Services;
using CoachTraining.DemoSeed.Reports;
using CoachTraining.DemoSeed.Scenarios;
using CoachTraining.Infra.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CoachTraining.DemoSeed;

public sealed class DemoSeedRunner
{
    private readonly CoachTrainingDbContext _dbContext;
    private readonly CadastroProfessorService _cadastroProfessorService;
    private readonly CadastroAtletaService _cadastroAtletaService;
    private readonly CadastrarSessaoDeTreinoService _cadastrarSessaoDeTreinoService;
    private readonly GerenciarProvaAlvoService _gerenciarProvaAlvoService;
    private readonly GerenciarPlanejamentoBaseService _gerenciarPlanejamentoBaseService;

    public DemoSeedRunner(
        CoachTrainingDbContext dbContext,
        CadastroProfessorService cadastroProfessorService,
        CadastroAtletaService cadastroAtletaService,
        CadastrarSessaoDeTreinoService cadastrarSessaoDeTreinoService,
        GerenciarProvaAlvoService gerenciarProvaAlvoService,
        GerenciarPlanejamentoBaseService gerenciarPlanejamentoBaseService)
    {
        _dbContext = dbContext;
        _cadastroProfessorService = cadastroProfessorService;
        _cadastroAtletaService = cadastroAtletaService;
        _cadastrarSessaoDeTreinoService = cadastrarSessaoDeTreinoService;
        _gerenciarProvaAlvoService = gerenciarProvaAlvoService;
        _gerenciarPlanejamentoBaseService = gerenciarPlanejamentoBaseService;
    }

    public async Task<DemoSeedReport> RunAsync(
        DemoSeedOptions options,
        DateOnly? referencia = null,
        CancellationToken cancellationToken = default)
    {
        var dataReferencia = referencia ?? DateOnly.FromDateTime(DateTime.UtcNow);
        var profile = DemoScenarioFactory.CreateProfile(options.Profile, dataReferencia);

        await _dbContext.Database.EnsureCreatedAsync(cancellationToken);

        if (options.ResetAll)
        {
            _dbContext.ProvasAlvo.RemoveRange(_dbContext.ProvasAlvo);
            _dbContext.SessoesDeTreino.RemoveRange(_dbContext.SessoesDeTreino);
            _dbContext.Atletas.RemoveRange(_dbContext.Atletas);
            _dbContext.Professores.RemoveRange(_dbContext.Professores);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
        else if (options.ResetDemo)
        {
            var demoProfessorIds = await _dbContext.Professores
                .Where(professor => professor.Email.StartsWith("demo."))
                .Select(professor => professor.Id)
                .ToListAsync(cancellationToken);

            if (demoProfessorIds.Count > 0)
            {
                var demoAtletaIds = await _dbContext.Atletas
                    .Where(atleta => demoProfessorIds.Contains(atleta.ProfessorId))
                    .Select(atleta => atleta.Id)
                    .ToListAsync(cancellationToken);

                _dbContext.ProvasAlvo.RemoveRange(_dbContext.ProvasAlvo.Where(prova => demoAtletaIds.Contains(prova.AtletaId)));
                _dbContext.SessoesDeTreino.RemoveRange(_dbContext.SessoesDeTreino.Where(sessao => demoAtletaIds.Contains(sessao.AtletaId)));
                _dbContext.Atletas.RemoveRange(_dbContext.Atletas.Where(atleta => demoProfessorIds.Contains(atleta.ProfessorId)));
                _dbContext.Professores.RemoveRange(_dbContext.Professores.Where(professor => demoProfessorIds.Contains(professor.Id)));
                await _dbContext.SaveChangesAsync(cancellationToken);
            }
        }

        var professor = _cadastroProfessorService.Cadastrar(new CriarProfessorDto
        {
            Nome = profile.ProfessorNome,
            Email = profile.ProfessorEmail,
            Senha = profile.ProfessorSenha
        });

        var atletas = new List<DemoSeedReportAtleta>(profile.Cenarios.Count);

        foreach (var scenario in profile.Cenarios)
        {
            var atleta = _cadastroAtletaService.Cadastrar(new CriarAtletaDto
            {
                Nome = scenario.Nome,
                Email = scenario.Email,
                ObservacoesClinicas = scenario.ObservacoesClinicas,
                NivelEsportivo = scenario.NivelEsportivo
            }, professor.Id);

            _gerenciarPlanejamentoBaseService.Salvar(atleta.Id, new SalvarPlanejamentoBaseDto
            {
                TreinosPlanejadosPorSemana = scenario.TreinosPlanejadosPorSemana
            }, professor.Id);

            if (scenario.ProvaAlvo is not null)
            {
                _gerenciarProvaAlvoService.Salvar(atleta.Id, new SalvarProvaAlvoDto
                {
                    DataProva = scenario.ProvaAlvo.DataProva,
                    DistanciaKm = scenario.ProvaAlvo.DistanciaKm,
                    Objetivo = scenario.ProvaAlvo.Objetivo
                }, professor.Id);
            }

            foreach (var sessao in scenario.Sessoes.OrderBy(sessao => sessao.Data))
            {
                _cadastrarSessaoDeTreinoService.Cadastrar(new CadastrarSessaoDeTreinoDto
                {
                    AtletaId = atleta.Id,
                    Data = sessao.Data,
                    Tipo = sessao.Tipo,
                    DuracaoMinutos = sessao.DuracaoMinutos,
                    DistanciaKm = sessao.DistanciaKm,
                    Rpe = sessao.Rpe
                }, professor.Id);
            }

            atletas.Add(new DemoSeedReportAtleta(atleta.Nome, scenario.Email, scenario.InsightEsperado));
        }

        return new DemoSeedReport(profile.Profile, professor.Email, profile.ProfessorSenha, atletas);
    }
}
```

- [ ] **Step 4: Run the targeted tests**

Run:

```bash
dotnet test tests/CoachTraining.Domain.Tests/CoachTraining.Tests.csproj --filter "FullyQualifiedName~DemoSeedRunnerTests"
```

Expected: PASS with one professor, six athletes, no duplication on rerun with `--reset-demo`, and a seeded session volume that fills the charts.

- [ ] **Step 5: Commit the runner**

```bash
git add src/CoachTraining.DemoSeed/DemoSeedRunner.cs tests/CoachTraining.Domain.Tests/DemoSeed/DemoSeedRunnerTests.cs
git commit -m "feat: implement demo dataset runner"
```

### Task 4: Format the final report and wire the real console host

**Files:**
- Create: `src/CoachTraining.DemoSeed/Reports/DemoSeedReport.cs`
- Create: `src/CoachTraining.DemoSeed/Reports/DemoSeedReportFormatter.cs`
- Modify: `src/CoachTraining.DemoSeed/Program.cs`
- Test: `tests/CoachTraining.Domain.Tests/DemoSeed/DemoSeedReportFormatterTests.cs`

- [ ] **Step 1: Write the failing test for the human-readable report**

```csharp
// tests/CoachTraining.Domain.Tests/DemoSeed/DemoSeedReportFormatterTests.cs
using CoachTraining.DemoSeed.Reports;

namespace CoachTraining.Tests.DemoSeed;

public class DemoSeedReportFormatterTests
{
    [Fact]
    public void Format_DeveImprimirCredenciaisEHeadlinesDosAtletas()
    {
        var report = new DemoSeedReport(
            "demo-v1",
            "demo.professor@coachtraining.local",
            "Demo@123456",
            [
                new DemoSeedReportAtleta("Ana Souza - Base estavel", "demo.ana.souza@coachtraining.local", "Sem alerta critico."),
                new DemoSeedReportAtleta("Diego Alves - Taper bem executado", "demo.diego.alves@coachtraining.local", "Taper adequado.")
            ]);

        var texto = DemoSeedReportFormatter.Format(report);

        Assert.Contains("demo.professor@coachtraining.local / Demo@123456", texto);
        Assert.Contains("Ana Souza - Base estavel -> Sem alerta critico.", texto);
        Assert.Contains("Diego Alves - Taper bem executado -> Taper adequado.", texto);
    }
}
```

- [ ] **Step 2: Run the report test to verify the failure**

Run:

```bash
dotnet test tests/CoachTraining.Domain.Tests/CoachTraining.Tests.csproj --filter "FullyQualifiedName~DemoSeedReportFormatterTests"
```

Expected: FAIL because the report types do not exist yet.

- [ ] **Step 3: Implement report types and upgrade `Program` to the final host-based execution**

```csharp
// src/CoachTraining.DemoSeed/Reports/DemoSeedReport.cs
namespace CoachTraining.DemoSeed.Reports;

public sealed record DemoSeedReport(
    string Profile,
    string ProfessorEmail,
    string ProfessorSenha,
    IReadOnlyList<DemoSeedReportAtleta> Atletas);

public sealed record DemoSeedReportAtleta(
    string Nome,
    string Email,
    string InsightEsperado);
```

```csharp
// src/CoachTraining.DemoSeed/Reports/DemoSeedReportFormatter.cs
using System.Text;

namespace CoachTraining.DemoSeed.Reports;

public static class DemoSeedReportFormatter
{
    public static string Format(DemoSeedReport report)
    {
        var builder = new StringBuilder();
        builder.AppendLine($"Perfil: {report.Profile}");
        builder.AppendLine($"Professor demo: {report.ProfessorEmail} / {report.ProfessorSenha}");

        foreach (var atleta in report.Atletas)
        {
            builder.AppendLine($"- {atleta.Nome} -> {atleta.InsightEsperado}");
        }

        return builder.ToString().TrimEnd();
    }
}
```

```csharp
// src/CoachTraining.DemoSeed/Program.cs
using CoachTraining.App.Services;
using CoachTraining.DemoSeed;
using CoachTraining.DemoSeed.Reports;
using CoachTraining.Infra;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var options = DemoSeedOptions.Parse(args);

if (options.HelpRequested)
{
    Console.WriteLine("Usage: dotnet run --project src/CoachTraining.DemoSeed -- --profile demo-v1 --reset-demo");
    Console.WriteLine("Flags: --profile <name> | --reset-demo | --reset-all | --help");
    return;
}

var builder = Host.CreateApplicationBuilder(args);

if (string.IsNullOrWhiteSpace(builder.Configuration.GetConnectionString("DefaultConnection")))
{
    throw new InvalidOperationException("ConnectionStrings:DefaultConnection deve estar configurada para executar o seed de demo.");
}

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddScoped<CadastroProfessorService>();
builder.Services.AddScoped<CadastroAtletaService>();
builder.Services.AddScoped<CadastrarSessaoDeTreinoService>();
builder.Services.AddScoped<GerenciarProvaAlvoService>();
builder.Services.AddScoped<GerenciarPlanejamentoBaseService>();
builder.Services.AddScoped<DemoSeedRunner>();

using var host = builder.Build();
using var scope = host.Services.CreateScope();

var runner = scope.ServiceProvider.GetRequiredService<DemoSeedRunner>();
var report = await runner.RunAsync(options);
Console.WriteLine(DemoSeedReportFormatter.Format(report));
```

- [ ] **Step 4: Run the targeted tests, then validate help output locally**

Run:

```bash
dotnet test tests/CoachTraining.Domain.Tests/CoachTraining.Tests.csproj --filter "FullyQualifiedName~DemoSeedReportFormatterTests"
dotnet run --project src/CoachTraining.DemoSeed -- --help
```

Expected:
- first command PASS
- second command prints the usage line and exits without touching the database

- [ ] **Step 5: Commit the console host and report formatting**

```bash
git add src/CoachTraining.DemoSeed/Program.cs src/CoachTraining.DemoSeed/Reports tests/CoachTraining.Domain.Tests/DemoSeed/DemoSeedReportFormatterTests.cs
git commit -m "feat: add demo seed report output"
```

### Task 5: Document the workflow and run the end-to-end verification

**Files:**
- Create: `docs/demo/demo-v1.md`
- Modify: `README.md`
- Modify: `docs/setup/ambiente.md`

- [ ] **Step 1: Add the demo dataset documentation**

````md
<!-- docs/demo/demo-v1.md -->
# Demo Dataset `demo-v1`

## Como gerar

```bash
cd infra
docker compose up -d db
cd ..
dotnet run --project src/CoachTraining.DemoSeed -- --profile demo-v1 --reset-demo
```

## Credenciais

- Professor: `demo.professor@coachtraining.local`
- Senha: `Demo@123456`

## Cenarios

- `Ana Souza - Base estavel`: carga controlada, sem alerta critico.
- `Bruno Lima - Construcao saudavel`: progressao consistente e sem risco alto.
- `Carla Mendes - Risco por carga abrupta`: ACWR e delta semanal elevados.
- `Diego Alves - Taper bem executado`: prova em 10 dias e reducao adequada de volume.
- `Fernanda Rocha - Aderencia baixa`: planejamento 5/semana, execucao recente de 2/semana.
- `Gustavo Nunes - Divergencia carga x rendimento`: carga sobe e o pace medio piora.
````

- [ ] **Step 2: Update the main docs to mention the seed command**

````md
<!-- README.md -->
## Dataset de demonstracao

Para preparar a base usada em apresentacoes:

```bash
dotnet run --project src/CoachTraining.DemoSeed -- --profile demo-v1 --reset-demo
```

O comando recria apenas o dataset `demo.*` e imprime as credenciais do professor demo e o resumo esperado de cada atleta.
````

````md
<!-- docs/setup/ambiente.md -->
## Dataset demo

Com o PostgreSQL local ou Docker em execucao:

```bash
dotnet run --project src/CoachTraining.DemoSeed -- --profile demo-v1 --reset-demo
```

O seed foi feito para apresentacao e feedback com treinadores. Por padrao, remove apenas dados `demo.*`.
````

- [ ] **Step 3: Run the full automated verification**

Run:

```bash
dotnet test CoachTraining.sln
```

Expected: PASS with the new seeder tests included.

- [ ] **Step 4: Run the end-to-end seed against the local PostgreSQL**

Run:

```bash
cd infra
docker compose up -d db
cd ..
dotnet run --project src/CoachTraining.DemoSeed -- --profile demo-v1 --reset-demo
```

Expected:
- command succeeds against PostgreSQL
- output contains `Professor demo: demo.professor@coachtraining.local / Demo@123456`
- output contains six athlete lines matching the six approved scenarios

- [ ] **Step 5: Commit the docs and verification updates**

```bash
git add README.md docs/setup/ambiente.md docs/demo/demo-v1.md
git commit -m "docs: document demo dataset workflow"
```

- [ ] **Step 6: Final review before handoff**

Run:

```bash
git status --short
git log --oneline -5
```

Expected:
- `git status --short` shows a clean working tree
- `git log --oneline -5` shows the four focused commits from this plan
