# Setup de Ambiente

## Requisitos

- .NET 10 SDK
- Node.js 22+
- Docker Desktop

## Variaveis importantes

JWT:

- `Jwt:Issuer`
- `Jwt:Audience`
- `Jwt:Key`
- `Jwt:ExpirationHours`

Banco:

- `ConnectionStrings:DefaultConnection`

No fluxo Docker, essas configuracoes ja sao fornecidas em `infra/docker-compose.yml`.

## Subindo o ambiente principal

```bash
cd infra
docker compose up --build
```

## Rodando localmente

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

## Health checks

```http
GET /api/healthcheck
GET /api/atleta/health
GET /api/dashboard/health
```

## Comandos uteis

```bash
dotnet test CoachTraining.sln
dotnet ef migrations list --project src/CoachTraining.Infra --startup-project src/CoachTraining.Api
cd frontend && npm test -- --watch=false --browsers=ChromeHeadless
cd frontend && npm run build
```

## Dataset demo

Com o PostgreSQL local ou Docker em execução:

```bash
dotnet run --project src/CoachTraining.DemoSeed -- --profile demo-v1 --reset-demo
```

O seed foi feito para apresentação e feedback com treinadores. Por padrão, remove apenas dados `demo.*` das execuções anteriores.

Credenciais do professor demo:
- Email: `demo.professor@coachtraining.local`
- Senha: `Demo@123456`

Para mais informações sobre os cenários inclusos e fluxo de uso, veja [docs/demo/demo-v1.md](../demo/demo-v1.md).
