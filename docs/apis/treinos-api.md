# API - Treinos

## Endpoint
`POST /treinos`

## Autenticacao
Endpoint protegido por JWT (`[Authorize]`).
O `ProfessorId` e obtido exclusivamente do token autenticado.

## Request
```json
{
  "atletaId": "f52af0fd-2a2f-4f43-82f9-c27953cbcb30",
  "data": "2026-03-28",
  "tipo": 2,
  "duracaoMinutos": 60,
  "distanciaKm": 12.5,
  "rpe": 7
}
```

## Response (201)
```json
{
  "id": "4f8067cf-85b9-4f28-afd3-40721db458af",
  "atletaId": "f52af0fd-2a2f-4f43-82f9-c27953cbcb30",
  "data": "2026-03-28",
  "tipo": 2,
  "duracaoMinutos": 60,
  "distanciaKm": 12.5,
  "rpe": 7
}
```

## Regras
- `AtletaId` obrigatorio.
- `DuracaoMinutos` deve ser maior que zero.
- `RPE` deve estar entre 1 e 10.
- O atleta precisa pertencer ao professor autenticado.
