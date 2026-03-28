# Regras de Negocio

## Professor
- Email deve ser unico.
- Senha minima de 6 caracteres.
- Senha armazenada apenas em hash BCrypt.

## Autenticacao
- Login valida email + senha.
- JWT deve conter `professor_id`.
- Endpoints sensiveis exigem autenticacao.

## Aluno/Atleta
- `ProfessorId` obrigatorio.
- Cadastro nao aceita `ProfessorId` no payload.
- Viculo sempre derivado do token.

## Isolamento
- Nenhuma consulta de atleta/dashboard pode retornar dados de outro professor.
- Caso de acesso cruzado deve responder `404`.
