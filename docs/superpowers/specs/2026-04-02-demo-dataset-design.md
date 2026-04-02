# Design - Dataset de Demo Reproduzivel para Apresentacao

## Contexto

O objetivo e preparar uma base de demonstracao no PostgreSQL real do projeto para apresentar o sistema a treinadores e coletar feedback sobre:

- coerencia dos calculos;
- clareza dos insights;
- leitura dos dashboards;
- utilidade dos cenarios para o acompanhamento do atleta.

O foco nao e teste automatizado puro. O foco e uma massa de dados plausivel, repetivel e facil de recriar antes de cada demo.

## Objetivo

Criar um mecanismo versionado no repositorio para recriar um dataset fixo de demonstracao, com:

- 1 professor demo;
- 6 atletas, um por cenario tecnico;
- historico suficiente para alimentar os dashboards de forma convincente;
- prova-alvo e planejamento base quando fizer sentido;
- execucao segura e repetivel contra o PostgreSQL do ambiente Docker.

## Abordagens avaliadas

### 1. Seeder em codigo

Popular a base via um executavel/projeto dedicado, usando o mesmo modelo e as mesmas regras da aplicacao.

**Vantagens**
- evolui junto com o dominio;
- evita dados invalidos ou inconsistentes com a API;
- permite cenarios deterministas e explicaveis;
- facilita gerar datas relativas ao momento da demo.

**Desvantagens**
- exige codigo adicional;
- precisa de uma pequena estrutura de manutencao.

### 2. Collection Postman/Newman

Popular a base chamando a API real em sequencia.

**Vantagens**
- valida os endpoints reais;
- util como checklist pre-demo;
- bom para smoke test.

**Desvantagens**
- mais lento para gerar historicos longos;
- pior de manter para 10-12 semanas de treinos coerentes;
- ruim para logica mais rica de geracao.

### 3. Script SQL

Inserir diretamente no PostgreSQL.

**Vantagens**
- rapido;
- facil de executar no banco.

**Desvantagens**
- ignora regras da aplicacao;
- fica mais fragil a mudancas de schema;
- pior para manter coerencia de cenarios ao longo do tempo.

## Recomendacao

Adotar **seeder em codigo como fonte principal** do dataset de demo.

Usar **Postman/Newman como complemento opcional** para smoke test e roteiro de apresentacao.

Nao usar SQL puro como mecanismo principal da demo.

## Design escolhido

### Estrutura

Criar um projeto dedicado:

```text
src/CoachTraining.DemoSeed/
  Program.cs
  DemoSeedOptions.cs
  DemoSeedRunner.cs
  Scenarios/
    DemoScenarioFactory.cs
    BaseEstavelScenario.cs
    ConstrucaoSaudavelScenario.cs
    RiscoCargaAbruptaScenario.cs
    TaperBemExecutadoScenario.cs
    AderenciaBaixaScenario.cs
    DivergenciaCargaRendimentoScenario.cs
  Reports/
    DemoSeedReport.cs
```

Esse projeto usara a mesma configuracao de infraestrutura e o mesmo `CoachTrainingDbContext`, apontando para a base PostgreSQL do ambiente atual.

### Forma de execucao

Comando principal:

```bash
dotnet run --project src/CoachTraining.DemoSeed -- --profile demo-v1 --reset-demo
```

Comportamento esperado:

1. conectar na base configurada;
2. aplicar migrations se necessario;
3. remover apenas o dataset demo anterior;
4. recriar o professor demo;
5. recriar os 6 atletas e seus historicos;
6. imprimir um resumo final para orientar a apresentacao.

### Estrategia de reset

O comportamento padrao deve ser **resetar apenas os dados demo**.

Regra recomendada:

- usar um prefixo padrao em emails e identificadores textuais, por exemplo `demo.`;
- remover apenas professor e atletas pertencentes ao dataset demo;
- nunca limpar a base inteira por padrao.

Se um reset total existir, ele deve ser uma opcao explicita e separada, por exemplo `--reset-all`, para uso tecnico e nao para rotina de demo.

### Estrategia de geracao dos dados

Os dados devem ser:

- deterministas;
- relativos a data atual;
- plausiveis para corrida de rua;
- suficientes para 10-12 semanas de leitura visual.

Regras de geracao:

- usar datas relativas ao dia de execucao, nunca datas fixas antigas;
- manter planejamento base em todos os atletas, salvo motivo clinico forte;
- gerar sessoes com combinacao coerente de `tipo`, `duracao`, `distancia` e `RPE`;
- garantir que os cenarios forcem os insights esperados sem parecerem artificiais demais;
- gerar nomes e descricoes que ajudem o apresentador a localizar rapidamente cada atleta.

## Dataset demo v1

### Professor demo

- Nome: `Professor Demo`
- Email: `demo.professor@coachtraining.local`
- Senha: `Demo@123456`

### Cenarios obrigatorios

#### 1. Base estavel

- Objetivo: mostrar um atleta sem alertas relevantes.
- Historico: carga estavel nas ultimas semanas.
- Planejamento: 4 treinos por semana.
- Prova-alvo: nao obrigatoria.
- Resultado esperado:
  - fase mais estavel;
  - risco normal;
  - sem insight critico.

#### 2. Construcao saudavel

- Objetivo: mostrar progressao boa e controlada.
- Historico: aumento gradual de carga.
- Planejamento: 5 treinos por semana.
- Prova-alvo: opcional, mas nao em taper.
- Resultado esperado:
  - fase de construcao;
  - sem risco alto;
  - evolucao considerada coerente.

#### 3. Risco por aumento abrupto de carga

- Objetivo: validar leitura de sobrecarga.
- Historico: semanas anteriores moderadas e semana atual muito acima.
- Planejamento: 4 treinos por semana.
- Resultado esperado:
  - delta semanal alto;
  - ACWR elevado;
  - status de risco alto;
  - insights de sobrecarga.

#### 4. Taper bem executado

- Objetivo: mostrar a leitura pre-prova correta.
- Historico: prova em 7-21 dias e reducao de volume de 40-60%.
- Planejamento: 5 treinos por semana.
- Prova-alvo: obrigatoria.
- Resultado esperado:
  - atleta em janela de taper;
  - insight de taper adequado;
  - visual convincente de reducao de carga.

#### 5. Aderencia baixa ao planejamento

- Objetivo: validar planejado x realizado.
- Historico: planejamento de 5 treinos, execucao recente de 2.
- Resultado esperado:
  - aderencia abaixo do esperado;
  - insight claro sobre baixa execucao;
  - contraste visivel no dashboard.

#### 6. Divergencia carga x rendimento

- Objetivo: discutir se o sistema esta detectando fadiga de forma util.
- Historico: carga recente aumenta enquanto o pace medio piora.
- Planejamento: 5 treinos por semana.
- Resultado esperado:
  - insight de divergencia entre carga e rendimento;
  - historico visualmente plausivel nas series semanais.

## Relatorio final do seed

Ao terminar, o seeder deve imprimir:

- credenciais do professor demo;
- lista dos 6 atletas;
- qual insight principal cada um deve exibir;
- observacoes uteis para a apresentacao.

Exemplo:

```text
Professor demo: demo.professor@coachtraining.local / Demo@123456
- Ana Souza - Base estavel -> sem alerta critico
- Bruno Lima - Construcao saudavel -> progressao controlada
- Carla Mendes - Risco por carga abrupta -> ACWR e delta altos
- Diego Alves - Taper bem executado -> taper adequado
- Fernanda Rocha - Aderencia baixa -> baixa aderencia ao planejamento
- Gustavo Nunes - Divergencia carga x rendimento -> carga sobe e pace piora
```

## O que fica fora deste escopo

- popular dados aleatorios grandes para teste de performance;
- substituir testes automatizados;
- criar dados multi-professor complexos;
- criar interface administrativa para seed;
- sincronizar seed por API externa.

## Validacao esperada

### Validacao manual

- rodar o seed no PostgreSQL do Docker;
- autenticar com o professor demo;
- abrir a home do professor;
- abrir os 6 dashboards individuais;
- confirmar se os insights e graficos batem com o roteiro esperado.

### Validacao automatizada minima

- teste do `DemoScenarioFactory` para garantir 6 cenarios;
- teste do `DemoSeedRunner` para garantir comportamento deterministico;
- smoke test opcional por API ou consulta do banco validando que o dataset foi criado.

## Extensoes futuras

- perfil `demo-v2` com mais atletas;
- perfil `feedback-clinico` com casos mais sensiveis;
- collection Postman/Newman para smoke test da demo;
- fixture PostgreSQL/Testcontainers para reaproveitar os mesmos cenarios em testes de integracao.
