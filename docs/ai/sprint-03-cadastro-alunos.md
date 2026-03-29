# Sprint 03 - Cadastro e Visualizacao de Alunos

## Objetivo
Permitir que o professor autenticado cadastre e visualize somente seus proprios alunos, criando base para o dashboard individual na Sprint 04.

## Escopo
- Estruturar modulo de alunos no frontend.
- Implementar tela de cadastro de aluno.
- Implementar tela de listagem de alunos.
- Integrar com API autenticada via JWT.
- Atualizar navegacao no app shell.
- Garantir isolamento por professor no endpoint de listagem.

## Tarefas implementadas
1. Criacao da feature `students` com pastas `pages`, `services` e `models`.
2. Definicao do model centralizado `Student`.
3. Criacao da pagina `/dashboard/alunos/novo` com validacao de formulario.
4. Integracao de cadastro de aluno com backend via token JWT.
5. Feedback com snackbar e redirecionamento para listagem.
6. Criacao da pagina `/dashboard/alunos` com estado vazio e tabela.
7. Integracao de listagem via endpoint autenticado.
8. Inclusao dos links `Alunos` e `Novo aluno` no menu lateral.
9. Atualizacao do backend para suportar `GET /api/atleta` filtrado por professor.
10. Inclusao de `email` no modelo de atleta (dominio, persistencia e API).

## Endpoints utilizados
- `POST /api/atleta` - cadastro de aluno.
- `GET /api/atleta` - listagem de alunos do professor autenticado.
- `GET /api/atleta/{id}` - consulta individual.

## Campos de cadastro de aluno
- `nome` (obrigatorio)
- `email` (opcional)
- `observacoesClinicas` (opcional)
- `nivelEsportivo` (opcional)

## Decisoes tecnicas
- Reutilizacao do endpoint de atleta para representar aluno na sprint atual.
- Isolamento por tenant mantido com `professor_id` vindo do JWT.
- `email` de aluno agora possui coluna dedicada no banco (`atletas.email`).
- `dataCriacao` permanece retornada por mapeamento de servico nesta fase.

## Criterios de aceite atendidos
- Professor autenticado consegue cadastrar aluno.
- Professor autenticado consegue listar apenas seus alunos.
- Campos adicionais de cadastro (`email`, `observacoesClinicas`, `nivelEsportivo`) sao enviados e persistidos.
- Navegacao lateral inclui atalhos para listagem e novo cadastro.

## Pendencias para proxima sprint
- Persistir `dataCriacao` no modelo de atleta para listagem cronologica real.
- Implementar rota de dashboard individual `/dashboard/alunos/:id`.
- Substituir placeholder "Ver dashboard" por acao navegavel.
