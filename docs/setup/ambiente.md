# Setup de Ambiente

## Requisitos
- .NET 10 SDK
- Docker (opcional, para PostgreSQL/API/frontend)

## Configuracao JWT
No `appsettings`:
- `Jwt:Issuer`
- `Jwt:Audience`
- `Jwt:Key`
- `Jwt:ExpirationHours`

## Banco
- Local: usar `ConnectionStrings:DefaultConnection`.
- Em dev, migracao atual pode assumir reset de base para aplicar `ProfessorId` obrigatorio.

## Comandos uteis
```bash
dotnet restore
dotnet build
dotnet test tests/CoachTraining.Domain.Tests/CoachTraining.Tests.csproj
dotnet ef migrations list --project src/CoachTraining.Infra --startup-project src/CoachTraining.Api
```
