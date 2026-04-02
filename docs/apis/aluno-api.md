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

## GET /api/atleta/{id}/prova-alvo
- `200 OK`: retorna a prova-alvo atual do atleta.
- `404 Not Found`: quando o atleta nao possui prova-alvo cadastrada ou nao pertence ao professor autenticado.

### Response 200 (exemplo)
```json
{
  "id": "5db28c72-d984-47fd-b4d5-c18f4fa1f4f9",
  "atletaId": "6f4bb540-2d56-4ce7-a56d-8acaf556ab49",
  "dataProva": "2026-05-24",
  "distanciaKm": 21.1,
  "objetivo": "Completar forte"
}
```

## PUT /api/atleta/{id}/prova-alvo

### Request
```json
{
  "dataProva": "2026-05-24",
  "distanciaKm": 21.1,
  "objetivo": "Completar forte"
}
```

### Responses
- `200 OK`: cria ou atualiza a prova-alvo do atleta.
- `400 Bad Request`: payload invalido.
- `403 Forbidden`: tentativa de alterar prova-alvo de atleta sem ownership.
- `401 Unauthorized`: sem token/token invalido.

## GET /api/atleta/{id}/planejamento-base
- `200 OK`: retorna o planejamento base atual do atleta.
- `404 Not Found`: quando o atleta nao possui planejamento base cadastrado ou nao pertence ao professor autenticado.

### Response 200 (exemplo)
```json
{
  "atletaId": "6f4bb540-2d56-4ce7-a56d-8acaf556ab49",
  "treinosPlanejadosPorSemana": 5
}
```

## PUT /api/atleta/{id}/planejamento-base

### Request
```json
{
  "treinosPlanejadosPorSemana": 5
}
```

### Responses
- `200 OK`: cria ou atualiza o planejamento base do atleta.
- `400 Bad Request`: payload invalido.
- `403 Forbidden`: tentativa de alterar planejamento de atleta sem ownership.
- `401 Unauthorized`: sem token/token invalido.

## GET /api/dashboard/atleta/{id}
- `200 OK`: dashboard consolidado do aluno do professor autenticado.
- `404 Not Found`: aluno nao pertence ao professor autenticado.

### Response 200 (campos novos do planejamento)
```json
{
  "treinosPlanejadosPorSemana": 4,
  "treinosRealizadosNaSemana": 3,
  "aderenciaPlanejamentoPercentual": 75.0
}
```

## Health (anonimo)
- `GET /api/atleta/health`
- `GET /api/dashboard/health`
