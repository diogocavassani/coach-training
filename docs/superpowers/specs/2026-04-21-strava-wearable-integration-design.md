# Design - Integracao publica de wearables com Strava como primeiro provedor

## Contexto

O CoachTraining hoje possui um fluxo autenticado centrado no professor. O professor cadastra atletas, define prova-alvo, ajusta planejamento base e registra sessoes de treino manualmente. O sistema ainda nao possui um fluxo publico controlado para que o proprio aluno conecte contas externas nem um modulo tecnico dedicado a integracoes com provedores de atividades.

O novo objetivo e permitir que o professor envie a um aluno um link publico, reutilizavel e opaco, sem exigir login do aluno e sem expor qualquer identificador previsivel na URL. A partir desse link, o aluno deve conseguir autorizar provedores de wearables, comecando por Strava, para que as atividades concluídas sejam importadas automaticamente para a plataforma.

Essa frente toca tres preocupacoes ao mesmo tempo:

- experiencia do professor e do aluno;
- seguranca de links publicos, OAuth e webhooks;
- arquitetura extensivel para Strava agora e Garmin/Polar depois.

## Objetivo

Criar uma arquitetura de integracoes publicas com wearables que permita:

- o professor gerar e copiar um link publico por atleta;
- o aluno autorizar o envio de atividades sem login;
- o backend concluir o OAuth do Strava com seguranca;
- o sistema importar automaticamente atividades novas recebidas via webhook;
- a implementacao permanecer preparada para novos provedores sem duplicacao estrutural.

## Decisoes aprovadas no alinhamento

Durante o brainstorming, as seguintes decisoes foram aprovadas:

- o link publico do aluno sera `reutilizavel`, nao expira por tempo automaticamente e pode ser regenerado para invalidacao manual;
- a primeira integracao sera `Strava`;
- o fluxo publico ficara no `mesmo frontend Angular`, mas fora da area autenticada do professor;
- a URL publica nao pode expor identificadores do aluno, professor ou registro interno;
- a importacao de atividades sera `automatica`;
- a importacao `nao fara validacao de conflito de negocio` com atividade manual ja existente;
- mesmo sem validacao de negocio, a implementacao deve manter `idempotencia tecnica` para evitar duplicacao causada por retry de webhook;
- o backend e o frontend devem nascer com estrutura `anemica, clara e extensivel` para novos provedores.

## Direcao escolhida

A direcao aprovada foi a abordagem:

### Link publico por atleta + estado assinado + modulo generico de provedores

Nessa abordagem:

- cada atleta possui um link publico reutilizavel representado por um token opaco;
- a pagina publica lista provedores disponiveis, comecando por Strava;
- a autorizacao OAuth sempre e iniciada e concluida pelo backend;
- o backend usa um `state` assinado, temporario e de uso unico para amarrar o callback ao contexto correto;
- as conexoes com provedores sao modeladas de forma generica, com credenciais e eventos desacoplados do core de treino;
- o webhook do Strava apenas dispara processamento assincrono, que busca o detalhe da atividade e o traduz para o modelo interno.

Essa direcao foi escolhida por equilibrar seguranca, clareza de fluxo e preparo real para Garmin/Polar sem criar um mini-sistema separado antes da hora.

## Abordagens consideradas

### 1. Link publico por atleta + estado assinado + modulo generico de provedores

O professor gera um link publico reutilizavel. O aluno abre esse link, escolhe um provedor e segue por um fluxo de autorizacao controlado pelo backend. Cada provedor implementa o mesmo contrato base de autorizacao, refresh, leitura de atividade e traducao de webhook.

**Vantagens**
- separa bem o core do produto dos adaptadores externos;
- permite reaproveitar o mesmo fluxo publico para Garmin e Polar;
- reduz risco de hardcode de Strava em rotas, DTOs e telas;
- acomoda OAuth, refresh token, webhook e revogacao com clareza.

**Desvantagens**
- exige um pouco mais de modelagem de dominio e persistencia desde a primeira entrega.

### 2. Link publico com fluxo embutido diretamente em Strava

O link publico cairia em uma pagina que ja conhece Strava e conversa com endpoints especificos, sem uma camada generica de provedores.

**Vantagens**
- entrega mais rapido a primeira integracao.

**Desvantagens**
- tende a gerar duplicacao quando Garmin/Polar entrarem;
- espalha conhecimento do provedor em varias camadas;
- aumenta o custo de extensao e de manutencao.

### 3. Portal publico separado da SPA principal

Criar uma aplicacao publica isolada apenas para integracoes.

**Vantagens**
- forte isolamento entre area publica e area autenticada;
- mais liberdade de evolucao visual e operacional.

**Desvantagens**
- cria sobrecusto tecnico cedo demais;
- nao acompanha a decisao de manter o fluxo no frontend atual;
- adiciona deploy, roteamento e operacao desnecessarios para a fase atual.

## Recomendacao adotada

Adotar a abordagem `1`, com link publico por atleta, contratos genericos para provedores, modulo dedicado de integracoes e Strava como primeira implementacao concreta.

## Requisitos funcionais

### Fluxo do professor

- o professor deve ver um bloco `Integracoes` dentro da visao do aluno;
- o professor deve poder gerar o link publico do aluno;
- o professor deve poder copiar o link publico;
- o professor deve poder regenerar o link, invalidando o anterior;
- o professor deve poder ver o status de cada provedor disponivel para aquele aluno;
- o professor deve poder ver, no minimo, status atual e ultima sincronizacao conhecida do provedor conectado.

### Fluxo do aluno

- o aluno nao faz login;
- o aluno acessa uma URL publica opaca;
- a pagina publica lista os provedores habilitados;
- o aluno seleciona `Strava`;
- o sistema redireciona o aluno para a autorizacao do Strava;
- apos retorno do callback, o sistema conclui a conexao e mostra sucesso ou erro em uma pagina publica da plataforma.

### Fluxo de importacao

- apos autorizacao concluida, o backend passa a receber eventos do Strava para aquele atleta;
- quando chegar uma atividade nova, o sistema importa automaticamente uma nova sessao de treino;
- o sistema nao tenta impedir a importacao com base em treinos manuais existentes;
- o sistema evita duplicacao puramente tecnica causada por retry do webhook ou reprocessamento do mesmo evento.

## Requisitos nao funcionais

- nenhum segredo do Strava pode ser exposto ao frontend;
- o token do link publico deve ser imprevisivel e nao derivado do `AtletaId`;
- o token do link publico nao deve ser persistido em texto puro no banco;
- credenciais do atleta devem ser protegidas em repouso;
- o webhook deve responder ao Strava dentro do limite operacional da doc e delegar o processamento pesado de forma assincrona;
- a arquitetura deve permitir adicionar novos provedores com impacto local;
- o fluxo publico deve ser reutilizavel sem acoplar a experiencia do aluno ao shell autenticado do professor.

## Referencias oficiais do Strava

Esta especificacao se apoia em tres pontos da documentacao oficial do Strava:

- a autorizacao e baseada em OAuth 2.0 com `code`, `refresh_token` e `access_token` de curta duracao: [Authentication](https://developers.strava.com/docs/authentication/)
- o webhook envia apenas metadados do evento e exige que o callback responda `200 OK` rapidamente: [Webhook Events API](https://developers.strava.com/docs/webhooks/)
- o detalhe da atividade deve ser buscado depois pelo endpoint `Get Activity`: [API Reference - Get Activity](https://developers.strava.com/docs/reference/#api-Activities-getActivityById)

## Escopos iniciais do Strava

Para a primeira versao, a integracao pedira:

- `activity:read`

Justificativa:

- e o menor escopo pratico para receber e consultar atividades visiveis a `Everyone` e `Followers`;
- reduz friccao na autorizacao inicial;
- mantem a primeira entrega mais conservadora em privacidade.

Consequencia explicita:

- atividades `Only You` nao entram na primeira versao;
- se futuramente for necessario importar tambem atividades privadas, a evolucao prevista sera ampliar para `activity:read_all`, com ajuste de comunicacao ao aluno e reautorizacao quando necessario.

## Arquitetura alvo

### Principio central

O dominio canonico do produto continua sendo o treino interno do CoachTraining. Strava e futuros wearables atuam como fontes externas que:

- autenticam o aluno;
- notificam eventos;
- fornecem detalhes de atividade;
- sao traduzidos para o modelo interno de `SessaoDeTreino`.

O core do sistema nao deve depender de tipos, formatos ou regras do Strava alem dos adaptadores e contratos de integracao.

### Camadas

#### API

Responsavel por expor:

- endpoints privados do professor para gerenciamento do link e visualizacao de status;
- endpoints publicos do fluxo do aluno;
- callback de OAuth;
- endpoint de webhook com validacao de `hub.challenge`.

Controllers desta frente nao concentram regra de negocio de integracao. Eles validam entrada, resolvem identidade do professor quando aplicavel, chamam services e retornam respostas.

#### Application

Responsavel por coordenar:

- geracao e regeneracao de links publicos;
- resolucao de link publico;
- inicio de autorizacao de um provedor;
- conclusao do callback OAuth;
- registro e atualizacao de conexoes;
- persistencia de credenciais protegidas;
- recepcao e enfileiramento de webhooks;
- importacao de atividade externa para `SessaoDeTreino`.

Essa camada abriga os contratos genericos dos provedores e os casos de uso orientados ao fluxo do produto.

#### Domain

Responsavel por representar:

- link publico de integracao;
- conexao de um atleta com um provedor;
- status da conexao;
- origem do treino;
- evento importado;
- regras de consistencia basicas desses objetos.

O dominio nao conhece HTTP, OAuth endpoint nem detalhes de serializacao do Strava.

#### Infrastructure

Responsavel por implementar:

- persistencia EF Core das novas entidades;
- protecao criptografica de credenciais;
- cliente HTTP do Strava;
- tradutor de payload do Strava;
- fila ou mecanismo de processamento assincrono de webhooks;
- registracao de provedores disponiveis na DI.

## Modelo de dominio proposto

### LinkPublicoIntegracao

Representa o link reutilizavel do aluno.

Campos recomendados:

- `Id`
- `AtletaId`
- `TokenHash`
- `TokenVersao`
- `Ativo`
- `CriadoEmUtc`
- `RegeneradoEmUtc`
- `RevogadoEmUtc`

Regras:

- o token real e gerado aleatoriamente e entregue apenas para o professor/copiar para o aluno;
- apenas o `hash` do token e persistido;
- regenerar o link invalida o token anterior e emite um novo;
- o mesmo atleta possui apenas um link ativo por vez.

### ConexaoWearable

Representa a conexao de um atleta com um provedor externo.

Campos recomendados:

- `Id`
- `AtletaId`
- `Provedor`
- `Status`
- `ExternalAthleteId`
- `ScopesConcedidos`
- `ConectadoEmUtc`
- `DesconectadoEmUtc`
- `UltimaSincronizacaoEmUtc`
- `UltimoErro`

Status previstos:

- `NaoConectado`
- `Conectado`
- `ErroAutorizacao`
- `RequerReconexao`
- `Desconectado`

Regras:

- um atleta pode ter no maximo uma conexao ativa por provedor;
- reconexao sobrescreve credenciais anteriores do mesmo provedor;
- se o Strava sinalizar revogacao, a conexao vira `Desconectado`.

### CredencialWearable

Mantem os segredos fora da entidade principal da conexao.

Campos recomendados:

- `Id`
- `ConexaoWearableId`
- `AccessTokenProtegido`
- `RefreshTokenProtegido`
- `ExpiresAtUtc`
- `AtualizadoEmUtc`

Regras:

- os campos sensiveis ficam protegidos em repouso;
- sempre que o refresh do Strava retornar novo `refresh_token`, o valor mais recente substitui o anterior;
- o consumo de APIs do provedor deve sempre ler a credencial mais atual.

### EventoWebhookRecebido

Representa o recebimento e processamento do webhook.

Campos recomendados:

- `Id`
- `Provedor`
- `SubscriptionId`
- `ObjectType`
- `ObjectId`
- `OwnerId`
- `AspectType`
- `PayloadJson`
- `Fingerprint`
- `RecebidoEmUtc`
- `ProcessadoEmUtc`
- `StatusProcessamento`
- `ErroProcessamento`

Status previstos:

- `Recebido`
- `Enfileirado`
- `Processado`
- `Falhou`
- `Ignorado`

Objetivo:

- auditoria;
- reprocessamento controlado;
- rastreabilidade operacional.

### AtividadeImportada

Representa o vinculo entre uma atividade externa e a sessao de treino criada internamente.

Campos recomendados:

- `Id`
- `Provedor`
- `ConexaoWearableId`
- `ExternalActivityId`
- `SessaoDeTreinoId`
- `ImportadoEmUtc`
- `PayloadResumoJson`

Regras:

- deve existir unicidade em `Provedor + ExternalActivityId`;
- isso garante idempotencia tecnica sem bloquear a regra de negocio aprovada de importar automaticamente.

### Evolucao em SessaoDeTreino

`SessaoDeTreino` deve passar a registrar a origem do treino.

Campo recomendado:

- `OrigemTreino`

Valores iniciais:

- `Manual`
- `Strava`

Essa pequena extensao preserva o core como fonte canonica, mas permite rastrear a origem operacional do dado.

## Contratos para provedores

### Interface principal

O backend deve trabalhar com um contrato generico de provedor, por exemplo `IWearableProvider`.

Responsabilidades:

- montar dados da autorizacao;
- trocar `code` por credenciais;
- atualizar tokens expirados;
- buscar detalhes de atividade;
- traduzir payloads de webhook para um evento interno conhecido pela aplicacao.

Metodos esperados:

- `BuildAuthorizationRequest(...)`
- `ExchangeAuthorizationCodeAsync(...)`
- `RefreshAccessTokenAsync(...)`
- `GetActivityAsync(...)`
- `ParseWebhookEvent(...)`

### Registro de provedores

Os provedores devem ser resolvidos por `enum` ou chave canonica (`strava`, `garmin`, `polar`) a partir de um registro central.

Beneficios:

- evita `switch` espalhado pela aplicacao;
- permite exibir provedores disponiveis no frontend a partir do backend;
- facilita habilitar/desabilitar provedores por configuracao.

### Primeira implementacao concreta

`StravaWearableProvider` sera a primeira implementacao do contrato.

Responsabilidades especificas:

- gerar a URL de autorizacao do Strava com `client_id`, `redirect_uri`, `scope`, `approval_prompt` e `state`;
- trocar `code` por tokens no endpoint de OAuth;
- aplicar refresh quando necessario;
- buscar atividade detalhada em `GET /activities/{id}`;
- converter o webhook bruto do Strava para um modelo interno de evento.

## Fluxo aprovado

### 1. Professor gera ou copia link publico

1. O professor entra na tela autenticada do aluno.
2. O frontend chama endpoint privado do professor para obter o link atual ou gerar um novo.
3. O backend gera um token aleatorio opaco e armazena apenas o hash.
4. O professor copia uma URL publica no formato:
   - `https://coachtraining.com/conectar/{token}`

Regras:

- a URL nunca inclui `AtletaId`, nome, email ou `ProfessorId`;
- o link permanece valido ate ser regenerado ou revogado.

### 2. Aluno abre a pagina publica

1. O aluno acessa `GET /conectar/{token}` na SPA.
2. O frontend publico consulta `GET /public/integracoes/{token}`.
3. O backend valida o token por hash e responde:
   - provedores disponiveis;
   - estado atual de cada provedor para esse token;
   - mensagens neutras de UX.

O backend nao devolve dados sensiveis do atleta. Se for necessario contexto visual, ele devolve apenas um texto neutro.

### 3. Aluno inicia OAuth do Strava

1. O aluno clica em `Conectar com Strava`.
2. O frontend chama `POST /public/integracoes/{token}/strava/autorizar`.
3. O backend:
   - valida o token publico;
   - gera um `state` assinado, temporario e de uso unico;
   - monta a URL de autorizacao do Strava;
   - devolve a URL ao frontend.
4. O frontend redireciona o navegador do aluno para o Strava.

Conteudo minimo do `state`:

- identificador do link publico ou do contexto de autorizacao;
- provedor;
- `nonce`;
- instante de expiracao curto.

### 4. Strava chama o callback da plataforma

1. O Strava redireciona para `GET /public/integracoes/strava/callback` com `code`, `scope` e `state`.
2. O backend valida o `state`.
3. O backend troca o `code` por `refresh_token`, `access_token` e `expires_at`.
4. O backend verifica quais escopos foram efetivamente concedidos.
5. O backend cria ou atualiza `ConexaoWearable` e `CredencialWearable`.
6. O backend redireciona o navegador para a rota publica fixa `/conectar/strava/retorno`.
7. A rota publica renderiza sucesso ou erro a partir de um resultado seguro devolvido pelo backend, sem expor `code`, tokens ou detalhes sensiveis na UI.

Tratamento de falhas:

- `error=access_denied` retorna pagina publica de recusa de autorizacao;
- `state` invalido ou expirado retorna erro tecnico seguro;
- escopo insuficiente retorna erro orientando nova tentativa.

### 5. Strava envia webhook

1. O Strava envia eventos para o callback registrado.
2. O endpoint responde `200 OK` rapidamente.
3. O payload e salvo como `EventoWebhookRecebido`.
4. Um processador assincrono traduz e trata o evento.
5. Para `activity:create`, o processador:
   - localiza a conexao ativa por `owner_id`;
   - garante idempotencia tecnica por `external_activity_id`;
   - busca o detalhe da atividade no Strava;
   - traduz a atividade para `SessaoDeTreino`;
   - cria a sessao;
   - registra `AtividadeImportada`;
   - atualiza `UltimaSincronizacaoEmUtc`.

## Rotas recomendadas

### Endpoints privados do professor

- `GET /api/atletas/{id}/integracoes`
  Retorna status dos provedores do atleta e o link publico atual, se existir.

- `POST /api/atletas/{id}/integracoes/link`
  Gera o primeiro link ou devolve o atual.

- `POST /api/atletas/{id}/integracoes/link/regenerar`
  Invalida o token atual e gera um novo.

### Endpoints publicos do aluno

- `GET /public/integracoes/{token}`
  Resolve o token publico e devolve os provedores disponiveis.

- `POST /public/integracoes/{token}/strava/autorizar`
  Inicia a autorizacao do Strava.

- `GET /public/integracoes/strava/callback`
  Recebe o retorno do Strava e conclui a conexao.

### Endpoints de webhook do Strava

- `GET /api/integrations/strava/webhook/{secret}`
  Valida `hub.challenge` e `hub.verify_token`.

- `POST /api/integrations/strava/webhook/{secret}`
  Recebe eventos e os enfileira para processamento.

Observacao:

- o uso de um segmento secreto no path do webhook e proposital, ja que a documentacao consultada do Strava nao descreve assinatura criptografica no POST do webhook.

## Persistencia e schema

### Tabelas novas

- `links_publicos_integracao`
- `conexoes_wearable`
- `credenciais_wearable`
- `eventos_webhook_recebidos`
- `atividades_importadas`

### Ajustes em tabelas existentes

- `sessoes_treino`
  Adicionar coluna `origem_treino`

### Restricoes recomendadas

- os links publicos devem ser `versionados historicamente`, nao sobrescritos em linha;
- indice unico em `token_hash`;
- indice unico parcial em `links_publicos_integracao(atleta_id)` onde `ativo = true`, garantindo apenas um link ativo por atleta;
- indice unico em `conexoes_wearable(atleta_id, provedor)`;
- indice unico em `atividades_importadas(provedor, external_activity_id)`;
- indice em `eventos_webhook_recebidos(fingerprint)` para apoio a deduplicacao tecnica de eventos identicos.

## Seguranca

### Segredos da aplicacao

Os seguintes dados ficam apenas no backend:

- `client_secret` do Strava;
- `verify_token` do webhook;
- segredo usado para montar o path secreto do webhook;
- chave de protecao de dados usada para criptografar credenciais dos atletas.

Esses valores devem vir de fonte secreta de ambiente e nunca do frontend ou de arquivo publico.

### Link publico do aluno

Regras:

- token aleatorio, longo e opaco;
- nunca derivado de IDs internos;
- persistencia apenas em forma de hash;
- regeneracao invalida imediatamente o token anterior;
- nenhuma informacao pessoal deve ser inferivel pela URL.

### OAuth state

Regras:

- assinado pelo backend;
- expiracao curta;
- uso unico;
- vinculado ao provedor e ao contexto correto do link;
- qualquer divergencia invalida o callback.

### Credenciais do atleta

Regras:

- `access_token` e `refresh_token` protegidos em repouso;
- refresh sempre persiste o token mais recente retornado pelo Strava;
- leitura de credencial centralizada em uma abstracao de seguranca, por exemplo `ISecretProtector`.

### Webhook

Como o Strava, na documentacao consultada, exige validacao por `hub.challenge` e `verify_token`, mas nao documenta assinatura criptografica no POST do evento, o endpoint deve ser protegido por combinacao de:

- path secreto;
- verificacao do `hub.verify_token` no GET de validacao;
- verificacao de `subscription_id` esperado;
- resolucao de `owner_id` apenas para conexoes ativas;
- logging e auditoria de payload recebido.

### Resposta rapida

O callback de POST do webhook deve:

- devolver `200 OK` em ate dois segundos;
- nao executar refresh, leitura detalhada de atividade nem escrita pesada em linha;
- persistir/enfileirar e sair.

## Regra de importacao de atividade

### Fonte de verdade do evento

O webhook do Strava nao traz detalhes completos da atividade. Ele informa o tipo de objeto, o aspecto do evento e os IDs relevantes. Por isso, para `activity:create`, a aplicacao deve sempre buscar a atividade detalhada pela API do Strava antes de criar a `SessaoDeTreino`.

### Traducao inicial para o modelo interno

Mapeamento inicial:

- data da atividade -> `Data`
- tipo ou `sport_type` -> tabela de mapeamento para `TipoDeTreino`
- `moving_time` como preferencia, com fallback para `elapsed_time` -> `DuracaoMinutos`
- `distance` em metros -> `DistanciaKm`
- origem -> `OrigemTreino.Strava`
- RPE -> valor fixo inicial `5`

### RPE de atividades importadas

O Strava nao oferece RPE nativo no fluxo considerado. Para nao bloquear a integracao, a versao inicial padroniza:

- `Rpe = 5`

Essa decisao e intencionalmente simples e documentada. A evolucao futura pode incluir:

- regra configuravel por professor;
- pos-processamento pelo professor;
- campos complementares de percepcao subjetiva.

Nada disso entra no escopo inicial.

### Mapeamento inicial de tipo

Tabela inicial sugerida:

- `Run` -> `Leve`
- `TrailRun` -> `Longo`
- `Ride` e `VirtualRide` -> `Leve`
- `Workout` -> `Ritmo`
- demais valores desconhecidos -> `Leve`

Essa tabela e apenas um ponto de partida tecnico para a primeira entrega. Ela deve ficar isolada no adaptador do Strava, nao espalhada pelo core.

### Idempotencia tecnica

Mesmo com a regra aprovada de importar automaticamente sem checar treinos existentes, a aplicacao precisa impedir duplicacao tecnica do mesmo evento por retry ou reprocessamento.

Regra:

- se `AtividadeImportada` ja existir para `Provedor + ExternalActivityId`, o evento e considerado processado e nenhuma nova `SessaoDeTreino` e criada.

Isso nao contradiz a decisao do produto. Isso apenas protege a plataforma contra retries legitimos do provedor.

## Tratamento de eventos do Strava

### activity:create

- buscar atividade detalhada;
- importar treino automaticamente;
- registrar `AtividadeImportada`.

### activity:update

Na primeira versao:

- registrar o evento;
- nao alterar a `SessaoDeTreino` ja importada automaticamente.

Justificativa:

- evita uma primeira versao com sincronizacao bidirecional parcial e potencialmente confusa;
- reduz risco de drift de regras quando o atleta edita titulo, tipo ou privacidade.

### activity:delete

Na primeira versao:

- registrar o evento;
- nao deletar automaticamente a `SessaoDeTreino` interna.

Justificativa:

- remocao automatica no core exige uma regra de negocio mais delicada;
- a prioridade atual e importacao confiavel.

### athlete update com `authorized=false`

- desativar localmente a conexao;
- marcar status como `Desconectado`;
- impedir novas tentativas de leitura com token antigo.

## Frontend alvo

### Area autenticada do professor

Dentro da tela do aluno em `/dashboard/alunos/:id`, deve existir um bloco novo `Integracoes`.

Capacidades:

- exibir provedores disponiveis;
- exibir status atual de cada provedor;
- mostrar acoes de copiar link e regenerar link;
- futuramente acomodar ultima sincronizacao e reconexao.

Essa area continua sob `authGuard` e consome apenas APIs privadas.

### Area publica do aluno

Rotas publicas previstas:

- `/conectar/:token`
- `/conectar/strava/retorno`

Principios:

- sem shell do professor;
- sem menu autenticado;
- sem dependencia do JWT do professor;
- layout proprio e enxuto;
- mensagens claras explicando que o aluno nao precisa login.

### Contrato de UI generico por provedor

O frontend nao deve hardcodar Strava como excecao estrutural.

Contrato sugerido por item de provedor:

- `providerKey`
- `displayName`
- `status`
- `enabled`
- `connectAction`

Assim, a pagina publica e a area do professor podem listar provedores dinamicamente e absorver Garmin/Polar depois sem duplicacao de tela.

### Dados visiveis ao aluno

A pagina publica deve exibir o minimo necessario:

- nome da plataforma;
- texto explicando a finalidade da conexao;
- lista de provedores disponiveis;
- sucesso ou falha do fluxo.

Nao deve exibir:

- email do aluno;
- ID interno;
- informacoes clinicas;
- identificadores do professor.

## Observabilidade

Desde a primeira versao, a implementacao deve gerar logs estruturados com:

- `provider`
- `athleteId`
- `connectionId`
- `externalOwnerId`
- `externalActivityId`
- `subscriptionId`
- `webhookEventId`

Tambem deve ser possivel observar:

- quantidade de eventos recebidos;
- quantidade de importacoes concluidas;
- quantidade de falhas;
- ultima sincronizacao por conexao.

## Estrategia de rollout

### Fase 1

- modelo de dominio e persistencia de links, conexoes e credenciais;
- endpoints privados do professor para link publico;
- pagina publica do aluno com listagem de provedores;
- implementacao de OAuth do Strava;
- registro da conexao e persistencia segura dos tokens.

### Fase 2

- webhook do Strava com validacao;
- processamento assincrono;
- importacao automatica de `activity:create`;
- rastreamento de atividade importada;
- atualizacao de status e ultima sincronizacao.

### Fase 3

- refinamentos de status na UI do professor;
- observabilidade adicional;
- preparacao operacional para novos provedores.

## Escopo inicial

### Incluido

- link publico reutilizavel por atleta;
- regeneracao do link pelo professor;
- pagina publica no mesmo frontend Angular;
- primeiro provedor `Strava`;
- OAuth completo com callback seguro;
- armazenamento protegido de credenciais;
- webhook do Strava;
- importacao automatica de novas atividades;
- rastreamento tecnico de atividades importadas;
- suporte arquitetural para Garmin/Polar no futuro.

### Fora de escopo

- sincronizacao retroativa de historico antigo do Strava;
- importacao de atividades `Only You`;
- sincronizacao bidirecional;
- atualizacao automatica de treino interno em `activity:update`;
- remocao automatica de treino interno em `activity:delete`;
- interface de administracao de webhook fora da area do professor;
- implementacao concreta de Garmin ou Polar nesta fase.

## Validacao esperada

### Validacao funcional

- o professor consegue copiar um link opaco por atleta;
- o aluno consegue conectar o Strava sem login;
- a conexao fica registrada no atleta correto;
- uma nova atividade do Strava gera uma `SessaoDeTreino` automaticamente;
- retries do webhook nao criam sessoes duplicadas.

### Validacao de seguranca

- nenhum identificador do aluno e exposto na URL;
- `client_secret` nao sai do backend;
- `state` invalido falha com seguranca;
- credenciais do atleta estao protegidas em repouso;
- regenerar o link invalida o token anterior.

### Validacao arquitetural

- Strava fica isolado em modulo proprio;
- o fluxo do produto usa contratos genericos por provedor;
- adicionar Garmin/Polar exige nova implementacao de provedor, nao reescrever o fluxo inteiro;
- o dominio de treino continua sendo a representacao canonica das atividades no produto.

## Resultado esperado

Ao final desta frente, o CoachTraining passa a ter um fluxo publico seguro e reutilizavel para conectar alunos a wearables, com Strava como primeira integracao efetiva, importacao automatica de atividades e uma base arquitetural limpa para a entrada futura de Garmin e Polar sem retrabalho estrutural.
