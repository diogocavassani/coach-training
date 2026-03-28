# Fluxo - Cadastro de Professor

1. Cliente envia `POST /professores`.
2. API valida campos.
3. App verifica duplicidade de email.
4. Infra aplica hash BCrypt.
5. Infra persiste em `professores`.
6. API retorna `201`.
7. Cliente executa `POST /auth/login` para obter JWT.
