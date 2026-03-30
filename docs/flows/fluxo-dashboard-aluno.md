# Fluxo - Professor -> Aluno -> Dashboard

1. Professor autentica e recebe JWT com `professor_id`.
2. Professor acessa listagem de alunos (`GET /api/atleta`).
3. Professor seleciona um aluno e navega para `/dashboard/alunos/:id`.
4. Frontend consulta `GET /api/dashboard/atleta/{id}`.
5. API valida ownership via token (`professor_id`) e atleta solicitado.
6. API retorna metricas, insights, series de carga/pace e `treinosJanela` (12 semanas).
7. Professor analisa cards, graficos e insights.
8. Professor pode exportar dados da janela em Excel e PDF diretamente na tela.
