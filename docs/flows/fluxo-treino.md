# Fluxo - Treino

1. Professor autentica e recebe JWT com `professor_id`.
2. Professor cadastra aluno (`POST /api/atleta`).
3. API extrai `professor_id` e grava vinculo.
4. Professor consulta dashboard (`GET /api/dashboard/atleta/{id}`).
5. Repositorios filtram por `ProfessorId` e retornam apenas dados do tenant atual.
