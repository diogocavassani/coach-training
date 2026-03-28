# Caso de Uso - Cadastrar Professor

## Ator
Professor

## Entrada
- Nome
- Email
- Senha

## Fluxo principal
1. API recebe `POST /professores`.
2. Valida dados obrigatorios.
3. Verifica se email ja existe.
4. Gera hash BCrypt da senha.
5. Persiste professor e retorna `201`.

## Fluxos de erro
- Email duplicado: `409 Conflict`.
- Dados invalidos: `400 Bad Request`.
