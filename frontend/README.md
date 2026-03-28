# CoachtrainingWeb

Frontend Angular da area do professor no projeto CoachTraining.

## Requisitos

- Node.js 22+
- npm 10+

## Setup rapido

```bash
npm install
npm start
```

Por padrao, o app abre em `http://localhost:4200`.

## Integracao local com API

O script `npm start` usa `proxy.conf.json` para rotear chamadas ao backend local:

- `/professores` -> `http://localhost:5096`
- `/auth` -> `http://localhost:5096`
- `/api` -> `http://localhost:5096`

## Rotas implementadas

- `/`: landing + cadastro de professor
- `/login`: autenticacao de professor
- `/dashboard`: area protegida com layout autenticado

## Scripts

```bash
npm start      # ng serve com proxy
npm run build  # build de producao
npm test       # testes unitarios
```

## Documentacao complementar

- `../docs/frontend/estrutura.md`
- `../docs/frontend/autenticacao.md`
- `../docs/frontend/integracao-api.md`
