# coach-training

![Status](https://img.shields.io/badge/status-em_desenvolvimento-blue)
![.NET Version](https://img.shields.io/badge/.NET-10.0-512BD4)
![License](https://img.shields.io/badge/license-MIT-green)

## 📋 Sobre o Projeto

**CoachTraining** é um sistema de apoio à decisão para treinadores de corrida, baseado em evidências científicas da fisiologia do exercício. O sistema consolida dados de treino, aplica regras científicas e gera indicadores para auxiliar o treinador no monitoramento da preparação do atleta para uma prova alvo.

### Visão
- 🎯 **Não prescreve** treinos
- 🎯 **Não substitui** o treinador
- 🎯 **Suporta decisão** com dados baseados em ciência
- 🎯 **Centraliza e padroniza** análises de carga de treino

## 🏛️ Arquitetura

O projeto segue os princípios de **Clean Architecture** e **Domain-Driven Design (DDD)**:

```
CoachTraining.Api/              # Presentation Layer (Controllers, HTTP)
├── Controllers/
├── Middleware/
└── Program.cs

CoachTraining.App/              # Application Layer (Use Cases, DTOs)
├── UseCases/
├── DTOs/
└── Services/

CoachTraining.Domain/           # Domain Layer (Entities, Value Objects, Rules)
├── Entities/
├── ValueObjects/
├── Services/
├── Enums/
└── Exceptions/

CoachTraining.Infra/            # Infrastructure Layer (Database, External Services)
├── Persistence/
├── Repositories/
└── Services/
```

### Dependências Entre Camadas
```
API → Application, Infrastructure, Domain
Application → Infrastructure, Domain
Infrastructure → Domain
Domain → (sem dependências)
```

## 🚀 Começando

### Pré-requisitos
- .NET 10.0 SDK
- SQL Server (ou SQL Server Express)
- Git

### Setup Inicial

1. Clone o repositório:
```bash
git clone https://github.com/seu-usuario/coach-training.git
cd coach-training
```

2. Restaure as dependências:
```bash
dotnet restore
```

3. Build da solução:
```bash
dotnet build
```

4. Execute a API:
```bash
cd src/CoachTraining.Api
dotnet run
```

A API estará disponível em: `https://localhost:5001`

## 📊 Health Check

Verifique se a API está rodando:

```bash
GET /health-check
```

Resposta esperada:
```json
{
	"status": "healthy",
	"timestamp": "2025-12-16T10:30:45.1234567Z",
	"version": "1.0.0",
	"environment": "Development"
}
```

## 🔬 Fundamentos Científicos

O sistema implementa regras baseadas em literatura científica:

- **Session-RPE**: Carga = Duração (min) × RPE (1-10)
- **Carga Crônica**: Média semanal das últimas 4 semanas
- **ACWR**: Razão entre carga aguda e carga crônica
	- < 0,8: Destreinamento
	- 0,8-1,3: Zona segura ✓
	- ≥ 1,5: Alerta de risco

- **Fases do Treinamento**: Base, Construção, Pico, Polimento (Taper)

## 📅 Roadmap (10 Semanas)

- **Semana 1**: Setup, Arquitetura, Health Check ✅
- **Semana 2**: Modelagem de Domínio (Entidades, Value Objects)
- **Semana 3**: Cálculo de Carga de Treino
- **Semana 4**: ACWR e Progressão de Carga
- **Semana 5**: Fases do Treinamento e Taper
- **Semana 6**: Read Model e Dashboard
- **Semana 7**: Geração de Insights
- **Semana 8**: Testes de Cenários Reais
- **Semana 9**: Refinamento e Robustez
- **Semana 10**: Documentação e Fechamento

## 📚 Documentação

- [Visão Geral do Projeto](docs/projeto_sistema_de_monitoramento_de_carga_e_preparacao_de_atletas.md)
- [Roadmap de Implementação](docs/roadmap_de_implementacao_tarefas_por_semana.md)
- [Princípios Arquiteturais](docs/ARQUITETURA.md)

## 🛠️ Stack Tecnológico

| Camada | Tecnologia |
|--------|-----------|
| **Backend** | .NET 10.0 (C#) |
| **API** | ASP.NET Core |
| **Banco** | SQL Server |
| **ORM** | Entity Framework Core |
| **Testes** | XUnit |
| **Logging** | Serilog (planejado) |

## 🧪 Testes

Execute os testes da solução:

```bash
dotnet test
```

## 📝 Padrões de Código

O projeto segue:
- ✅ Padrão de código C# conforme `.editorconfig`
- ✅ Nullable reference types habilitado
- ✅ Implicit usings habilitado
- ✅ Princípios SOLID
- ✅ DDD e Clean Architecture

## 🤝 Contribuindo

Este é um projeto pessoal de desenvolvimento. Sugestões são bem-vindas via issues.

## 📄 Licença

MIT License - veja [LICENSE](LICENSE) para detalhes.

## 👨‍💻 Autor

Desenvolvido como projeto de estudo em arquitetura de software e ciência do esporte.

---

**Status**: Em desenvolvimento (Semana 1 completada) ✅