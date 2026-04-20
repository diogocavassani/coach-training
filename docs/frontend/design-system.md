# Design System Frontend (Padrao Oficial)

Este documento define o contrato visual oficial do frontend CoachTraining a partir da direcao `Executive Signal`.
Todas as novas telas devem partir desta base antes de criar variacoes locais.

## Direcao principal

- Tema principal: light premium
- Cor estrutural: azul-petroleo
- Cor de apoio: verde funcional
- Layout: editorial, claro, com foco em leitura de prioridade e decisao

Direcao de produto: menos campanha fitness, mais workspace premium orientado por contexto, leitura e decisao.

## Paleta estrutural

### Azul-petroleo

- `Primary 50`: `#EEF6F8`
- `Primary 100`: `#D9E8EE`
- `Primary 200`: `#B7D2DC`
- `Primary 300`: `#8CB5C5`
- `Primary 400`: `#5589A0`
- `Primary 500`: `#163D57`
- `Primary 600`: `#123247`
- `Primary 700`: `#0F2939`
- `Primary 800`: `#0D2230`
- `Primary 900`: `#0A1A24`

Uso recomendado:

- `500`: estrutura principal, CTA primario, titulos de maior autoridade
- `600` e `700`: hover, estados ativos e contraste
- `50` a `200`: fundos, detalhes editoriais e suporte de leitura

### Verde funcional

- `Support 50`: `#EDF6F1`
- `Support 100`: `#D7EADF`
- `Support 200`: `#B5D8C2`
- `Support 300`: `#8EC2A3`
- `Support 400`: `#68AA84`
- `Support 500`: `#4F8F6E`
- `Support 600`: `#3F7459`
- `Support 700`: `#335D48`

Uso recomendado:

- apoio visual sutil
- estados positivos e sinais de estabilidade
- nunca competir com o azul-petroleo como cor dominante da tela

## Cores semanticas

- Background principal: `#F4F1EA`
- Background alternativo: `#EDF2F3`
- Background de apoio: `#D8E3E6`
- Surface: `rgba(255, 255, 255, 0.74)`
- Surface forte: `#FFFDF9`
- Border: `rgba(22, 61, 87, 0.14)`
- Border forte: `rgba(22, 61, 87, 0.24)`
- Texto principal: `#173042`
- Texto secundario: `#5F7481`
- Texto suave: `#738793`
- Success: `#3F8B62`
- Warning: `#B98021`
- Error: `#B84A4A`
- Info: `#27689A`

## Tipografia

- Display font oficial: `Manrope`
- Base font oficial: `Public Sans`
- Heading XL: `clamp(3rem, 6vw, 5.2rem)` com peso `800`
- Heading L: `clamp(2rem, 4vw, 3rem)` com peso `700`
- Heading M: `clamp(1.35rem, 2vw, 1.75rem)` com peso `600`
- Body: `16 / 400`
- Caption: `14 / 400`
- Label: `14 / 700`, uppercase e tracking expandido para orientacao

Regra de uso:

- headings e momentos de autoridade usam `Manrope`
- leitura corrida, formularios e copy operacional usam `Public Sans`

## Espacamento, radius e sombra

Escala oficial:

- `4`
- `8`
- `12`
- `16`
- `24`
- `32`
- `48`
- `64`

Radius:

- `sm`: `10px`
- `md`: `16px`
- `lg`: `24px`
- `xl`: `32px`

Sombras:

- `shadow-soft`: elevacao discreta para superfices de apoio
- `shadow-elevated`: elevacao premium para blocos que precisam destaque real

## Primitivos compartilhados

- `.page-surface`: usar somente quando uma superficie melhora leitura, agrupamento ou hierarquia
- `.section-label`: etiqueta editorial curta para orientar blocos, nunca para competir com o heading

## Componentes base

### Botoes

- CTA primario:
  - fundo `Primary 500`
  - hover `Primary 600`
  - texto claro
  - uso: uma unica acao dominante por tela
- Acoes secundarias:
  - peso visual menor
  - preferir `mat-button` ou outlined leve
  - nao disputar atencao com o CTA principal

### Inputs

- Background: `Surface forte`
- Border: `Border forte`
- Focus border: `Primary 500`
- Radius: `14px` no contexto Angular Material atual
- Height minima: `48px`
- Text: `Texto principal`
- Placeholder e label: `Texto secundario`
- Erro:
  - border `Error`
  - helper text em tom derivado de erro

## Governanca de estilo

- Evitar cards por padrao.
- Usar superfices apenas quando elas realmente ajudam leitura, agrupamento ou acao.
- Cada tela deve ter um unico CTA primario dominante.
- Acoes secundarias devem ter menor peso visual que a principal.
- Layouts devem priorizar hierarquia editorial, respiro e leitura rapida de prioridade.
- Nao criar paleta paralela por componente sem justificativa tecnica e registro neste documento.

## Superficies operacionais

- Dashboards usam superfices para separar prioridade, nao para repetir mosaicos de cards equivalentes.
- Formularios usam secoes por intencao: identidade, contexto, sessao e esforco.
- Tabelas devem priorizar leitura e acao, com cabecalhos discretos e estados vazios orientadores.

## Landing e autenticacao

- Landing usa hero com prova de produto e contexto editorial.
- Login deve parecer extensao direta da landing, com a mesma paleta e hierarquia.

## Tokens CSS oficiais

Os tokens globais estao em `frontend/src/styles.css`.
Qualquer extensao de tema, tipografia, sombra ou primitive compartilhado deve ser registrada la e refletida aqui.
