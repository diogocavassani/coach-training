# Arquitetura - Fluxo Professor

## Objetivo
Suportar cadastro e autenticacao de professor, com isolamento de dados por tenant (`ProfessorId`) no MVP.

## Camadas
- `CoachTraining.Api`: endpoints HTTP e autentificacao JWT.
- `CoachTraining.App`: casos de uso (`CadastroProfessorService`, `LoginProfessorService`, `CadastroAtletaService`).
- `CoachTraining.Domain`: regras de negocio (`Professor`, `Atleta` com `ProfessorId` obrigatorio).
- `CoachTraining.Infra`: persistencia EF Core, repositorios, hashing e emissao de token.

## Fluxo principal
1. `POST /professores` cria conta de professor com hash de senha.
2. `POST /auth/login` valida credenciais e retorna JWT.
3. Endpoints protegidos extraem `professor_id` do token.
4. Repositorios filtram consultas por `ProfessorId`.

## Isolamento de dados
- Relacao obrigatoria `Professor (1) -> (N) Atletas`.
- FK em banco: `atletas.professor_id -> professores.id`.
- Consultas de atleta/dashboard sempre usam `(AtletaId + ProfessorId)`.

## Seguranca no MVP
- JWT com expiracao de 8 horas.
- Claim obrigatoria `professor_id`.
- Sem roles, sem refresh token e sem OAuth neste ciclo.
