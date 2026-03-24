# Architecture - CoachTraining

## Visão Geral

**CoachTraining** é um sistema web ASP.NET Core 8.0 para monitoramento de carga e preparação de atletas. Implementa **Clean Architecture** com princípios de **Domain-Driven Design (DDD)** para garantir uma solução escalável, testável e maintível.

**Tecnologias Principais:**
- ASP.NET Core 8.0
- C# 12
- Domain-Driven Design (DDD)
- Clean Architecture
- Entity Framework Core (future)

---

## Estrutura de Camadas

```
┌────────────────────────────────┐
│      API Layer (Presentation)   │
│  Controllers + HTTP Endpoints   │
│   CoachTraining.Api.csproj      │
└────────────────────────────────┘
            ↓↑
┌────────────────────────────────┐
│  Application Layer (Use Cases)  │
│    Services + DTOs + Mappers    │
│  CoachTraining.App.csproj       │
└────────────────────────────────┘
            ↓↑
┌────────────────────────────────┐
│ Domain Layer (Business Logic)   │
│ Entities + Value Objects + DomainServices │
│  CoachTraining.Domain.csproj    │
└────────────────────────────────┘
            ↓↑
┌────────────────────────────────┐
│ Infrastructure (Data & External)│
│    Repositories + Gateways      │
│  CoachTraining.Infra.csproj     │
└────────────────────────────────┘
```

### Camadas Detalhadas

#### **1. API Layer (CoachTraining.Api)**
- **Responsabilidade**: Receber requisições HTTP, validar entrada, chamar Application Services, retornar respostas
- **Componentes**:
  - `Controllers/`: Endpoints REST (ex: `DashboardController`, `AtletaController`)
  - `Program.cs`: Configuração de DI e middleware
  - `appsettings.json`: Configurações de ambiente

#### **2. Application Layer (CoachTraining.App)**
- **Responsabilidade**: Orquestra Domain Services e Entities, coordena fluxos de negócio, mapeia DTOs
- **Componentes**:
  - `Services/`: Application Services (ex: `ObterDashboardAtletaService`, `CadastroAtletaService`)
  - `DTOs/`: Data Transfer Objects (ex: `DashboardAtletaDto`, `CriarAtletaDto`)

#### **3. Domain Layer (CoachTraining.Domain)**
- **Responsabilidade**: Contém a lógica de negócio pura, independente de frameworks
- **Componentes**:
  - `Entities/`: Entidades com identidade (ex: `Atleta`, `Coach`, `SessaoDeTreino`)
  - `ValueObjects/`: Objetos imutáveis sem identidade (ex: `CargaTreino`, `RPE`)
  - `Services/`: Domain Services que coordenam lógica complexa (ex: `CalculadoraDeCarga`, `AvaliadorDeRisco`, `ClassificadorDeFase`)
  - `Enums/`: Enumerações de domínio (ex: `FaseDoCiclo`, `StatusDeRisco`, `TipoDeTreino`)

#### **4. Infrastructure Layer (CoachTraining.Infra)**
- **Responsabilidade**: Implementações técnicas (DB, APIs externas, etc)
- **Componentes**:
  - `Repositories/`: Implementações de interfaces de persistência
  - `Gateways/`: Integrações com APIs externas

---

## Padrões Adotados

### 1. Clean Architecture
- Regra de dependência: **sempre apontam para o Domain**
- Independência de frameworks
- Testabilidade máxima

### 2. Domain-Driven Design (DDD)
- **Entities**: `Coach`, `Atleta`, `SessaoDeTreino` (têm ID e ciclo de vida)
- **Value Objects**: `CargaTreino`, `RPE`, `Pace` (imutáveis, sem ID)
- **Domain Services**: Lógica que não pertence a uma Entity específica
- **Enums**: Conceitos fechados do domínio (`FaseDoCiclo`, `StatusDeRisco`)
- **Ubiquitous Language**: Termos específicos de treinamento esportivo

### 3. Padrões de Projeto
- **Dependency Injection (DI)**: ASP.NET Core built-in
- **Repository Pattern** (future): Abstração de acesso a dados
- **Service Locator**: Usado para Application Services

### 4. Linguagem Única (Ubiquitous Language)
- **RPE**: Rating of Perceived Exertion (1-10)
- **Carga de Treino**: Duração × RPE
- **ACWR**: Acute : Chronic Workload Ratio (indicador de risco)
- **Taper**: Redução de volume pré-prova (7-21 dias)
- **Carga Aguda**: Última semana (7 dias)
- **Carga Crônica**: Média das últimas 4 semanas
- **Fase do Ciclo**: Base, Construção, Especificação, Taper, Recuperação

---

## Convenções de Código

### Nomenclatura
- **Classes**: `PascalCase` (ex: `DashboardAtletaDto`, `CalculadoraDeCarga`)
- **Métodos**: `PascalCase` (ex: `ObterDashboard`, `CalcularCarga`)
- **Propriedades públicas**: `PascalCase` (ex: `Nome`, `CargaSemanal`)
- **Parâmetros**: `camelCase` (ex: `atleta`, `id`, `sessoes`)
- **Variáveis locais**: `camelCase` (ex: `hoje`, `resultado`)
- **Campos privados**: `_camelCase` (ex: `_dashboardService`)
- **Constantes**: `UPPER_CASE` (ex: `JANELA_TAPER_DIAS = 21`)

### Organização de Arquivos
```
CoachTraining.Domain/
├── Entities/
│   ├── Coach.cs
│   ├── Atleta.cs
│   ├── ProvaAlvo.cs
│   └── SessaoDeTreino.cs
├── ValueObjects/
│   ├── CargaTreino.cs
│   ├── RPE.cs
│   └── Pace.cs
├── Enums/
│   ├── FaseDoCiclo.cs
│   ├── StatusDeRisco.cs
│   └── TipoDeTreino.cs
└── Services/
    ├── CalculadoraDeCarga.cs
    ├── AvaliadorDeRisco.cs
    └── ClassificadorDeFase.cs

CoachTraining.App/
├── DTOs/
│   ├── DashboardAtletaDto.cs
│   └── CriarAtletaDto.cs
└── Services/
    ├── ObterDashboardAtletaService.cs
    └── CadastroAtletaService.cs

CoachTraining.Api/
└── Controllers/
    ├── DashboardController.cs
    ├── HealthCheckController.cs
    └── AtletaController.cs
```

### Estilo de Código
- **Null checks**: Usar `ArgumentNullException` e `??` operator
- **Imutabilidade**: Value Objects com `readonly` fields
- **Validações**: No construtor de Entities/Value Objects
- **Logging**: Injetar `ILogger<T>` em Controllers
- **XML Comments**: Documentar públicos (Classes, Métodos, Propriedades)

---

## Arquivos de Referência por Tipo

### Controllers
- `CoachTraining.Api/Controllers/DashboardController.cs` - Exemplo de GET consolidado com logging
- `CoachTraining.Api/Controllers/HealthCheckController.cs` - Exemplo simples de GET
- `CoachTraining.Api/Controllers/AtletaController.cs` - Exemplo de POST com cadastro

### Application Services
- `CoachTraining.App/Services/ObterDashboardAtletaService.cs` - Orquestração de Domain Services
- `CoachTraining.App/Services/CadastroAtletaService.cs` - Cadastro de atletas

### Domain Services
- `CoachTraining.Domain/Services/CalculadoraDeCarga.cs` - Cálculos (referência futura)
- `CoachTraining.Domain/Services/AvaliadorDeRisco.cs` - Análise de risco (referência futura)
- `CoachTraining.Domain/Services/ClassificadorDeFase.cs` - Classificação (referência futura)

### Entities
- `CoachTraining.Domain/Entities/Atleta.cs` - Entity com validação no construtor
- `CoachTraining.Domain/Entities/Coach.cs` - Entity simples

### DTOs
- `CoachTraining.App/DTOs/DashboardAtletaDto.cs` - DTO complexo com coleções
- `CoachTraining.App/DTOs/CriarAtletaDto.cs` - DTO de requisição de cadastro
- `CoachTraining.App/DTOs/AtletaDto.cs` - DTO de resposta de cadastro

---

## Fluxo de Dados - Exemplo GET Dashboard

```
1. HTTP GET /api/dashboard/atleta/{id}
          ↓
2. DashboardController.ObterDashboard(id)
          ↓
3. Valida ID (empty check)
          ↓
4. ObterDashboardAtletaService.ObterDashboard(atleta, sessoes, prova)
          ↓
5. Domain Services:
   - CalculadoraDeCarga.AgregarCargaDiaria(sessoes)
   - AvaliadorDeRisco.CalcularAcwr(aguda, cronica)
   - ClassificadorDeFase.ClassificarFase(cargas, hoje, prova)
          ↓
6. Mapeia para DashboardAtletaDto
          ↓
7. Retorna JSON: 200 OK
```

---

## Fluxo de Dados - Exemplo POST Cadastro Atleta (A IMPLEMENTAR)

```
1. HTTP POST /api/atleta
   Body: { "nome": "João", "nivelEsportivo": "Elite", ... }
          ↓
2. AtletaController.CadastrarAtleta(criarAtletaDto)
          ↓
3. Valida DTO
          ↓
4. CadastroAtletaService.Cadastrar(dto)
          ↓
5. Cria Entity: new Atleta(nome, obs, nivel)
          ↓
6. Persiste via AtletaRepository.Add(atleta) [future]
          ↓
7. Mapeia para AtletaDto (response)
          ↓
8. Retorna JSON: 201 Created
```

---

## Comandos do Projeto

### Build e Compilação
```bash
# Compilar solução
dotnet build

# Compilar projeto específico
dotnet build src/CoachTraining.Api

# Compilar com log verboso
dotnet build --verbosity diagnostic
```

### Testes
```bash
# Rodar todos os testes
dotnet test

# Rodar com verbose
dotnet test --verbosity normal

# Rodar testes específicos
dotnet test --filter "Category=Domain"
```

### Execução
```bash
# Rodar API
dotnet run --project src/CoachTraining.Api

# Rodar com watch (reload automático)
dotnet watch --project src/CoachTraining.Api

# Definir ambiente
set ASPNETCORE_ENVIRONMENT=Development
dotnet run --project src/CoachTraining.Api
```

### Formatação
```bash
# Formatar código
dotnet format

# Verificar formatação (sem aplicar)
dotnet format --verify-no-changes
```

### NuGet
```bash
# Listar pacotes instalados
dotnet list package

# Verificar pacotes desatualizados
dotnet list package --outdated
```

---

## Checklist para Novas Features

Ao implementar uma nova funcionalidade, siga este fluxo:

1. **Defina a regra de negócio**
   - É cálculo/análise? → Domain Service
   - É validação/conceito? → Entity ou Value Object
   - É coordenação? → Application Service

2. **Modele no Domain**
   - Crie Entities/Value Objects se necessário
   - Crie Domain Services se necessário
   - Escreva testes do Domain

3. **Exponha via Application**
   - Crie Application Service
   - Defina DTOs de entrada/saída
   - Escreva testes de integração

4. **Integre na API**
   - Crie ou estenda Controller
   - Configure DI no Program.cs
   - Documente o endpoint com XML comments

5. **Teste**
   - Compile: `dotnet build`
   - Teste: `dotnet test`
   - Valide: `dotnet run` + testar endpoint

---

## Estrutura de Pasta Completa

```
coach-training/
├── ARCHITECTURE.md (este arquivo)
├── CoachTraining.sln
├── README.md
├── docs/
│   ├── ARQUITETURA.md
│   ├── projeto_sistema_de_monitoramento_de_carga_e_preparacao_de_atletas.md
│   └── roadmap_de_implementacao_tarefas_por_semana.md
├── src/
│   ├── CoachTraining.Api/
│   │   ├── CoachTraining.Api.csproj
│   │   ├── Program.cs
│   │   ├── Controllers/
│   │   │   ├── DashboardController.cs
│   │   │   ├── HealthCheckController.cs
│   │   │   └── AtletaController.cs (a criar)
│   │   ├── appsettings.json
│   │   └── appsettings.Development.json
│   ├── CoachTraining.App/
│   │   ├── CoachTraining.App.csproj
│   │   ├── Services/
│   │   │   ├── ObterDashboardAtletaService.cs
│   │   │   └── CadastroAtletaService.cs (a criar)
│   │   └── DTOs/
│   │       ├── DashboardAtletaDto.cs
│   │       └── CriarAtletaDto.cs (a criar)
│   ├── CoachTraining.Domain/
│   │   ├── CoachTraining.Domain.csproj
│   │   ├── Entities/
│   │   │   ├── Atleta.cs
│   │   │   ├── Coach.cs
│   │   │   ├── ProvaAlvo.cs
│   │   │   └── SessaoDeTreino.cs
│   │   ├── ValueObjects/
│   │   │   ├── CargaTreino.cs
│   │   │   ├── RPE.cs
│   │   │   └── Pace.cs
│   │   ├── Enums/
│   │   │   ├── FaseDoCiclo.cs
│   │   │   ├── StatusDeRisco.cs
│   │   │   └── TipoDeTreino.cs
│   │   └── Services/
│   │       ├── CalculadoraDeCarga.cs
│   │       ├── AvaliadorDeRisco.cs
│   │       └── ClassificadorDeFase.cs
│   └── CoachTraining.Infra/
│       ├── CoachTraining.Infra.csproj
│       └── (futura: Repositories, Gateways)
└── tests/
    ├── CoachTraining.Domain.Tests/
    └── CoachTraining.Tests.csproj
```

---

## Dependências entre Projetos

```
CoachTraining.Api
  ├─ depends on → CoachTraining.App
  ├─ depends on → CoachTraining.Domain
  └─ depends on → CoachTraining.Infra

CoachTraining.App
  ├─ depends on → CoachTraining.Domain
  └─ depends on → CoachTraining.Infra

CoachTraining.Infra
  └─ depends on → CoachTraining.Domain

CoachTraining.Domain
  └─ depends on → NOBODY ✓
```

---

## Próximas Etapas (Roadmap)

- [ ] Implementar AtletaController com POST /cadastro
- [ ] Criar CadastroAtletaService
- [ ] Implementar Repository Pattern
- [ ] Integrar Entity Framework Core
- [ ] Implementar SessaoDeTreinoController
- [ ] Adicionar autenticação/autorização
- [ ] Implementar paginação em listagens
- [ ] Adicionar testes automatizados

---

**Última atualização**: 24/02/2026
**Status**: Architecture base documentada, pronto para implementações de features
