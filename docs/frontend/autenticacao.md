# Fluxo de Autenticacao (Frontend)

## Objetivo

Garantir que apenas professores autenticados acessem rotas privadas, com sessao persistida localmente.

## Sequencia

1. Professor envia credenciais na tela `/login`.
2. Frontend chama `POST /auth/login`.
3. Em caso de sucesso, `AuthService` persiste:
   - `coachtraining.auth.token`
   - `coachtraining.auth.expiration`
4. Navegacao segue para `/dashboard`.
5. Toda request subsequente passa por `authTokenInterceptor`, que adiciona `Authorization: Bearer <token>`.
6. `authGuard` valida sessao para rotas privadas.
7. Se token estiver ausente/expirado, o guard limpa sessao e redireciona para `/login`.

## Componentes envolvidos

- `AuthService`: login, logout, token, expiracao, validacao de sessao.
- `authGuard`: gatekeeper de rotas protegidas.
- `authTokenInterceptor`: injecao automatica de JWT nas requests.

## Decisoes da sprint

- Persistencia em `localStorage` (MVP).
- Sem refresh token nesta etapa.
- Expiracao confiada ao `expiraEmUtc` retornado pela API.
