# Data Model - Diagramas

```mermaid
erDiagram
    PROFESSORES ||--o{ ATLETAS : possui
    ATLETAS ||--o{ SESSOES_TREINO : registra
    ATLETAS ||--o| PROVAS_ALVO : define

    PROFESSORES {
        uuid id PK
        string nome
        string email UK
        string senha_hash
        datetime data_criacao
    }

    ATLETAS {
        uuid id PK
        uuid professor_id FK
        string nome
        string email
        string observacoes_clinicas
        string nivel_esportivo
    }

    SESSOES_TREINO {
        uuid id PK
        uuid atleta_id FK
        date data
        int tipo
        int duracao_minutos
        double distancia_km
        int rpe
    }

    PROVAS_ALVO {
        uuid id PK
        uuid atleta_id FK_UQ
        date data_prova
        double distancia_km
        string objetivo
    }
```
