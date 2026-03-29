# Dominio - Aluno

> Observacao: nesta documentacao, **aluno** corresponde a entidade `Atleta` no codigo.

## Entidade (Atleta)
- `Id`
- `ProfessorId`
- `Nome`
- `Email`
- `ObservacoesClinicas`
- `NivelEsportivo`

## Regras
- `ProfessorId` obrigatorio.
- Aluno nao existe sem professor.
- `Email` e opcional, mas quando informado deve ser valido.
- Cadastro de aluno nao aceita `ProfessorId` no payload; origem sempre do token autenticado.

## Isolamento
- Leituras de aluno sempre filtradas por `ProfessorId`.
