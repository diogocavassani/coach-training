# coach-training

![Status](https://img.shields.io/badge/status-MVP_pronto-brightgreen)
![.NET Version](https://img.shields.io/badge/.NET-10.0-512BD4)
![License](https://img.shields.io/badge/license-MIT-green)

## Sobre

CoachTraining e um sistema de apoio a decisao para treinadores de corrida. O produto consolida sessoes de treino, calcula indicadores de carga e apresenta dashboards com alertas tecnicos para orientar o acompanhamento do atleta rumo a uma prova-alvo.

O sistema nao prescreve treino e nao substitui o treinador. No MVP, ele cobre cadastro de professor, autenticacao, cadastro de atletas, prova-alvo, planejamento base, registro de treinos, dashboard individual do atleta e home autenticada com resumo operacional real.

## Arquitetura

O backend segue Clean Architecture e DDD:

```text
src/CoachTraining.Api/          API ASP.NET Core
src/CoachTraining.App/          Application Layer
src/CoachTraining.Domain/       Domain Layer
src/CoachTraining.Infra/        Persistencia EF Core, repositorios e integracoes
frontend/                       SPA Angular
infra/                          Docker Compose e configuracoes locais
```

## Stack

- Backend: .NET 10 + ASP.NET Core
- Frontend: Angular
- Banco: PostgreSQL 16
- ORM: Entity Framework Core
- Orquestracao local: Docker Compose
- Testes: xUnit no backend e Karma/Jasmine no frontend

## MVP entregue

- Cadastro de professor e login com JWT
- Cadastro e listagem de atletas por professor
- Cadastro e edicao de prova-alvo por atleta
- Cadastro e edicao de planejamento base por atleta
- Registro de sessoes de treino
- Dashboard individual do atleta com metricas, graficos, exportacao e insights
- Home autenticada do professor com resumo operacional real
- Persistencia real com migrations EF Core

## Executando com Docker

Pre-requisitos:

- Docker Desktop

Passos:

```bash
cd infra
docker compose up --build
```

Servicos disponiveis:

- Frontend: `http://localhost:4200`
- API: `http://localhost:8080`
- PostgreSQL: `localhost:5432`

## Executando localmente sem Docker

Pre-requisitos:

- .NET 10 SDK
- Node.js 22+
- PostgreSQL local ou string de conexao valida

Backend:

```bash
dotnet restore
dotnet build
cd src/CoachTraining.Api
dotnet run
```

Frontend:

```bash
cd frontend
npm install
npm start
```

O frontend local usa proxy para `http://localhost:5096` nos caminhos `/professores`, `/auth` e `/api`.

## Health checks

Endpoints uteis:

- `GET http://localhost:8080/api/healthcheck`
- `GET http://localhost:8080/api/dashboard/health`
- `GET http://localhost:8080/api/atleta/health`

Resposta esperada do endpoint principal:

```json
{
  "status": "healthy",
  "timestamp": "2026-04-02T10:30:45.1234567Z",
  "version": "1.0.0",
  "environment": "Development"
}
```

## Dataset de Demonstração

Para preparar a base usada em apresentações com dados pré-populados:

```bash
dotnet run --project src/CoachTraining.DemoSeed -- --profile demo-v1 --reset-demo
```

O comando recria apenas o dataset `demo.*` (professor e 6 atletas) e imprime as credenciais do professor demo e o resumo esperado de cada atleta.

**Professor Demo**: `demo.professor@coachtraining.local` / `Demo@123456`

**Cenários inclusos**: Base estável, Construção saudável, Risco por carga abrupta, Taper bem executado, Aderência baixa e Divergência carga x rendimento.

Para mais detalhes: [docs/demo/demo-v1.md](docs/demo/demo-v1.md)

## Testes

Backend:

```bash
dotnet test CoachTraining.sln
```

Frontend:

```bash
cd frontend
npm test -- --watch=false --browsers=ChromeHeadless
npm run build
```

## Documentacao principal

- [Escopo do produto](docs/projeto_sistema_de_monitoramento_de_carga_e_preparacao_de_atletas.md)
- [Backlog do MVP](docs/backlog_mvp.md)
- [Overview do projeto](docs/overview_do_projeto.md)
- [Arquitetura atual](docs/architecture/overview.md)
- [API de aluno](docs/apis/aluno-api.md)
- [API de dashboard](docs/apis/dashboard-api.md)
- [Integracao frontend x API](docs/frontend/integracao-api.md)
- [Setup de ambiente](docs/setup/ambiente.md)
- [Regras de negocio](docs/rules/regras-negocio.md)

## Status atual

O MVP esta funcional, documentado e pronto para demonstracao tecnica. Os proximos ciclos podem focar em performance, observabilidade e integracoes externas sem reabrir o escopo base.
