---
agent: agent
---

# Prompt CoachTraining - Sistema de Monitoramento de Carga e Preparação de Atletas

## 🎯 Contexto do Projeto

Este é um **sistema profissional de apoio à decisão para treinadores de corrida**, baseado em evidências científicas de fisiologia do exercício. O objetivo é auxiliar no monitoramento de carga de treino, identificação de fases de treinamento e geração de insights para tomada de decisão.

**O sistema NÃO prescreve treinos** — ele consolida dados, aplica regras científicas e apresenta indicadores ao professor.

---

## 📊 Stack Tecnológico

- **Backend**: .NET 10.0 (C#)
- **Arquitetura**: Clean Architecture + DDD (Domain-Driven Design)
- **Padrões**: Domain Services, Value Objects, Entities, Aggregates
- **Banco de Dados**: SQL Server (Infrastructure Layer)
- **API**: REST API (Controllers)
- **Testes**: XUnit para testes unitários

---

## 🏛️ Princípios Arquiteturais

1. **Clean Architecture**: Separação clara de camadas (Domain, Application, Infrastructure, API)
2. **Domain-Driven Design**: Lógica de negócio concentrada no Domain Layer
3. **Explicabilidade**: Todas as regras de cálculo devem ser testáveis e compreensíveis
4. **Simplicidade no MVP**: Focar no essencial, evolução incremental
5. **Baseado em Evidência**: Decisões apoiadas em literatura científica

---

## 🔬 Fundamentos Científicos Utilizados

### Métricas Principais
- **Session-RPE**: Duração (min) × RPE (escala 1-10) = Carga do treino
- **Carga Semanal**: Soma das cargas dos treinos da semana
- **Carga Crônica**: Média da carga semanal das últimas 4 semanas
- **Carga Aguda**: Carga da semana atual (últimos 7 dias)
- **ACWR**: Razão entre carga aguda e carga crônica

### Interpretação de ACWR
- **< 0,8**: Carga baixa / risco de destreinamento
- **0,8 – 1,3**: Zona de adaptação segura ✓
- **≥ 1,5**: Alerta de risco aumentado ⚠️

### Fases do Treinamento
- **Base**: Carga estável ou abaixo da média
- **Construção**: Carga crescente controlada
- **Pico**: Carga elevada e sustentada
- **Polimento (Taper)**: Redução de 40–60% de volume, 7–21 dias antes da prova

---

## 📈 Regras de Negócio Implementadas

1. **Cálculo de Carga**: `Carga = Duração × RPE`
2. **Progressão Semanal**: Aumento recomendado até 10–15%; acima de 20% gera alerta
3. **Detecção de Overreaching**: ACWR ≥ 1,5 por mais de 1 semana
4. **Identificação de Taper**: Redução de carga nos últimos 14 dias antes da prova
5. **Validação de Planejamento**: Treinos realizados vs. treinos planejados

---

## 📅 Roadmap de 10 Semanas

### **Semana 1**: Setup, Visão e Arquitetura
- Estruturação de camadas (.Domain, .Application, .Infra, .API)
- Configuração de injeção de dependência
- Endpoint de health-check
- README e documentação arquitetural

### **Semana 2**: Modelagem de Domínio
- Entidades: Coach, Atleta, ProvaAlvo, SessaoDeTreino
- Value Objects: RPE, CargaTreino, Pace
- Enums: TipoDeTreino, FaseDoCiclo, StatusDeRisco
- Testes unitários de entidades

### **Semana 3**: Cálculo de Carga de Treino
- Domain Service `CalculadoraDeCarga`
- Agregação de carga diária, semanal, crônica
- Testes com cenários reais

### **Semana 4**: ACWR e Progressão de Carga
- Domain Service `AvaliadorDeRisco`
- Cálculo de carga aguda, ACWR, delta percentual
- Testes cobrindo limiares científicos

### **Semana 5**: Fases do Treinamento e Taper
- Domain Service `ClassificadorDeFase`
- Detecção de tendência, classificação de fases
- Validação de taper pré-prova

### **Semana 6**: Read Model e Dashboard
- DTO de dashboard
- Queries de consolidação de métricas
- Endpoint `GET /dashboard/atleta/{id}`

### **Semana 7**: Geração de Insights
- Mapear alertas para mensagens textuais
- Priorização por criticidade
- Linguagem técnica contextualizada

### **Semana 8**: Testes de Cenários Reais
- Cenários: iniciante, intermediário, avançado
- Cenários de overreaching e taper bem executado
- Ajuste de limiares conforme resultados

### **Semana 9**: Refinamento e Robustez
- Revisão de mensagens e código
- Otimização de performance
- Tratamento de dados inconsistentes

### **Semana 10**: Documentação e Fechamento
- Documentar regras científicas utilizadas
- Documentar limitações e evoluções futuras
- Overview do projeto para portfólio

---

## 👥 Entidades Principais

- **Coach**: Professor/treinador, identifica-se no sistema
- **Atleta**: Monitorado pelo coach (não é usuário do MVP)
- **ProvaAlvo**: Meta do ciclo de treino (data, distância, objetivo)
- **SessaoDeTreino**: Registro de um treino realizado (data, tipo, duração, RPE)

---

## 📊 Dados Coletados

| Categoria | Dados |
|-----------|-------|
| **Atleta** | Nome, observações clínicas, nível esportivo |
| **Prova Alvo** | Data, distância, objetivo |
| **Planejamento** | Treinos semanais planejados |
| **Sessão** | Data, tipo, duração, distância, RPE |

---

## 💡 Insights Gerados

- Situação atual do atleta (fase do ciclo)
- ACWR atual e tendência
- Aumento abrupto de carga
- Monotonia elevada (carga concentrada)
- Divergência entre carga e rendimento
- Aderência ao planejamento

---

## ✅ Critérios de Sucesso

- **Código limpo**: Seguir princípios SOLID e DDD
- **Testabilidade**: Regras científicas cobertas por testes
- **Documentação**: Cada decisão arquitetural explicada
- **Explicabilidade**: Regras baseadas em literatura
- **Performance**: Queries otimizadas para múltiplos atletas
- **Robustez**: Tratamento de dados inconsistentes

---

## 🚫 Fora do MVP

- Prescrição automática de treino
- Comunicação direta com atleta
- Integração com Garmin/Strava/Apple Health
- Machine Learning
- Multi-modalidade esportiva
- App mobile

---

## 🎓 Como Usar Este Prompt

Quando precisar de ajuda na implementação:
1. **Cite a semana** em que está trabalhando
2. **Mencione a entidade ou serviço** em questão
3. **Forneça contexto** sobre o desafio específico
4. **Referencie as regras científicas** quando aplicável

**Exemplo**: "Estou na Semana 4. Preciso implementar o `AvaliadorDeRisco`. Como estruturar os testes para o limiar de ACWR ≥ 1,5?"