# Design System Frontend (Padrao Oficial)

Este documento define o padrao visual oficial do CoachTraining frontend.
Todas as novas telas e evolucoes devem seguir estas definicoes.

## Objetivo visual

A interface deve comunicar:

- evolucao
- saude
- progresso
- meta atingida
- performance

Direcao de produto: menos "site institucional", mais "ferramenta premium de trabalho".

## Tema base

- Tema principal: dark
- Cor principal de acao: verde
- Layout: limpo, com foco em leitura de KPI e decisao rapida

## Paleta principal (verde)

- Green 50: `#F0FDF4`
- Green 100: `#DCFCE7`
- Green 200: `#BBF7D0`
- Green 300: `#86EFAC`
- Green 400: `#4ADE80`
- Green 500: `#22C55E`
- Green 600: `#16A34A`
- Green 700: `#15803D`
- Green 800: `#166534`
- Green 900: `#14532D`

Uso recomendado:

- `500`: acao principal
- `600`: hover
- `700`: estado ativo
- `100` e `50`: fundos suaves
- `800` e `900`: contraste em contextos escuros

## Cores semanticas

- Success: `#22C55E`
- Warning: `#F59E0B`
- Error: `#EF4444`
- Info: `#3B82F6`

## Fundo e superfices (dark)

- Background Primary: `#0B1220`
- Background Secondary: `#111827`
- Surface Card: `#1F2937`
- Border: `#374151`
- Texto principal: `#F9FAFB`
- Texto secundario: `#9CA3AF`

## Tipografia

- Fonte oficial: `Inter`
- Heading XL: `48 / 700` (responsivo com `clamp`)
- Heading L: `32 / 700` (responsivo com `clamp`)
- Heading M: `24 / 600`
- Body: `16 / 400`
- Caption: `14 / 400`
- Label: `14 / 500`

## Escala de espacamento

Use somente a escala:

- `4`
- `8`
- `12`
- `16`
- `24`
- `32`
- `48`
- `64`

## Componentes base

### Botoes

- Primary:
  - background `Green 500`
  - hover `Green 600`
  - texto branco
  - radius `12px`
  - altura minima `48px`
  - padding horizontal `16-24px`
- Secondary:
  - fundo transparente
  - borda `1px Green 500`
  - texto `Green 300/400`

### Inputs

- Background: `#111827`
- Border: `#374151`
- Focus border: `Green 500`
- Radius: `10px`
- Height: `48px`
- Padding: `12px 16px`
- Text: `#F9FAFB`
- Placeholder: `#9CA3AF`
- Erro:
  - border `#EF4444`
  - helper text `#F87171`

### Cards

- Background: `#1F2937`
- Border: `1px solid #374151`
- Radius: `16px`
- Padding: `24px`
- Hover: borda em `Green 500`

## Dashboard (proximas sprints)

Blocos de KPI devem priorizar:

- alunos ativos
- treinos pendentes
- evolucao semanal
- taxa de conclusao

Padrao de card KPI:

- titulo pequeno
- numero grande
- indicador de variacao em verde (quando positivo)

## Tokens CSS oficiais

Os tokens globais estao em:

- `frontend/src/styles.css`

Qualquer nova variavel de tema deve ser adicionada la.
Nao criar paleta paralela por componente sem justificativa tecnica.

## Governanca de estilo

- Este documento e o contrato visual vigente.
- Mudancas de paleta, tipografia, espacamento e comportamento base devem ser registradas neste arquivo.
- Pull requests com alteracoes visuais relevantes devem citar este documento e indicar se houve extensao de tokens.
