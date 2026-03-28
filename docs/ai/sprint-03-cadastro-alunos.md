# Sprint 03 — Cadastro e Visualização de Alunos

## Objetivo
Permitir que o professor autenticado cadastre e visualize somente seus próprios alunos, criando base para o dashboard individual na Sprint 04.

## Escopo
- Estruturar módulo de alunos no frontend.
- Implementar tela de cadastro de aluno.
- Implementar tela de listagem de alunos.
- Integrar com API autenticada via JWT.
- Atualizar navegação no app shell.
- Garantir isolamento por professor no endpoint de listagem.

## Tarefas implementadas
1. Criação da feature `students` com pastas `pages`, `components`, `services` e `models`.
2. Definição do model centralizado `Student`.
3. Criação da página `/dashboard/alunos/novo` com validação de formulário.
4. Integração de cadastro de aluno com backend via token JWT.
5. Feedback com snackbar e redirecionamento para listagem.
6. Criação da página `/dashboard/alunos` com estado vazio.
7. Integração de listagem via endpoint autenticado.
8. Inclusão dos links `Alunos` e `Novo aluno` no menu lateral.
9. Atualização do backend para suportar `GET /api/Atleta` filtrado por professor.

## Endpoints utilizados
- `POST /api/Atleta` — cadastro de aluno.
- `GET /api/Atleta` — listagem de alunos do professor autenticado.
- `GET /api/Atleta/{id}` — consulta individual (já existente).

## Componentes/Páginas criadas
- `StudentCreatePageComponent`
- `StudentsListPageComponent`

## Serviços e modelos criados
- `StudentsApiService`
- `Student` (model)

## Rotas adicionadas
- `/dashboard/alunos`
- `/dashboard/alunos/novo`

## Decisões técnicas
- Reutilização do endpoint de atleta para representar aluno na sprint atual.
- Campo `email` tratado como opcional no frontend e armazenado temporariamente em `observacoesClinicas` até existir coluna dedicada no backend.
- Data de cadastro exibida apenas quando disponível no payload.

## Critérios de aceite atendidos
- Professor autenticado consegue cadastrar aluno.
- Professor autenticado consegue listar apenas seus alunos.
- Estado vazio com CTA para cadastro implementado.
- Navegação lateral inclui atalhos para listagem e novo cadastro.
- Documentação versionada da sprint criada e atualizada.

## Pendências para próxima sprint
- Criar endpoint/coluna dedicada para e-mail de aluno.
- Persistir `dataCriacao` no modelo de atleta para listagem cronológica real.
- Implementar rota de dashboard individual `/dashboard/alunos/:id`.
- Substituir placeholder "Ver dashboard" por ação navegável.
