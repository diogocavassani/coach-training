# Data Model - Entidades

## Professores
- `id` (PK)
- `nome`
- `email` (unique)
- `senha_hash`
- `data_criacao`

## Atletas
- `id` (PK)
- `professor_id` (FK -> professores.id)
- `nome`
- `email`
- `observacoes_clinicas`
- `nivel_esportivo`

## Sessoes de Treino
- `id` (PK)
- `atleta_id` (FK -> atletas.id)
- `data`
- `tipo`
- `duracao_minutos`
- `distancia_km`
- `rpe`

## Provas Alvo
- `id` (PK)
- `atleta_id` (FK -> atletas.id, unique)
- `data_prova`
- `distancia_km`
- `objetivo`
