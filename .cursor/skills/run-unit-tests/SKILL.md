---
name: run-unit-tests
description: Define como executar testes unitários do CoachTraining com dotnet test (xUnit, net10.0). Acionar quando o usuário pedir para rodar testes, dotnet test, testes unitários, validação local ou saída detalhada de falhas.
---

# Testes unitários (CoachTraining)

## Contexto do repositório

- Solução: `CoachTraining.sln` (raiz do repo).
- Projeto de testes referenciado na solução: `tests/CoachTraining.Domain.Tests/CoachTraining.Tests.csproj` (xUnit + `Microsoft.NET.Test.Sdk`).
- Existe também `tests/CoachTraining.Tests.csproj` (layout alternativo com `Compile Include`); **preferir o projeto da pasta `CoachTraining.Domain.Tests` alinhado à solução**, salvo se o usuário indicar o outro.

## O que fazer

1. Executar a partir da **raiz do repositório** (`coach-training`).
2. Usar `dotnet test` (não inventar outro runner salvo pedido explícito).
3. Se o usuário quiser **mais detalhe em falhas**, combinar verbosidade e logger de console (ver abaixo).

## Comandos padrão

**Toda a solução (inclui o projeto de testes da sln):**

```bash
dotnet test CoachTraining.sln
```

**Somente o projeto de testes:**

```bash
dotnet test tests/CoachTraining.Domain.Tests/CoachTraining.Tests.csproj
```

**Configuração Release:**

```bash
dotnet test CoachTraining.sln -c Release
```

## Falhas com saída mais detalhada

```bash
dotnet test tests/CoachTraining.Domain.Tests/CoachTraining.Tests.csproj --verbosity detailed --logger "console;verbosity=detailed"
```

Máximo de ruído (depuração de host/build):

```bash
dotnet test tests/CoachTraining.Domain.Tests/CoachTraining.Tests.csproj --verbosity diagnostic
```

**Relatório TRX** (útil para abrir no VS ou inspecionar stack traces):

```bash
dotnet test tests/CoachTraining.Domain.Tests/CoachTraining.Tests.csproj --logger "trx;LogFileName=TestResults/resultados.trx"
```

## Filtros e ciclo rápido

**Filtrar por nome (VSTest):**

```bash
dotnet test tests/CoachTraining.Domain.Tests/CoachTraining.Tests.csproj --filter "FullyQualifiedName~NomeDaClasseOuMetodo"
```

**Sem rebuild** (após build recente):

```bash
dotnet test tests/CoachTraining.Domain.Tests/CoachTraining.Tests.csproj --no-build
```

## Cobertura (opcional)

O projeto referencia `coverlet.collector`. Para coletar cobertura:

```bash
dotnet test tests/CoachTraining.Domain.Tests/CoachTraining.Tests.csproj --collect:"XPlat Code Coverage"
```

## Resumo para o agente

- Rodar testes: `dotnet test` na sln ou no `.csproj` acima.
- Falhas pouco claras: adicionar `--verbosity detailed` e `--logger "console;verbosity=detailed"`.
- Não assumir que `tests/CoachTraining.Tests.csproj` é o alvo principal sem checar a solução ou o pedido do usuário.
