# Estrutura do Frontend

## Visao geral

O frontend da area do professor foi implementado em Angular (standalone components), com separacao por camadas de responsabilidade para manter evolucao incremental nas proximas sprints.

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
- `features/dashboard`: tela inicial autenticada com placeholder operacional.
- `services/api`: contratos e chamadas HTTP orientadas ao backend.
- `services/auth`: sessao local (token/expiracao), login e logout.

## Rotas principais

- `/`: landing + cadastro de professor.
- `/login`: autenticacao.
- `/dashboard`: area protegida com layout autenticado.
