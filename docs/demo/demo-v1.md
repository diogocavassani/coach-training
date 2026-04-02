# Demo Dataset `demo-v1`

Dataset de demonstração reproduzível para apresentação do Coach Training.

## Como gerar

### Pré-requisitos

- Docker e Docker Compose instalados
- .NET 10.0+ instalado
- PostgreSQL rodando (via Docker Compose)

### Passos

1. Inicie o PostgreSQL (se ainda não estiver rodando):
```bash
cd infra
docker compose up -d db
cd ..
```

2. Execute o seeder:
```bash
dotnet run --project src/CoachTraining.DemoSeed -- --profile demo-v1 --reset-demo
```

3. A saída será algo como:
```
╔══════════════════════════════════════════════════════════════════════════════╗
║                    DEMO DATASET SEED - RELATÓRIO FINAL                        ║
╚══════════════════════════════════════════════════════════════════════════════╝

📋 Perfil: demo-v1

👨‍🎓 PROFESSOR DEMO
   Email: demo.professor@coachtraining.local
   Senha: Demo@123456

🏃 ATLETAS E INSIGHTS

1. Ana Souza
   Email: ana.souza@demo.local
   Insight: Sem alerta crítico - carga controlada e consistente
   ...
```

## Credenciais

- **Professor**: `demo.professor@coachtraining.local`
- **Senha do Professor**: `Demo@123456`
- **Acesso**: http://localhost:4200

## Cenários

### 1. Ana Souza - Base Estável
- **Email**: ana.souza@demo.local
- **Cenário**: Atleta com carga controlada e consistente
- **Insight Esperado**: Sem alerta crítico - base bem estabelecida
- **Uso**: Demonstrar dashboard limpo sem alertas

### 2. Bruno Lima - Construção Saudável
- **Email**: bruno.lima@demo.local
- **Cenário**: Progressão gradual e consistente de carga
- **Insight Esperado**: Evolução controlada - progressão coerente
- **Uso**: Mostrar evolução positiva e planejamento bem-executado

### 3. Carla Mendes - Risco por Carga Abrupta
- **Email**: carla.mendes@demo.local
- **Cenário**: Aumento abrupto de carga nas últimas semanas
- **Insight Esperado**: ALERTA - Delta semanal elevado e ACWR alto
- **Uso**: Demonstrar sistema de alertas funcionando

### 4. Diego Alves - Taper Bem Executado
- **Email**: diego.alves@demo.local
- **Cenário**: Preparação para prova em 10 dias com redução adequada de volume
- **Prova**: Meia Maratona (21.1 km) em 10 dias
- **Insight Esperado**: Taper adequado - redução de volume 40-50% pré-prova
- **Uso**: Mostrar gerenciamento de prova-alvo

### 5. Fernanda Rocha - Aderência Baixa
- **Email**: fernanda.rocha@demo.local
- **Cenário**: Planejamento 5 treinos/semana, execução recente de 2/semana
- **Insight Esperado**: Baixa aderência - planejado vs realizado
- **Uso**: Demonstrar tracking de aderência

### 6. Gustavo Nunes - Divergência Carga x Rendimento
- **Email**: gustavo.nunes@demo.local
- **Cenário**: Carga aumenta mas o pace médio piora nas últimas semanas
- **Insight Esperado**: Divergência - carga sobe mas rendimento piora
- **Uso**: Mostrar detecção de fadiga e sinais de overtraining

## Uso na Apresentação

### Fluxo Sugerido

1. **Login**: Autenticar como professor demo
2. **Dashboard Resumo**: Mostrar visão geral dos 6 atletas
3. **Atletas Individuais**: 
   - Começar com Ana Souza (baseline)
   - Mostrar Bruno Lima (progressão positiva)
   - Demonstrar alertas com Carla Mendes
   - Destacar Taper com Diego Alves
   - Discutir Aderência com Fernanda
   - Conversar sobre Overtraining com Gustavo

### Checklist Pré-Demo

- [ ] PostgreSQL rodando via Docker
- [ ] DemoSeed executado com sucesso
- [ ] API iniciada em http://localhost:4200 (ou porta configurada)
- [ ] Frontend acessível
- [ ] Vérificar se todos os 6 atletas aparecem no dashboard
- [ ] Uma prova visível para Diego Alves

## Reset e Limpeza

### Reset apenas do dataset demo
```bash
dotnet run --project src/CoachTraining.DemoSeed -- --profile demo-v1 --reset-demo
```

### Reset completo (remove TODOS os dados)
```bash
dotnet run --project src/CoachTraining.DemoSeed -- --profile demo-v1 --reset-all
```

### Help
```bash
dotnet run --project src/CoachTraining.DemoSeed -- --help
```

## Notas Técnicas

- Os dados são determinísticos baseados na data de execução
- Todas as sessões são renderizadas com a data de hoje como referência
- O dataset é idempotente - executar múltiplas vezes com `--reset-demo` não cria duplicatas
- Histórico inclui 10-12 semanas de dados para demonstração adequada dos gráficos

## Troubleshooting

### Erro: ConnectionStrings:DefaultConnection não configurada
- Verifique `appsettings.Development.json` tem a connection string do PostgreSQL

### Erro: Cannot connect to database
- Verifique se PostgreSQL está rodando: `docker compose ps`
- Inicie se necessário: `docker compose up -d db`

### Nenhum atleta aparece no dashboard
- Re-execute o seeder com `--reset-demo` flag
- Verifique se a API reiniciou após o seed
