# Caso de Uso - Cadastrar Aluno

> Aluno na documentacao equivale a `Atleta` no codigo.

## Ator
Professor autenticado

## Entrada
- Nome
- ObservacoesClinicas (opcional)
- NivelEsportivo (opcional)

## Fluxo principal
1. API recebe `POST /api/atleta` com bearer token.
2. Extrai `professor_id` do JWT.
3. Cria aluno/atleta com `ProfessorId` do token.
4. Persiste registro.
5. Retorna `201 Created`.

## Fluxos de erro
- Sem token ou token invalido: `401 Unauthorized`.
- Dados invalidos: `400 Bad Request`.
