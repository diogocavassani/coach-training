# Integracao com API

## Ambiente de desenvolvimento

No desenvolvimento local (`ng serve`), o frontend usa proxy Angular (`frontend/proxy.conf.json`) para evitar CORS sem alterar backend.

Alvos de proxy:

- `/professores` -> `http://localhost:5096/professores`
- `/auth` -> `http://localhost:5096/auth`
- `/api` -> `http://localhost:5096/api`

## Ambiente Docker Compose

Quando executado via `docker compose`, o frontend e servido por Nginx e as rotas de API sao encaminhadas para o servico `api` na rede interna:

- `/professores` -> `http://api:8080/professores`
- `/auth` -> `http://api:8080/auth`
- `/api` -> `http://api:8080/api`

## Endpoints usados na sprint

### Cadastro de professor

- Metodo: `POST /professores`
- Request:

```json
{
  "nome": "Professor Exemplo",
  "email": "professor@coach.com",
  "senha": "123456"
}
```

- Response esperado: `201 Created` com dados do professor.
- Erros tratados:
  - `400`: validacao
  - `409`: email duplicado

### Login

- Metodo: `POST /auth/login`
- Request:

```json
{
  "email": "professor@coach.com",
  "senha": "123456"
}
```

- Response esperado: `200 OK`

```json
{
  "token": "<jwt>",
  "expiraEmUtc": "2026-03-28T12:00:00Z"
}
```

- Erros tratados:
  - `400`: validacao de payload
  - `401`: credenciais invalidas

### Alunos - Cadastro

- Metodo: `POST /api/atleta`
- Request:

```json
{
  "nome": "Atleta Exemplo",
  "email": "atleta@coach.com",
  "observacoesClinicas": "Sem restricoes",
  "nivelEsportivo": "Intermediario"
}
```

- Response esperado: `201 Created` com `id`, `professorId`, `nome`, `email`, `observacoesClinicas`, `nivelEsportivo`, `dataCriacao`.
- Erros tratados:
  - `400`: validacao (nome vazio, email invalido)
  - `401`: token ausente/invalido

### Alunos - Listagem

- Metodo: `GET /api/atleta`
- Response esperado: `200 OK` com lista de alunos vinculados ao professor autenticado.
- Erros tratados:
  - `401`: token ausente/invalido

### Dashboard do aluno

- Metodo: `GET /api/dashboard/atleta/{id}`
- Response esperado: `200 OK` com metricas consolidadas, insights, series (`serieCargaSemanal`, `seriePaceSemanal`), `treinosJanela` e os campos de aderencia (`treinosPlanejadosPorSemana`, `treinosRealizadosNaSemana`, `aderenciaPlanejamentoPercentual`).
- Erros tratados:
  - `401`: token ausente/invalido
  - `404`: atleta nao encontrado para o professor autenticado

### Alunos - Prova alvo

- Metodo: `GET /api/atleta/{id}/prova-alvo`
- Response esperado: `200 OK` com `id`, `atletaId`, `dataProva`, `distanciaKm` e `objetivo`.
- Erros tratados:
  - `401`: token ausente/invalido
  - `404`: atleta sem prova alvo cadastrada ou fora do ownership

- Metodo: `PUT /api/atleta/{id}/prova-alvo`
- Request:

```json
{
  "dataProva": "2026-05-24",
  "distanciaKm": 21.1,
  "objetivo": "Completar forte"
}
```

- Response esperado: `200 OK` com a prova alvo persistida.
- Erros tratados:
  - `400`: validacao de payload
  - `401`: token ausente/invalido
  - `403`: tentativa de editar prova de atleta sem ownership

### Alunos - Planejamento base

- Metodo: `GET /api/atleta/{id}/planejamento-base`
- Response esperado: `200 OK` com `atletaId` e `treinosPlanejadosPorSemana`.
- Erros tratados:
  - `401`: token ausente/invalido
  - `404`: atleta sem planejamento base cadastrado ou fora do ownership

- Metodo: `PUT /api/atleta/{id}/planejamento-base`
- Request:

```json
{
  "treinosPlanejadosPorSemana": 5
}
```

- Response esperado: `200 OK` com o planejamento persistido.
- Erros tratados:
  - `400`: validacao de payload
  - `401`: token ausente/invalido
  - `403`: tentativa de editar planejamento de atleta sem ownership

### Treinos - Cadastro

- Metodo: `POST /api/treinos`
- Response esperado: `201 Created` com dados da sessao cadastrada.
- Erros tratados:
  - `400`: validacao de payload (dados obrigatorios/faixas)
  - `401`: token ausente/invalido
  - `403`: tentativa de cadastrar treino para atleta sem ownership

## Requests autenticadas

Chamadas para recursos privados usam header:

```http
Authorization: Bearer <jwt>
```

Esse comportamento e centralizado no interceptor `authTokenInterceptor`.
