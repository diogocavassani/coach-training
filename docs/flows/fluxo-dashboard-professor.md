# Fluxo - Home autenticada do professor

1. Professor autentica e recebe JWT com `professor_id`.
2. Professor acessa `/dashboard`.
3. Frontend consulta `GET /api/dashboard/professor/resumo`.
4. API lista apenas os atletas do professor autenticado e consolida KPIs, prioridades e treinos recentes.
5. Frontend exibe cards reais, lista de atletas prioritarios e atalhos para cadastro de aluno, treino e dashboards individuais.
