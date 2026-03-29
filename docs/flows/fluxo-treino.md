# Fluxo - Treino

1. Professor autentica e recebe JWT com `professor_id`.
2. Professor cadastra aluno (`POST /api/atleta`) com `nome`, `email`, `observacoesClinicas` e `nivelEsportivo`.
3. API extrai `professor_id` e grava vinculo no atleta.
4. Professor lista seus alunos (`GET /api/atleta`).
5. Professor consulta dashboard (`GET /api/dashboard/atleta/{id}`).
6. Repositorios filtram por `ProfessorId` e retornam apenas dados do tenant atual.
