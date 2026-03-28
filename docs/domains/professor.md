# Dominio - Professor

## Entidade
- `Id`
- `Nome`
- `Email`
- `SenhaHash`
- `DataCriacao`

## Regras
- Nome obrigatorio.
- Email obrigatorio, valido e normalizado (lowercase).
- Senha nunca em texto plano; apenas `SenhaHash`.
- Email deve ser unico no sistema.

## Responsabilidades
- Representar o dono dos dados de atletas.
- Ser principal de autenticacao do MVP.
