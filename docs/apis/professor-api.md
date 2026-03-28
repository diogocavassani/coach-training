# API - Professor

## POST /professores

### Request
```json
{
  "nome": "Professor Teste",
  "email": "professor@teste.com",
  "senha": "123456"
}
```

### Responses
- `201 Created`: professor cadastrado.
- `400 Bad Request`: payload invalido.
- `409 Conflict`: email ja cadastrado.

## POST /auth/login

### Request
```json
{
  "email": "professor@teste.com",
  "senha": "123456"
}
```

### Response 200
```json
{
  "token": "jwt...",
  "expiraEmUtc": "2026-03-27T12:00:00Z"
}
```

### Responses de erro
- `401 Unauthorized`: credenciais invalidas.
- `400 Bad Request`: payload invalido.
