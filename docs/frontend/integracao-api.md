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

## Requests autenticadas

Chamadas para recursos privados usam header:

```http
Authorization: Bearer <jwt>
```

Esse comportamento e centralizado no interceptor `authTokenInterceptor`.
