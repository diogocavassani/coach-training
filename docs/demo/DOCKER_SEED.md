# Demo Seed via Docker

Execução do gerador de dados demo via Docker Compose.

## Quick Start

```bash
cd infra

# Iniciar banco + seed em Docker
docker compose --profile seed up --build demoseed

# Saida esperada:
# ✅ Dataset criado com sucesso!
#    6 atletas com dados históricos e insights
#    Professor: demo.professor@coachtraining.local / Demo@123456
```

## Estrutura

### Dockerfile

O Dockerfile em `src/CoachTraining.DemoSeed/Dockerfile` utiliza:

1. **Build Stage**: .NET SDK 10.0 para compilação
   - Copia apenas arquivos necessários (não testes)
   - Resolução de dependências
   - `dotnet publish -c Release` para gerar binários com dependências

2. **Runtime Stage**: ASP.NET Runtime 10.0
   - Imagem enxuta com suporte a EF Core
   - Copia artefatos de build e appsettings
   - Variáveis de ambiente para configuração PostgreSQL

### Docker Compose Integration

**Configuração em `infra/docker-compose.yml`**:

```yaml
demoseed:
  build:
    context: ..
    dockerfile: src/CoachTraining.DemoSeed/Dockerfile
  profiles: ["seed"]  # Opcional - rodar só com --profile seed
  environment:
    ASPNETCORE_ENVIRONMENT: Development
    ConnectionStrings__DefaultConnection: >-
      Host=db;Port=5432;Database=coachtraining;...
  depends_on:
    db:
      condition: service_healthy
```

**Uso**:

```bash
# Rodar apenas o seed
docker compose --profile seed up demoseed

# Rodar com rebuild
docker compose --profile seed up --build demoseed

# Limpar containers
docker compose --profile seed down
```

## Configuração

### EF Core Version

Atualmente usando **Microsoft.EntityFrameworkCore `10.0.4`** (alinhado com disponibilidade em NuGet).

Versões definidas em [Directory.Build.props](../../Directory.Build.props):

```xml
<PackageReference Update="Microsoft.EntityFrameworkCore" Version="10.0.4" />
<PackageReference Update="Microsoft.EntityFrameworkCore.Relational" Version="10.0.4" />
```

### Program.cs Configuration Logic

Path resolution automático para appsettings (funciona em dev e Docker):

```csharp
var possiblePaths = new[]
{
    Path.Combine(Directory.GetCurrentDirectory(), "src", "CoachTraining.DemoSeed"),
    Directory.GetCurrentDirectory(),
    "/app"
};

var configBasePath = possiblePaths.FirstOrDefault(p => 
    Directory.Exists(p) && (
        File.Exists(Path.Combine(p, "appsettings.json")) || 
        File.Exists(Path.Combine(p, "appsettings.Development.json"))
    )
) ?? Directory.GetCurrentDirectory();
```

## Troubleshooting

### Container exits immediately

Verifique os logs:
```bash
docker compose --profile seed logs demoseed
```

Possíveis causas:
- PostgreSQL não está saudável: `depends_on` aguarda healthcheck
- Arquivo de configuração não encontrado
- Erro de conexão ao banco

### Port conflicts

PostgreSQL usa `5432`. Se já houver uma instância rodando localmente:

```bash
docker ps  # Verificar containers ativos
docker network ls  # Verificar redes
```

### Build errors

Limpar cache:
```bash
docker builder prune -a
docker compose --profile seed down -v && docker compose --profile seed up --build demoseed
```

## Next Steps

- Adicionar mais cenários ao seed
- Integrar seed como pré-step em CI/CD
- Considerar seed automático ao iniciar stack `docker compose up`
