# Design - Executive Signal para o Frontend

## Contexto

O frontend atual do CoachTraining ja possui uma base funcional consistente, com tema escuro, estrutura de navegacao clara e telas principais do MVP entregues. O problema nao e falta de consistencia tecnica. O problema e posicionamento visual e percepcao de produto.

Hoje o sistema ainda transmite mais "ferramenta interna funcional" do que "software premium para treinadores". A landing tem boa estrutura, mas ainda lembra uma pagina de produto generica. A area logada e consistente, mas ainda muito baseada em blocos equivalentes, pouca hierarquia editorial e uma linguagem visual mais operacional do que premium.

O objetivo desta frente e modernizar o frontend com um reposicionamento visual completo, comecando pela landing page e depois descendo esse sistema para login, shell e telas principais da area autenticada.

## Objetivo

Criar uma linguagem visual chamada **Executive Signal**, com foco em:

- posicionamento `performance tech premium`;
- base clara e sofisticada;
- azul-petroleo como cor principal;
- verde usado de forma sutil como acento funcional;
- melhor hierarquia visual;
- melhores padroes de usabilidade para fluxos operacionais;
- coerencia entre marketing, autenticacao e produto.

## Direcao escolhida

Durante o alinhamento, a direcao aprovada foi:

- linguagem visual `Executive Signal`;
- tom premium, executivo, confiavel e tecnologico;
- base clara em vez de escura;
- hero da landing com mistura de fotografia e prova visual do produto;
- CTA principal de cadastro com CTA secundaria mais discreta;
- foco em produto + contexto + decisao, sem cair em "fitness generico" nem em "marketing vazio".

## Abordagens consideradas

### 1. Executive Signal

Base clara, sofisticada, com leitura de produto de decisao. Mistura atmosfera humana e interface real do sistema, com layout mais editorial e menos dependencia de cards.

**Vantagens**
- melhor equilibrio entre premium e usabilidade;
- conversa bem com treinadores e com o posicionamento do produto;
- diferencia do visual comum de apps fitness;
- desce bem da landing para a area logada.

**Desvantagens**
- exige atualizar tokens globais e varias telas para manter coerencia.

### 2. Data Atelier

Direcao ainda mais minimalista e editorial, com menos energia visual e mais cara de relatorio executivo.

**Vantagens**
- muito sofisticada;
- alta sensacao de confianca.

**Desvantagens**
- risco de ficar fria demais;
- menor impacto comercial na landing.

### 3. Athlete Intelligence

Direcao mais vibrante, com mais camadas visuais, mais motion e mais cara de produto tech em "launch mode".

**Vantagens**
- chama atencao rapido;
- mais impacto de primeiro olhar.

**Desvantagens**
- maior risco de parecer startup fitness;
- mais distancia do tom executivo aprovado.

## Recomendacao adotada

Adotar **Executive Signal** como sistema visual do frontend inteiro.

## Sistema visual

### Paleta

Nova estrutura cromatica:

- `azul-petroleo` como base de marca e principal acento estrutural;
- `off-white`, `mist`, `slate-light` para fundos e superficies;
- `verde suave` como cor de apoio para sinais positivos e destaque funcional;
- `ambar` para atencao;
- `vermelho controlado` para risco e erro.

Intencao:

- o azul-petroleo posiciona o produto como tech premium;
- o verde deixa de ser dominante e vira sinal de performance;
- o produto ganha sofisticacao sem perder leitura funcional.

### Tipografia

Usar uma dupla mais refinada:

- `Manrope` para titulos;
- `Public Sans` para interface, labels e conteudo.

Objetivo:

- dar mais presenca de marca na landing;
- manter legibilidade e neutralidade nas telas operacionais.

### Superficies

A interface passa a usar:

- menos cards por padrao;
- mais secoes abertas, blocos leves e faixas;
- bordas suaves e contrastes discretos;
- sombras limpas e menos "glass dark dashboard".

Regra:

- usar card apenas quando ele realmente ajuda a separar contexto, prioridade ou acao.

### Motion

Animacoes curtas e discretas:

- entrada suave no hero da landing;
- transicoes leves em hover e navegacao;
- revelacao de secoes com pouca distancia;
- sem excesso de microinteracoes.

Objetivo:

- parecer polido, nao chamativo.

## Landing page

### Estrutura alvo

A landing sera reorganizada em quatro atos:

#### 1. Hero com prova

Primeira dobra clara, premium e orientada a valor:

- headline dominante;
- subtexto curto e especifico;
- CTA principal de cadastro;
- CTA secundaria com contraste menor;
- fotografia real de contexto esportivo;
- mock do produto com dados de verdade como prova visual.

Essa secao deve comunicar:

- o produto existe;
- ele organiza a operacao do treinador;
- ele ajuda a decidir com mais confianca.

#### 2. Sinais que importam

Secao curta explicando os diferenciais do sistema com linguagem de produto:

- risco;
- taper;
- aderencia;
- historico de 12 semanas;
- priorizacao do professor.

Sem grade de cards pesada. A linguagem deve ser mais editorial e mais premium.

#### 3. Fluxo do treinador

Secao mostrando o uso real:

1. cadastra atleta;
2. registra treino;
3. acompanha sinais;
4. toma decisao.

Essa secao melhora clareza e credibilidade porque mostra o produto em uso, nao apenas promessas.

#### 4. Conversao final

O formulario de cadastro continua na landing, mas com melhor contexto:

- fechamento coerente da pagina;
- reforco de valor;
- menos cara de formulario "isolado".

### Ajustes finos aprovados para o hero

Os seguintes refinamentos devem entrar na implementacao:

- headline mais especifica e menos generica;
- CTA secundaria mais discreta;
- mock do produto com mais presenca de sistema real;
- blocos de apoio abaixo do hero com menor peso visual.

## Login

O login deve ser alinhado ao novo sistema, mas com foco total em clareza e rapidez:

- layout claro;
- estrutura mais editorial;
- menos sensacao de modal escuro centralizado;
- melhor hierarquia entre titulo, descricao, formulario e link de retorno/cadastro.

O login deve parecer parte do mesmo produto da landing, e nao uma tela herdada de um tema diferente.

## Area logada

### App shell

O shell deve mudar de um layout escuro e pesado para um app claro e refinado:

- header mais leve;
- sidebar mais elegante e estavel;
- melhor contraste entre navegacao, conteudo e acoes;
- sensacao de workspace premium.

### Dashboard do professor

A home autenticada deve priorizar leitura e acao:

- KPIs mais escaneaveis;
- destaque mais forte para prioridades reais;
- menos competicao entre blocos do mesmo peso;
- organizacao por importancia operacional.

O objetivo nao e so "ficar bonito", mas deixar mais obvio quem precisa de atencao e qual acao tomar.

### Dashboard do atleta

O dashboard individual deve ganhar:

- melhor agrupamento de metricas principais;
- maior destaque para insights;
- relacao mais clara entre planejamento, aderencia e prova-alvo;
- graficos mais integrados ao layout;
- formularios embutidos com mais hierarquia.

### Alunos, novo aluno e novo treino

As telas operacionais devem sair do aspecto de formulario cru:

- headings mais orientados a tarefa;
- agrupamento semantico de campos;
- melhor espacamento;
- estados vazios mais orientadores;
- tabelas e formularios mais leves visualmente.

## Padroes de usabilidade

A modernizacao deve seguir estes principios:

- uma acao principal por tela;
- acoes secundarias com menor peso;
- headings orientados a tarefa;
- melhor escaneabilidade de KPIs, insights e tabelas;
- estados vazios com proximo passo claro;
- consistencia de tom entre landing, login e area logada;
- responsividade real em mobile e desktop;
- contraste suficiente e hierarquia visual forte.

## Escopo de implementacao

### Incluido

- tokens globais de cor, tipografia, radius, sombras e espacamento;
- landing page;
- login;
- app shell;
- dashboard do professor;
- dashboard do atleta;
- lista de alunos;
- cadastro de aluno;
- cadastro de treino;
- ajustes de testes do frontend afetados pela mudanca estrutural ou visual;
- documentacao de design e direcao visual.

### Fora de escopo

- rebranding de nome ou logo;
- reestruturacao funcional do backend;
- criacao de novas features alem das ja existentes;
- refactor grande de logica de negocio do frontend sem necessidade de UI.

## Estrategia de rollout

A modernizacao deve seguir esta ordem:

1. atualizar sistema visual global;
2. refazer landing;
3. alinhar login;
4. modernizar shell;
5. modernizar dashboard do professor;
6. modernizar dashboard do atleta;
7. ajustar alunos e formularios operacionais.

Essa ordem garante que a linguagem nasca na vitrine, consolide na autenticacao e depois entre na area interna sem parecer dois produtos diferentes.

## Validacao esperada

### Validacao visual

- a landing precisa parecer premium, clara e confiavel;
- o produto precisa parecer real, nao conceitual;
- a area logada precisa parecer parte da mesma marca;
- o sistema nao deve cair em visual fitness generico ou em dashboard escuro padrao.

### Validacao funcional

- fluxos existentes continuam funcionando;
- rotas e formularios nao quebram;
- responsividade continua adequada;
- testes do frontend continuam passando.

### Validacao subjetiva de qualidade

Perguntas que a implementacao final deve responder positivamente:

- parece um software premium para treinador?
- a home publica vende o produto sem exagero?
- a area autenticada melhora decisao e leitura?
- landing, login e dashboard parecem o mesmo produto?

## Resultado esperado

Ao final, o CoachTraining deve aparentar um produto mais maduro, premium e confiavel, com melhor narrativa visual e melhor usabilidade operacional, sem perder a objetividade tecnica que sustenta o valor da plataforma.
