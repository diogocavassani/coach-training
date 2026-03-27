# ADR-001 - Autenticacao de Professor com JWT

- Status: Aceita
- Data: 2026-03-27

## Contexto
O sistema precisava habilitar login de professor para proteger endpoints e vincular dados ao usuario autenticado.

## Decisao
Adotar autenticacao propria com:
- cadastro via `POST /professores`
- login via `POST /auth/login`
- token JWT assinado com claim `professor_id`
- hash de senha com BCrypt

## Consequencias
- Simplicidade e entrega rapida para MVP.
- Menor complexidade operacional que OAuth/Identity completo.
- Necessidade de gerir segredo JWT por ambiente.
- Sem refresh token neste momento (relogin apos expiracao).
