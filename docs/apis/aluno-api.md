# API - Aluno

> Aluno na documentacao equivale a `Atleta` no codigo.

## Autenticacao
Todos os endpoints abaixo exigem `Authorization: Bearer <token>`.

## POST /api/atleta

### Request
```json
{
  "nome": "Atleta A",
  "email": "atleta.a@teste.com",
  "observacoesClinicas": "Sem restricoes",
  "nivelEsportivo": "Intermediario"
}
```

### Response 201 (exemplo)
```json
{
  "id": "6f4bb540-2d56-4ce7-a56d-8acaf556ab49",
  "professorId": "659cf34f-b7ee-4d32-aa77-c595f6da9bc7",
  "nome": "Atleta A",
  "email": "atleta.a@teste.com",
  "observacoesClinicas": "Sem restricoes",
  "nivelEsportivo": "Intermediario",
  "dataCriacao": "2026-03-29T15:00:00Z"
}
```

### Responses
- `201 Created`: aluno criado com `ProfessorId` do token.
- `400 Bad Request`: dados invalidos (ex.: nome vazio, email invalido).
- `401 Unauthorized`: sem token/token invalido.

## GET /api/atleta
- `200 OK`: lista somente alunos do professor autenticado.
- `401 Unauthorized`: sem token/token invalido.

### Response 200 (exemplo)
```json
[
  {
    "id": "6f4bb540-2d56-4ce7-a56d-8acaf556ab49",
    "professorId": "659cf34f-b7ee-4d32-aa77-c595f6da9bc7",
    "nome": "Atleta A",
    "email": "atleta.a@teste.com",
    "observacoesClinicas": "Sem restricoes",
    "nivelEsportivo": "Intermediario",
    "dataCriacao": "2026-03-29T15:00:00Z"
  }
]
```

## GET /api/atleta/{id}
- `200 OK`: quando aluno pertence ao professor autenticado.
- `404 Not Found`: quando nao existe ou pertence a outro professor.

## GET /api/dashboard/atleta/{id}
- `200 OK`: dashboard consolidado do aluno do professor autenticado.
- `404 Not Found`: aluno nao pertence ao professor autenticado.

## Health (anonimo)
- `GET /api/atleta/health`
- `GET /api/dashboard/health`
