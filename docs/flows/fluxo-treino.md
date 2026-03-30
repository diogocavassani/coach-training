# Fluxo - Professor -> Atleta -> Treino

1. Professor autentica e recebe JWT com `professor_id`.
2. Professor cadastra atleta (`POST /api/atleta`).
3. Professor registra treino (`POST /treinos`) informando dados da sessao.
4. API extrai `ProfessorId` do token (nunca do body).
5. Caso de uso valida se o atleta pertence ao professor autenticado.
6. Sessao e persistida em `sessoes_treino` com FK para atleta.
7. Dashboard e demais calculos passam a considerar a nova sessao.
