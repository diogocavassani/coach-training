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
- Monotonia de carga considera a media e o desvio-padrao das cargas diarias da janela movel dos ultimos 7 dias, incluindo dias sem treino.
- O proxy de rendimento do MVP e o pace medio semanal, calculado apenas quando existe distancia valida registrada na semana.
- Divergencia carga x rendimento e sinalizada quando a carga recente sobe e o pace medio piora nas ultimas 4 semanas comparaveis.

## Isolamento
- Nenhuma consulta de atleta/dashboard pode retornar dados de outro professor.
- Caso de acesso cruzado deve responder `404`.
