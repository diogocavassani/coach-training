# API - Dashboard do Aluno

## Resumo do professor
`GET /api/dashboard/professor/resumo`

### Response (200)
```json
{
  "totalAtletas": 12,
  "atletasEmAtencao": 3,
  "atletasEmRisco": 1,
  "atletasEmTaper": 2,
  "treinosRegistradosNaSemana": 19,
  "atletasComPlanejamentoConfigurado": 8,
  "dataUltimaAtualizacao": "2026-04-02T02:10:00Z",
  "atletasPrioritarios": [
    {
      "atletaId": "4df75e0e-3df5-4ef6-8cb7-8f7b5f8d87af",
      "nome": "Atleta Exemplo",
      "statusRisco": 2,
      "emJanelaDeTaper": false,
      "proximaProva": null,
      "cargaSemanal": 1180,
      "aderenciaPlanejamentoPercentual": 50.0
    }
  ],
  "treinosRecentes": [
    {
      "atletaId": "4df75e0e-3df5-4ef6-8cb7-8f7b5f8d87af",
      "nomeAtleta": "Atleta Exemplo",
      "data": "2026-04-01",
      "tipo": 2,
      "carga": 810
    }
  ]
}
```

### Regras
- Resume apenas dados do `professor_id` autenticado.
- `atletasPrioritarios` combina risco, taper e aderencia fora do esperado.
- `treinosRecentes` retorna os treinos mais recentes do workspace para apoiar a navegacao inicial.

## Endpoint
`GET /api/dashboard/atleta/{id}`

## Autenticacao
Endpoint protegido por JWT (`[Authorize]`).
O `professor_id` e extraido do token autenticado para validar ownership do atleta.

## Path params
- `id` (Guid): identificador do atleta.

## Response (200)
```json
{
  "atletaId": "4df75e0e-3df5-4ef6-8cb7-8f7b5f8d87af",
  "nome": "Atleta Exemplo",
  "cargaUltimaSessao": 320,
  "cargaSemanal": 1180,
  "cargaSemanalAnterior": 1020,
  "cargaAguda": 1180,
  "cargaCronica": 1085,
  "acwr": 1.09,
  "deltaPercentualSemanal": 15.7,
  "treinosPlanejadosPorSemana": 4,
  "treinosRealizadosNaSemana": 3,
  "aderenciaPlanejamentoPercentual": 75.0,
  "faseAtual": 1,
  "statusRisco": 0,
  "emJanelaDeTaper": false,
  "proximaProva": null,
  "reducaoVolumeTaper": null,
  "dataUltimaAtualizacao": "2026-03-30T01:10:25.231Z",
  "observacoesClin": "Sem restricoes",
  "nivelAtleta": "Intermediario",
  "insights": [
    "Progressao de carga dentro da faixa esperada."
  ],
  "serieCargaSemanal": [
    {
      "semanaInicio": "2026-01-06",
      "semanaFim": "2026-01-12",
      "valor": 840
    }
  ],
  "seriePaceSemanal": [
    {
      "semanaInicio": "2026-01-06",
      "semanaFim": "2026-01-12",
      "valorMinPorKm": 5.12
    }
  ],
  "treinosJanela": [
    {
      "id": "773ed7af-570c-49fe-a3bc-53da455d2c20",
      "data": "2026-03-28",
      "tipo": 2,
      "duracaoMinutos": 50,
      "distanciaKm": 10,
      "rpe": 7,
      "carga": 350,
      "paceMinPorKm": 5.0
    }
  ]
}
```

## Regras de negocio
- O professor autenticado so pode consultar dashboard de atleta vinculado ao proprio `professor_id`.
- `treinosRealizadosNaSemana` usa a mesma janela movel de 7 dias adotada para `cargaSemanal`.
- `aderenciaPlanejamentoPercentual` so e calculada quando o atleta possui `treinosPlanejadosPorSemana`.
- O gerador de insights passa a sinalizar aderencia abaixo do esperado (`< 80%`) e execucao acima do planejado (`> 120%`).
- A serie semanal usa janela fixa de 12 semanas (segunda a domingo).
- `seriePaceSemanal.valorMinPorKm` pode ser `null` quando nao houver distancia valida na semana.
- `treinosJanela` contem os treinos da mesma janela de 12 semanas usada nos graficos.

## Erros comuns
- `400`: `id` invalido.
- `401`: token ausente/invalido.
- `404`: atleta nao encontrado para o professor autenticado.
- `500`: erro interno no processamento do dashboard.
