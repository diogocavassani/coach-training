# Backlog do MVP - CoachTraining

> Ultima atualizacao: 2026-04-01
> Fonte de verdade do escopo: `docs/projeto_sistema_de_monitoramento_de_carga_e_preparacao_de_atletas.md`

## Como usar este arquivo

- Cada item do backlog usa checkbox.
- Ao concluir uma entrega, trocar `- [ ]` por `- [x]` no item principal e nos criterios de aceite atendidos.
- Se surgir uma subtarefa tecnica durante a implementacao, adicionar como subitem no bloco do item correspondente.
- Nao marcar um item como concluido sem validacao minima, testes relevantes e atualizacao de documentacao quando aplicavel.
- Itens fora do MVP devem ficar em outro backlog para nao inflar este arquivo.

## Definicao de pronto do MVP

- [ ] O professor consegue se cadastrar e autenticar.
- [ ] O professor consegue cadastrar atleta, prova-alvo, planejamento base e sessoes de treino.
- [ ] O dashboard individual do atleta mostra metricas, alertas e aderencia ao planejamento.
- [ ] O frontend fecha build de producao sem erro.
- [ ] O ambiente principal sobe via Docker Compose.
- [ ] A documentacao principal reflete o comportamento real do sistema.

## Entregas ja concluidas fora deste backlog

- [x] Cadastro de professor.
- [x] Login com JWT.
- [x] Cadastro e listagem de atletas.
- [x] Cadastro de sessoes de treino.
- [x] Dashboard individual do atleta com metricas, graficos e exportacao.
- [x] Persistencia com EF Core, migrations e repositorios.
- [x] Suite `dotnet test CoachTraining.sln` verde na data desta revisao.

## P0 - Bloqueadores do MVP

### [x] BL-01 - Corrigir prontidao de entrega do frontend

**Objetivo:** garantir que o frontend rode em producao e no fluxo Docker sem falhar no build.

**Criterios de aceite**

- [x] `npm run build` executa com sucesso em `frontend/`.
- [x] O build de producao do Angular nao estoura budget ou o budget e revisado de forma consciente e justificada.
- [x] `frontend/Dockerfile` conclui a imagem sem falha.
- [x] `docker compose up --build` consegue subir o frontend no fluxo principal do projeto.
- [x] A estrategia adotada para bundle size fica documentada se houver decisao tecnica relevante.

**Validacao minima**

- [x] Rodar `npm run build`.
- [x] Rodar `docker compose up --build` a partir de `infra/`.

### [x] BL-02 - Implementar cadastro de prova-alvo por atleta

**Objetivo:** permitir que o professor cadastre e atualize a prova-alvo usada pelo dashboard.

**Criterios de aceite**

- [x] Existe endpoint autenticado para criar ou atualizar prova-alvo por atleta.
- [x] Existe consulta autenticada para recuperar a prova-alvo do atleta quando necessario.
- [x] A persistencia de prova-alvo esta integrada ao modelo atual.
- [x] O frontend possui formulario ou fluxo claro para cadastrar e editar prova-alvo.
- [x] O dashboard individual passa a refletir a prova cadastrada sem dependencias manuais.
- [x] Existem testes cobrindo fluxo feliz e validacoes principais.

**Validacao minima**

- [x] Rodar testes de backend relacionados.
- [x] Validar o fluxo manual: cadastrar atleta, cadastrar prova-alvo e abrir o dashboard.

### [x] BL-03 - Implementar planejamento base por atleta

**Objetivo:** registrar o numero de treinos semanais planejados para cada atleta.

**Criterios de aceite**

- [x] Existe modelo de dados para planejamento base por atleta.
- [x] Existe endpoint autenticado para criar ou atualizar o planejamento base.
- [x] Existe endpoint autenticado para consultar o planejamento do atleta quando necessario.
- [x] O frontend possui fluxo para definir e editar treinos planejados por semana.
- [x] Regras de validacao basicas estao definidas e testadas.
- [x] O dado fica pronto para ser consumido pelo dashboard e insights.

**Validacao minima**

- [x] Rodar testes de backend relacionados.
- [x] Validar o fluxo manual no frontend.

### [ ] BL-04 - Exibir aderencia ao planejamento no dashboard

**Objetivo:** fechar o ciclo entre planejamento base e execucao real dos treinos.

**Criterios de aceite**

- [ ] O dashboard expone planejado vs realizado de forma objetiva.
- [ ] O dashboard calcula um indicador de aderencia para o periodo analisado.
- [ ] Existe insight textual quando a aderencia estiver fora do esperado.
- [ ] O frontend mostra o indicador de aderencia em local claro no dashboard do atleta.
- [ ] Existem testes cobrindo o calculo e a exibicao do novo indicador.

**Validacao minima**

- [ ] Rodar testes de backend e frontend relacionados.
- [ ] Validar manualmente cenarios com aderencia alta e baixa.

## P1 - Fechamento da experiencia do produto

### [ ] BL-05 - Substituir o dashboard inicial do professor por dados reais

**Objetivo:** remover a tela placeholder da area autenticada e entregar um resumo operacional verdadeiro.

**Criterios de aceite**

- [ ] A tela `/dashboard` deixa de usar KPIs fixos.
- [ ] A tela mostra indicadores reais do workspace do professor.
- [ ] O conjunto inicial de indicadores e simples, util e coerente com o MVP.
- [ ] Se for necessario um novo endpoint ou query, ele fica coberto por testes.
- [ ] O fluxo de navegacao a partir da home autenticada ajuda o professor a chegar rapidamente nas acoes principais.

**Validacao minima**

- [ ] Validar manualmente a tela autenticada com dados reais.
- [ ] Rodar testes relevantes do frontend e backend envolvidos.

### [ ] BL-06 - Completar os insights prometidos no escopo do MVP

**Objetivo:** alinhar os insights implementados ao escopo documentado do produto.

**Criterios de aceite**

- [ ] Existe regra objetiva para monotonia de carga, caso ela permaneca no escopo.
- [ ] Existe regra objetiva para divergencia entre carga e rendimento, ou o escopo e a documentacao sao ajustados para remover essa promessa.
- [ ] Existe insight para aderencia ao planejamento depois da entrega do `BL-04`.
- [ ] Os novos insights possuem testes automatizados.
- [ ] A documentacao de regras de negocio e insights e atualizada.

**Validacao minima**

- [ ] Rodar testes da camada de aplicacao e dominio relacionados.
- [ ] Revisar exemplos de mensagens retornadas no dashboard.

## P2 - Fechamento documental e operacional

### [ ] BL-07 - Alinhar a documentacao ao estado real do sistema

**Objetivo:** deixar README e docs coerentes com o que o projeto realmente entrega.

**Criterios de aceite**

- [ ] O `README.md` reflete o status atual do MVP.
- [ ] A documentacao de setup e execucao nao aponta para fluxos quebrados ou desatualizados.
- [ ] O endpoint de health-check documentado bate com a rota real.
- [ ] As referencias antigas a persistencia pendente sao removidas ou corrigidas.
- [ ] As novas funcionalidades de prova-alvo e planejamento entram na documentacao quando implementadas.
- [ ] Este backlog continua sendo atualizado nas PRs que alterarem o estado do MVP.

**Validacao minima**

- [ ] Revisao manual dos documentos alterados.
- [ ] Conferencia de comandos e rotas descritos na documentacao.

## Ordem sugerida de execucao

1. [x] BL-01 - Corrigir prontidao de entrega do frontend.
2. [x] BL-02 - Implementar cadastro de prova-alvo por atleta.
3. [x] BL-03 - Implementar planejamento base por atleta.
4. [ ] BL-04 - Exibir aderencia ao planejamento no dashboard.
5. [ ] BL-05 - Substituir o dashboard inicial do professor por dados reais.
6. [ ] BL-06 - Completar os insights prometidos no escopo do MVP.
7. [ ] BL-07 - Alinhar a documentacao ao estado real do sistema.

## Regra de manutencao

- [ ] Ao concluir um item principal, marcar tambem os criterios de aceite e validacoes minimas correspondentes.
- [ ] Manter este arquivo atualizado na mesma PR da entrega implementada.
- [ ] Se um item mudar de escopo, ajustar este backlog e a documentacao de produto no mesmo ciclo.
