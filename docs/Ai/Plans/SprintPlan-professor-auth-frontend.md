## Sprint Frontend — Área do Professor (commits atômicos + PR)

### Resumo
Implementar o frontend Angular inicial da área do professor com cadastro, login, persistência JWT, proteção de rotas, layout base (`/dashboard`) e documentação técnica em `docs/frontend/`.  
Decisões já fechadas: PR com base em `main`, integração local por `proxy` Angular, direção visual “moderno e imponente”.

### Mudanças de implementação (interfaces e comportamento)
- Rotas públicas e protegidas:
  - `/` landing + cadastro de professor.
  - `/login` tela de autenticação.
  - `/dashboard` protegida por guard.
- Contratos TS da integração:
  - `CreateProfessorRequest` (`nome`, `email`, `senha`), `ProfessorResponse`.
  - `LoginRequest` (`email`, `senha`), `LoginResponse` (`token`, `expiraEmUtc`).
- Serviços de autenticação:
  - `AuthService.login`, `logout`, `getToken`, `isAuthenticated`, `getExpiration`.
  - Persistência em `localStorage`.
- Segurança no client:
  - `AuthGuard` bloqueando rotas privadas.
  - `HttpInterceptor` adicionando `Authorization: Bearer <token>`.
- Integração API no dev:
  - `proxy.conf.json` para `/api`, `/auth`, `/professores` apontando para `http://localhost:5096`.
- Documentação:
  - `docs/frontend/estrutura.md`
  - `docs/frontend/autenticacao.md`
  - `docs/frontend/integracao-api.md`
  - atualização de `README.md` e `frontend/README.md` com navegação e fluxo local.

### Sequência de pequenos commits
1. `chore(frontend): organize app structure for core features`
Cria estrutura `core`, `features/auth`, `features/professor`, `features/dashboard`, `shared`, `services` com componentes/arquivos base.

2. `chore(frontend): add angular material and modern theme foundation`
Instala Angular Material, configura tema base e tokens visuais (tipografia/cores) alinhados ao estilo “moderno e imponente”.

3. `feat(frontend): configure app routing for public and protected flows`
Define rotas `/`, `/login`, `/dashboard` e fallback de navegação.

4. `feat(frontend): build landing page with strong visual direction`
Implementa landing pública com CTA para cadastro/login e layout responsivo.

5. `feat(frontend): add professor signup form with validation`
Form reativo (nome, email, senha), validações básicas e estados de erro.

6. `feat(frontend): integrate signup with POST /professores`
Cria serviço de professor e integração HTTP de cadastro com tratamento de sucesso/erro.

7. `feat(frontend): add login screen with validation`
Tela de login com formulário reativo e validações obrigatórias.

8. `feat(frontend): implement auth service and local token persistence`
Integra `POST /auth/login`, salva JWT + expiração em `localStorage`, implementa logout.

9. `feat(frontend): add auth guard for protected routes`
Bloqueia `/dashboard` sem token válido e redireciona para `/login`.

10. `feat(frontend): add jwt http interceptor`
Adiciona token automaticamente nas requests autenticadas.

11. `feat(frontend): implement dashboard shell with header and sidebar`
Cria layout base da aplicação autenticada com header, sidebar e placeholder funcional.

12. `chore(frontend): configure angular dev proxy for backend integration`
Adiciona `proxy.conf.json` e ajusta scripts de execução local.

13. `test(frontend): add unit tests for auth flow and route protection`
Testes para `AuthService`, `AuthGuard`, interceptor e validações principais dos formulários.

14. `docs(frontend): add frontend technical documentation`
Cria `docs/frontend/estrutura.md`, `docs/frontend/autenticacao.md`, `docs/frontend/integracao-api.md`.

15. `docs(readme): update project and frontend readmes`
Atualiza `README.md` e `frontend/README.md` com execução, rotas e links da nova documentação.

16. `chore(github): open sprint frontend pull request`
Push da branch atual e criação de PR via plugin GitHub com base `main`, descrição por épicos e checklist de aceite.

### Plano de testes e aceite
- `ng serve` sobe sem erro e `/` carrega landing.
- Cadastro com dados válidos chama `POST /professores` e confirma sucesso.
- Cadastro inválido exibe erro de validação no formulário.
- Login válido chama `POST /auth/login` e persiste token.
- Login inválido exibe feedback de falha.
- Acesso a `/dashboard` sem token redireciona para `/login`.
- Requests autenticadas incluem header `Authorization`.
- Dashboard renderiza layout base responsivo.
- `ng test` passando para unidade (auth/guard/interceptor/forms).

### Assumptions e defaults
- Implementação em Angular standalone (sem NgModules adicionais).
- Escopo desta sprint não inclui cadastro de aluno nem dashboard analítico completo.
- Sem alterações de backend para CORS nesta sprint; integração local via proxy.
- Commits sempre com `git add` por caminho para manter atomicidade.
- PR final: `feat/couch-front` -> `main`.
