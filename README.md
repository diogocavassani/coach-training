# coach-training

![Status](https://img.shields.io/badge/status-MVP_funcional-brightgreen)
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

O projeto segue os princípios de **Clean Architecture** e **Domain-Driven Design (DDD)** no backend:

```
src/CoachTraining.Api/          # Presentation Layer (Controllers, HTTP)
src/CoachTraining.App/          # Application Layer (Use Cases, Services)
src/CoachTraining.Domain/       # Domain Layer (Entidades, Regras, Value Objects)
src/CoachTraining.Infra/        # Infrastructure Layer (Persistencia, Integracoes)
frontend/                       # Front-end Angular (SPA)
infra/                          # Docker Compose e configuracoes de ambiente
```

### Dependências Entre Camadas
```
API → Application, Infrastructure, Domain
Application → Infrastructure, Domain
Infrastructure → Domain
Domain → (sem dependências)
```

## 🚀 Comecando

### Pré-requisitos
- .NET 10.0 SDK
- Docker Desktop (ou Docker Engine + Compose)
- Node.js 22+ (opcional, para rodar front-end fora do Docker)
- Git

### Setup inicial

1. Clone o repositório:
```bash
git clone https://github.com/seu-usuario/coach-training.git
cd coach-training
```

2. (Opcional) Copie o arquivo de variaveis do Docker:
```bash
cd infra
cp .env.example .env
```

3. Suba todo o ambiente (banco + API + front-end):
```bash
docker compose up --build
```

Servicos disponiveis:
- Front-end: `http://localhost:4200`
- API: `http://localhost:8080`
- PostgreSQL: `localhost:5432`

### Execucao local sem Docker (backend)

```bash
dotnet restore
dotnet build
cd src/CoachTraining.Api
dotnet run
```

Por padrao, a API local usa o perfil de desenvolvimento do ASP.NET Core.

## 📊 Health check

Verifique se a API está rodando:

```http
GET http://localhost:8080/health-check
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

## 🐳 Docker compose

O `docker-compose.yml` em `infra/` orquestra 3 servicos:

- `db`: PostgreSQL 16 (volume persistente `postgres_data`)
- `api`: ASP.NET Core (.NET 10) em porta `8080`
- `frontend`: Angular buildado e servido por Nginx em porta `4200`

Comandos uteis:

```bash
cd infra
docker compose up --build
docker compose down
docker compose logs -f api
```

## 📅 Roadmap (10 semanas)

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

- Arquitetura:
  - [Overview](docs/architecture/overview.md)
  - [ADR-001 - Professor Auth](docs/architecture/decisions/adr-001-professor-auth.md)
  - [ADR-002 - Multi-tenant](docs/architecture/decisions/adr-002-multi-tenant.md)
- Domínios:
  - [Professor](docs/domains/professor.md)
  - [Aluno (Atleta no código)](docs/domains/aluno.md)
  - [Treino](docs/domains/treino.md)
- Casos de uso:
  - [Cadastrar Professor](docs/use-cases/cadastrar-professor.md)
  - [Cadastrar Aluno](docs/use-cases/cadastrar-aluno.md)
  - [Montar Treino](docs/use-cases/montar-treino.md)
- APIs:
  - [Professor API](docs/apis/professor-api.md)
  - [Aluno API](docs/apis/aluno-api.md)
  - [Dashboard API](docs/apis/dashboard-api.md)
- Fluxos:
  - [Cadastro de Professor](docs/flows/cadastro-professor.md)
  - [Fluxo de Treino](docs/flows/fluxo-treino.md)
  - [Fluxo de Dashboard do Aluno](docs/flows/fluxo-dashboard-aluno.md)
- Modelo de dados:
  - [Entidades](docs/data-model/entidades.md)
  - [Diagramas](docs/data-model/diagramas.md)
- Operação:
  - [Regras de Negócio](docs/rules/regras-negocio.md)
  - [Setup de Ambiente](docs/setup/ambiente.md)

## 🛠️ Stack tecnologico

| Camada | Tecnologia |
|--------|-----------|
| **Backend** | .NET 10 (C#) |
| **API** | ASP.NET Core |
| **Front-end** | Angular |
| **Web server (front)** | Nginx |
| **Banco** | PostgreSQL 16 |
| **ORM** | Entity Framework Core |
| **Orquestracao local** | Docker Compose |
| **Testes** | XUnit |
| **Logging** | Serilog (planejado) |

## 🧪 Testes

Execute os testes da solução:

```bash
dotnet test
```

## 📝 Padroes de codigo

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

**Status**: stack base definida (API + Angular + PostgreSQL + Docker Compose) e evolucao funcional em andamento ✅

## Frontend - Area do Professor (Sprint atual)

### Fluxo implementado

- Landing publica com cadastro de professor (`/`)
- Login com JWT (`/login`)
- Dashboard base protegido por autenticacao (`/dashboard`)
- Dashboard individual do aluno (`/dashboard/alunos/:id`) com metricas e graficos
- Exportacao de dados do dashboard do aluno em Excel e PDF

### Execucao local do frontend

```bash
cd frontend
npm install
npm start
```

O `npm start` usa proxy para `http://localhost:5096` nos paths `/professores`, `/auth` e `/api`.

### Documentacao tecnica do frontend

- [Estrutura do frontend](docs/frontend/estrutura.md)
- [Fluxo de autenticacao](docs/frontend/autenticacao.md)
- [Integracao com API](docs/frontend/integracao-api.md)
- [Design System (padrao visual oficial)](docs/frontend/design-system.md)
