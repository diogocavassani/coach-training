# Overview do Projeto - CoachTraining

## Proposta

CoachTraining e um sistema de apoio a decisao para treinadores de corrida, com foco em transformar dados de treino em informacao acionavel. O produto centraliza carga, risco, fase do ciclo, taper e aderencia ao planejamento sem substituir a leitura tecnica do treinador.

## Problema que resolve

Planilhas e acompanhamentos manuais tornam dificil:

- perceber aumentos abruptos de carga;
- acompanhar multiplos atletas com criterio consistente;
- identificar quem demanda atencao imediata;
- cruzar planejamento, execucao e proximidade de prova.

## MVP atual

- Cadastro de professor e autenticacao JWT
- Cadastro e listagem de atletas por professor
- Cadastro e edicao de prova-alvo por atleta
- Cadastro e edicao de planejamento base por atleta
- Registro de sessoes de treino
- Dashboard individual do atleta com metricas, insights, series e exportacao
- Home autenticada do professor com resumo operacional real
- Persistencia em PostgreSQL com EF Core e migrations

## Diferenciais tecnicos

- Clean Architecture com separacao clara entre API, aplicacao, dominio e infraestrutura
- DDD para entidades, value objects e regras de negocio
- Suite automatizada cobrindo dominio, aplicacao, API e frontend
- Docker Compose para subir banco, API e frontend no fluxo principal

## Estado atual

- Persistencia real em funcionamento
- Fluxos principais do MVP entregues ponta a ponta
- Build de producao do frontend validado
- Documentacao principal alinhada ao estado do codigo

## Proximos passos sugeridos

1. Evoluir observabilidade com logs estruturados e metricas.
2. Revisar performance de consultas conforme a base crescer.
3. Planejar integracoes futuras com plataformas externas e wearables.
