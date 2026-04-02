# Princípios Arquiteturais - CoachTraining

## 🏛️ Clean Architecture

O projeto segue os princípios de **Clean Architecture** para garantir independência de frameworks, testabilidade e manutenibilidade.

### Estrutura de Camadas

```
┌────────────────────────────────┐
│      Frameworks & Tools        │
│  (Web, Database, UI, etc)      │
└────────────────────────────────┘
            ↓↑
┌────────────────────────────────┐
│    Controllers & Presenters     │
│      (API Layer / API.csproj)   │
└────────────────────────────────┘
            ↓↑
┌────────────────────────────────┐
│   Use Cases / Interactors       │
│     (App Layer / App.csproj)    │
└────────────────────────────────┘
            ↓↑
┌────────────────────────────────┐
│  Entities & Business Rules      │
│   (Domain Layer / Domain.csproj)│
└────────────────────────────────┘
            ↓↑
┌────────────────────────────────┐
│ DB, External Services, Gateways │
│  (Infra Layer / Infra.csproj)   │
└────────────────────────────────┘
```

### Regra de Dependência

> **As dependências devem apontar SEMPRE para o núcleo (Domain Layer)**

```
API     → depends on → Application, Infrastructure, Domain
App     → depends on → Infrastructure, Domain
Infra   → depends on → Domain
Domain  → depends on → NINGUÉM (sem dependências externas)
```

### Benefícios

✅ **Independência de Framework**: A lógica de negócio não conhece ASP.NET
✅ **Testabilidade**: Domain e Application podem ser testados sem HTTP
✅ **Flexibilidade**: Trocar DB, framework ou entrega sem afetar domínio
✅ **Clareza**: Cada camada tem responsabilidade bem definida

---

## 🎯 Domain-Driven Design (DDD)

CoachTraining implementa padrões de **DDD** para modelar a complexidade do domínio de treinamento esportivo.

### Conceitos Principais

#### **Entities (Entidades)**
Objetos com identidade única que mudam ao longo do tempo.

Exemplos em CoachTraining:
- `Coach`: Identidade permanente, pode ser modificado
- `Atleta`: Identidade permanente, muda conforme progride
- `SessaoDeTreino`: Identidade única, representa um treino realizado

```csharp
public class Atleta
{
    public Guid Id { get; private set; }
    public string Nome { get; private set; }
    public string Observacoes { get; set; }
    // ... mais propriedades
    
    // Valor Object
    public CargaTreino CargaUltimaSemana { get; private set; }
}
```

#### **Value Objects (Objetos de Valor)**
Objetos sem identidade, imutáveis, que representam conceitos do domínio.

Exemplos em CoachTraining:
- `RPE`: Escala 1-10 de esforço percebido (não tem ID)
- `CargaTreino`: Resultado de Duração × RPE
- `Pace`: Velocidade por quilômetro

```csharp
public readonly struct RPE
{
    public int Valor { get; }
    
    public RPE(int valor)
    {
        if (valor < 1 || valor > 10)
            throw new ArgumentException("RPE deve estar entre 1 e 10");
        Valor = valor;
    }
    
    public static implicit operator int(RPE rpe) => rpe.Valor;
}
```

#### **Aggregates (Agregados)**
Grupos de Entities e Value Objects que formam uma unidade coerente.

Exemplo:
- `AtletaAggregate`: Contém Atleta + ProvaAlvo + Planejamento

#### **Domain Services (Serviços de Domínio)**
Lógica de negócio que não pertence a uma Entity específica.

Exemplos em CoachTraining:
- `CalculadoraDeCarga`: Calcula carga de treino
- `AvaliadorDeRisco`: Avalia nível de risco (ACWR, overreaching)
- `ClassificadorDeFase`: Classifica a fase do treinamento

```csharp
public class CalculadoraDeCarga
{
    public CargaTreino Calcular(Duracao duracao, RPE rpe)
    {
        return new CargaTreino(duracao.EmMinutos * rpe.Valor);
    }
    
    public CargaTreino CalcularSemanal(List<CargaTreino> cargas)
    {
        return new CargaTreino(cargas.Sum(c => c.Valor));
    }
}
```

#### **Ubiquitous Language (Linguagem Única)**
Linguagem comum entre developers e domain experts.

Em CoachTraining:
- **RPE**: Rating of Perceived Exertion
- **Carga de Treino**: Duration × RPE
- **ACWR**: Acute : Chronic Workload Ratio
- **Taper**: Redução de volume pré-prova
- **CargaCrônica**: Média móvel de 4 semanas

### Estrutura de Pastas

```
CoachTraining.Domain/
├── Entities/
│   ├── Coach.cs
│   ├── Atleta.cs
│   ├── ProvaAlvo.cs
│   └── SessaoDeTreino.cs
├── ValueObjects/
│   ├── RPE.cs
│   ├── CargaTreino.cs
│   └── Pace.cs
├── Enums/
│   ├── TipoDeTreino.cs
│   └── FaseDoCiclo.cs
├── Services/
│   ├── CalculadoraDeCarga.cs
│   ├── AvaliadorDeRisco.cs
│   └── ClassificadorDeFase.cs
├── Interfaces/
│   └── IRepository.cs
└── Exceptions/
    └── DomainException.cs
```

---

## 🔄 Fluxo de Dados

```
Request HTTP (GET /atleta/123)
    ↓
[API Controller] HealthCheckController
    ↓
[Application Service] AtletaService
    ↓
[Domain Service] CalculadoraDeCarga, AvaliadorDeRisco
    ↓
[Domain Entity] Atleta, SessaoDeTreino
    ↓
[Infrastructure] AtletaRepository
    ↓
[Database] SQL Server
    ↓
Response DTO
    ↓
JSON Response
```

---

## 📋 Checklist para Novas Features

Ao implementar uma nova funcionalidade, siga:

1. **Define a regra de negócio**
   - É cálculo? → Domain Service
   - É validação? → Entity ou Value Object
   - É coordenação? → Application Service

2. **Modela no Domain**
   - Cria Entities / Value Objects se necessário
   - Cria Domain Service se necessário
   - Escreve testes no Domain

3. **Expõe via Application**
   - Cria Use Case / Application Service
   - Define DTOs de entrada/saída
   - Escreve testes de integração

4. **Integra na API**
   - Cria Controller ou endpoint
   - Configura DI
   - Documenta o endpoint

---

## 🧪 Testabilidade

A arquitetura Clean + DDD permite:

```csharp
// ✅ Teste de Domain (sem mocks, sem HTTP, sem DB)
[Fact]
public void CalculadoraDeCarga_DeveCalcularCorretamente()
{
    var duracao = new Duracao(60);
    var rpe = new RPE(8);
    var calculadora = new CalculadoraDeCarga();
    
    var resultado = calculadora.Calcular(duracao, rpe);
    
    Assert.Equal(480, resultado.Valor);
}

// ✅ Teste de Application (com mocks de Repository)
[Fact]
public async Task AtletaService_DeveRetornarAtletaComCargas()
{
    var mockRepository = new Mock<IAtletaRepository>();
    var service = new AtletaService(mockRepository.Object);
    
    // ...
}

// ✅ Teste de API (com TestClient)
[Fact]
public async Task HealthCheck_DeveRetornar200()
{
    var client = _factory.CreateClient();
    var response = await client.GetAsync("/api/healthcheck");
    
    Assert.Equal(StatusCodes.Status200OK, (int)response.StatusCode);
}
```

---

## 📝 Padrões de Código

### Naming Conventions
- **PascalCase**: Classes, Métodos, Properties públicas
- **camelCase**: Parâmetros, Variáveis locais
- **UPPER_CASE**: Constantes
- **_camelCase**: Fields privados

### Nullable Reference Types

```csharp
#nullable enable

public class Coach
{
    public string Nome { get; set; } // Obrigatório
    public string? Email { get; set; } // Opcional
}
```

### Imutabilidade em Value Objects

```csharp
public readonly struct CargaTreino
{
    public int Valor { get; }
    
    public CargaTreino(int valor) => Valor = valor;
}
```

---

## 🚀 Evolução

A arquitetura permite evolução sem breaking changes:

- ✅ Adicionar novos Value Objects
- ✅ Adicionar novos Domain Services
- ✅ Adicionar novos Controllers
- ✅ Trocar implementação de Repository
- ❌ Mudar core de Domain (afeta tudo)

---

## 📖 Referências

- **Clean Architecture** - Robert C. Martin
- **Domain-Driven Design** - Eric Evans
- **Building Microservices** - Sam Newman

---

**Última atualização**: Semana 1
