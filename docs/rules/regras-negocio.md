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
- `TreinosPlanejadosPorSemana`, quando informado, deve ficar entre 1 e 14.

## Dashboard
- Aderencia ao planejamento compara treinos planejados por semana com os treinos realizados na janela movel dos ultimos 7 dias.
- O dashboard so calcula aderencia percentual quando existe planejamento base cadastrado para o atleta.

## Isolamento
- Nenhuma consulta de atleta/dashboard pode retornar dados de outro professor.
- Caso de acesso cruzado deve responder `404`.
