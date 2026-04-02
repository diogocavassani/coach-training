# Estrutura do Frontend

## Visao geral

O frontend da area do professor foi implementado em Angular (standalone components), com separacao por camadas de responsabilidade para manter evolucao incremental nas proximas sprints.

As paginas principais usam lazy loading via `loadComponent` nas rotas para reduzir o bundle inicial de producao. Componentes mais pesados, como o dashboard individual do aluno, ficam fora do chunk inicial e sao carregados sob demanda.

## Estrutura de pastas

```text
frontend/src/app
  core/
    guards/
    interceptors/
    layout/
  features/
    auth/pages/
    professor/pages/
    dashboard/pages/
    students/
      models/
      pages/
      services/
  shared/
    components/
  services/
    api/
    auth/
```

## Responsabilidades

- `core/guards`: regras de acesso (`authGuard`) para rotas protegidas.
- `core/interceptors`: comportamento global de HTTP (`authTokenInterceptor`).
- `core/layout`: shell autenticado com header, sidebar e area principal.
- `features/auth`: tela de login e validacoes.
- `features/professor`: landing publica e formulario de cadastro do professor.
- `features/dashboard`: tela inicial autenticada e dashboard individual do aluno (metricas, graficos e exportacao).
- `features/students`: cadastro e listagem de alunos do professor autenticado.
- `services/api`: contratos e chamadas HTTP orientadas ao backend.
- `services/auth`: sessao local (token/expiracao), login e logout.

## Rotas principais

- `/`: landing + cadastro de professor.
- `/login`: autenticacao.
- `/dashboard`: area protegida com layout autenticado.
- `/dashboard/alunos`: listagem de alunos do professor.
- `/dashboard/alunos/novo`: cadastro de aluno.
- `/dashboard/alunos/:id`: dashboard individual do aluno.
- `/dashboard/treinos/novo`: cadastro de treino.

## Estrategia de entrega

- Rotas publicas e autenticadas sao carregadas sob demanda para manter o bundle inicial enxuto.
- O chunk inicial deve continuar pequeno o suficiente para nao bloquear `ng build` em producao.
- Bibliotecas pesadas usadas em fluxos especificos devem permanecer fora da entrada principal sempre que possivel.
