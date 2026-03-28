# API - Aluno

> Aluno na documentacao equivale a `Atleta` no codigo.

## Autenticacao
Todos os endpoints abaixo exigem `Authorization: Bearer <token>`.

## POST /api/atleta

### Request
```json
{
  "nome": "Atleta A",
  "observacoesClinicas": "Sem restricoes",
  "nivelEsportivo": "Intermediario"
}
```

### Response
- `201 Created`: aluno criado com `ProfessorId` do token.
- `401 Unauthorized`: sem token/token invalido.

## GET /api/atleta/{id}
- `200 OK`: quando aluno pertence ao professor autenticado.
- `404 Not Found`: quando nao existe ou pertence a outro professor.

## GET /api/dashboard/atleta/{id}
- `200 OK`: dashboard consolidado do aluno do professor autenticado.
- `404 Not Found`: aluno nao pertence ao professor autenticado.

## Health (anonimos)
- `GET /api/atleta/health`
- `GET /api/dashboard/health`
