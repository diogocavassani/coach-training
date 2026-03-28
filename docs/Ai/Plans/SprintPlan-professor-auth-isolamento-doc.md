# Plano de Execução em Pequenos Commits (branch `feat/couch`)

## Resumo
Executar a sprint em commits atômicos, mantendo evolução incremental de domínio, infraestrutura, autenticação JWT, isolamento multi-tenant e documentação no novo padrão de `docs/`.  
Regra operacional fixa: usar `git add` por caminho e não incluir mudanças pré-existentes (`WeatherForecast` deletado e `docs/Ai/`).

## APIs/interfaces/tipos que serão introduzidos ou alterados
1. Novo endpoint `POST /professores` (cadastro).
2. Novo endpoint `POST /auth/login` (JWT).
3. Endpoints protegidos por JWT: `POST /api/atleta`, `GET /api/atleta/{id}`, `GET /api/dashboard/atleta/{id}`.
4. Claim obrigatória no token: `professor_id`.
5. Entidade `Coach` removida e substituída por `Professor`.
6. `Atleta` passa a exigir `ProfessorId` obrigatório.
7. Repositórios passam a consultar por contexto de professor (filtro de tenant).

## Sequência de commits pequenos
1. `refactor(domain): replace Coach entity with Professor`
Escopo: criar `Professor` no Domain e remover `Coach`.  
Validação: build do Domain.

2. `test(domain): add professor entity unit tests`
Escopo: testes de criação válida, email inválido, nome/senha hash obrigatórios.  
Validação: `dotnet test` com filtro de testes de domínio.

3. `feat(domain): require ProfessorId in Atleta entity`
Escopo: adicionar `ProfessorId` no construtor/propriedades de `Atleta` e validação `Guid.Empty`.  
Validação: ajustar e rodar testes que instanciam `Atleta`.

4. `feat(app): propagate ProfessorId in athlete DTOs and services`
Escopo: ajustar `AtletaDto`, `CadastroAtletaService` e assinaturas para contexto de professor autenticado.  
Validação: testes de App existentes.

5. `feat(app): add professor/auth contracts and DTOs`
Escopo: criar DTOs de cadastro/login e interfaces `IProfessorRepository`, `IPasswordHasher`, `ITokenService`.  
Validação: build da solução.

6. `feat(infra): add ProfessorModel and repository`
Escopo: criar `ProfessorModel` + `ProfessorRepository` + DI.  
Validação: build da Infra.

7. `feat(infra): enforce tenant filters in repositories`
Escopo: atualizar `AtletaRepository`, `SessaoDeTreinoRepository`, `ProvaAlvoRepository` para filtrar por `ProfessorId`.  
Validação: testes de App/Domain.

8. `feat(db): map professores and atleta-professor relation`
Escopo: `DbContext` com tabela `professores`, FK obrigatória em `atletas`, índice único email, índice `ProfessorId`.  
Validação: build + verificação de mapeamento.

9. `feat(db): add migration for professor auth and tenant isolation`
Escopo: criar migration EF correspondente (reset dev como estratégia).  
Validação: `dotnet ef database update` em ambiente local de dev.

10. `feat(api): configure JWT authentication and authorization pipeline`
Escopo: `AddAuthentication`, `AddJwtBearer`, `AddAuthorization`, `UseAuthentication` antes de `UseAuthorization`, seção `Jwt` no `appsettings`.  
Validação: build da API e smoke test de pipeline.

11. `feat(app): add professor registration service with BCrypt`
Escopo: serviço de cadastro com verificação de email duplicado e senha mínima 6 caracteres.  
Validação: testes unitários do serviço.

12. `feat(api): add POST /professores endpoint`
Escopo: controller e respostas HTTP de cadastro (`201`, `409`, `400`).  
Validação: testes de integração do endpoint.

13. `feat(app): add login service and token generation`
Escopo: validação de credenciais + emissão de JWT com claim `professor_id` e expiração 8h.  
Validação: testes unitários de login/token.

14. `feat(api): add POST /auth/login endpoint`
Escopo: endpoint de login retornando token e expiração; `401` para inválido.  
Validação: testes de integração de login.

15. `feat(api): protect athlete and dashboard endpoints with [Authorize]`
Escopo: bloquear endpoints sensíveis, liberar health via `[AllowAnonymous]`, extrair `ProfessorId` de claim e aplicar isolamento no fluxo.  
Validação: testes de integração de autorização e isolamento.

16. `test(api): add integration tests for professor auth and tenant isolation`
Escopo: cenários cadastro professor, login válido/inválido, Professor A x Professor B no acesso a atleta/dashboard.  
Validação: execução completa da suíte de integração.

17. `docs(architecture): add overview and ADRs`
Escopo:
`docs/architecture/overview.md`  
`docs/architecture/decisions/adr-001-professor-auth.md`  
`docs/architecture/decisions/adr-002-multi-tenant.md`

18. `docs(domains-usecases): add domain and use-case docs`
Escopo:
`docs/domains/professor.md`  
`docs/domains/aluno.md`  
`docs/domains/treino.md`  
`docs/use-cases/cadastrar-professor.md`  
`docs/use-cases/cadastrar-aluno.md`  
`docs/use-cases/montar-treino.md`

19. `docs(apis-flows-data-rules-setup): finalize docs structure and README links`
Escopo:
`docs/apis/professor-api.md`  
`docs/apis/aluno-api.md`  
`docs/flows/cadastro-professor.md`  
`docs/flows/fluxo-treino.md`  
`docs/data-model/entidades.md`  
`docs/data-model/diagramas.md`  
`docs/rules/regras-negocio.md`  
`docs/setup/ambiente.md`  
Atualização de `README.md` com navegação da nova documentação.

## Casos de teste obrigatórios (aceite)
1. Cadastro de professor com email novo: sucesso.
2. Cadastro de professor com email duplicado: bloqueado.
3. Login com credenciais válidas: retorna JWT.
4. Login com credenciais inválidas: `401`.
5. Professor A cadastra aluno/atleta e recupera seus dados.
6. Professor B não acessa atleta/dashboard de A (`404`).
7. Token contém claim `professor_id`.
8. Endpoints protegidos rejeitam requisição sem token.

## Assumptions e defaults
1. Mudanças sujas atuais ficarão fora dos commits da sprint.
2. Rotas de auth/cadastro sem prefixo `/api`.
3. Senha mínima de 6 caracteres (MVP).
4. JWT com TTL de 8 horas, sem refresh token.
5. Documentação mantém nome `aluno`, com equivalência explícita para `Atleta` no texto.
