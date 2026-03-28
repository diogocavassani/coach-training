# Caso de Uso - Montar Treino (Dashboard)

## Ator
Professor autenticado

## Objetivo
Visualizar consolidacao de carga, risco e fase de treinamento do aluno.

## Fluxo principal
1. API recebe `GET /api/dashboard/atleta/{id}` com token JWT.
2. Valida posse do aluno por `ProfessorId`.
3. Carrega sessoes e prova-alvo do aluno.
4. Calcula dashboard.
5. Retorna `200 OK`.

## Fluxos de erro
- Aluno nao pertence ao professor: `404 Not Found`.
- Sem autenticacao: `401 Unauthorized`.
