# ADR-002 - Isolamento Multi-tenant por ProfessorId

- Status: Aceita
- Data: 2026-03-27

## Contexto
Cada professor deve visualizar e operar apenas seus proprios atletas e dashboards.

## Decisao
Aplicar isolamento por `ProfessorId` em toda operacao sensivel:
- `Atleta` possui `ProfessorId` obrigatorio.
- Banco com FK obrigatoria `atletas.professor_id`.
- Repositorios filtram por `ProfessorId`.
- Endpoints extraem `professor_id` do JWT e nunca recebem `ProfessorId` no payload de cadastro de atleta.

## Consequencias
- Evita vazamento de dados entre professores.
- Consulta de atleta/dashboard retorna `404` quando o recurso existe mas pertence a outro tenant.
- Maior disciplina de contrato em repositorios e servicos.
