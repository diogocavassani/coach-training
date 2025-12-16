# Visão Geral do Projeto

Este projeto tem como objetivo criar um **sistema de apoio à decisão para professores/treinadores de corrida**, baseado em evidências científicas da fisiologia do exercício, biomecânica e treinamento esportivo.

O sistema **não prescreve treinos** e **não substitui o treinador**. Ele consolida dados de treino e aplica regras baseadas em literatura científica para gerar **indicadores, alertas e insights**, auxiliando o professor no acompanhamento da preparação do atleta para uma **prova alvo**.

---

## 1. Público-alvo

- Professores / treinadores de corrida
- Assessoria esportiva
- Profissionais de educação física

O atleta **não é usuário do sistema** no MVP. Ele é uma entidade monitorada.

---

## 2. Problema que o projeto resolve

Treinadores acompanham múltiplos atletas e precisam analisar:
- Evolução de carga
- Risco de excesso de treino
- Momento fisiológico do atleta (base, construção, pico, polimento)
- Aderência ao planejamento

Na prática, esses cálculos são feitos de forma manual, intuitiva ou com planilhas pouco padronizadas. O sistema centraliza, padroniza e fundamenta essas análises.

---

## 3. Escopo do MVP

### 3.1 Funcionalidades incluídas

- Cadastro de atletas
- Cadastro de prova alvo por atleta
- Configuração de planejamento base (treinos por semana)
- Registro de sessões de treino
- Cálculo automático de carga e indicadores
- Dashboard de acompanhamento por atleta
- Alertas baseados em regras científicas

### 3.2 Funcionalidades fora do MVP

- Prescrição automática de treino
- Comunicação direta com o atleta
- Integração com Garmin / Strava / Apple Health
- Machine Learning
- Multi-modalidade esportiva
- App mobile

---

## 4. Dados coletados no MVP

### 4.1 Por atleta
- Nome
- Observações clínicas (texto livre)
- Nível esportivo (opcional)

### 4.2 Prova alvo
- Data da prova
- Distância
- Objetivo (texto livre)

### 4.3 Planejamento
- Quantidade de treinos semanais planejados

### 4.4 Sessão de treino
- Data
- Tipo de treino (leve, ritmo, intervalado, longo)
- Duração (minutos)
- Distância (km)
- RPE da sessão (1–10)

---

## 5. Fundamentos científicos utilizados

O sistema se baseia principalmente em:
- **Carga interna de treino** (session-RPE)
- **Modelo impulso–resposta (fitness vs fadiga)**
- **Carga aguda vs carga crônica (ACWR)**
- **Princípios de progressão e sobrecarga**
- **Estratégias de taper (polimento pré-prova)**

As métricas são usadas como **indicadores de apoio**, não como verdades absolutas.

---

## 6. Regras de negócio principais

### 6.1 Cálculo de carga do treino

Carga do treino = Duração (min) × RPE

---

### 6.2 Carga semanal

Carga semanal = Soma das cargas dos treinos da semana

---

### 6.3 Carga crônica

Carga crônica = Média da carga semanal das últimas 4 semanas

---

### 6.4 Carga aguda

Carga aguda = Carga da semana atual (últimos 7 dias)

---

### 6.5 ACWR (Acute : Chronic Workload Ratio)

ACWR = Carga aguda / Carga crônica

Interpretação usada pelo sistema:
- < 0,8 → carga baixa / possível destreinamento
- 0,8 – 1,3 → zona de adaptação segura
- ≥ 1,5 → alerta de risco aumentado

Esses valores são **heurísticos**, baseados em literatura, e devem ser interpretados pelo treinador.

---

### 6.6 Progressão semanal de carga

- Aumento semanal recomendado: até 10–15%
- Aumentos acima de 20% geram alerta

---

### 6.7 Classificação da fase do treinamento

Com base na tendência de carga:
- **Base**: carga estável ou abaixo da média
- **Construção**: carga crescente controlada
- **Pico**: carga elevada e sustentada
- **Polimento (taper)**: redução progressiva de volume pré-prova

---

### 6.8 Janela de taper

- Janela típica: 7 a 21 dias antes da prova
- Redução esperada de volume: aproximadamente 40–60%
- Intensidade tende a ser mantida

Se o sistema identificar ausência ou excesso de redução, gera alerta.

---

## 7. Insights gerados pelo sistema (MVP)

- Situação atual do atleta (fase do ciclo)
- ACWR atual e tendência
- Aumento abrupto de carga
- Monotonia elevada (carga concentrada)
- Divergência entre carga e rendimento
- Aderência ao planejamento (treinos planejados vs realizados)

Os insights são apresentados **para o professor**, com linguagem técnica e contextualizada.

---

## 8. Princípios do projeto

- Baseado em evidência científica
- Foco em apoio à decisão
- Explicabilidade das regras
- Simplicidade no MVP
- Evolução incremental

---

## 9. Visão de evolução futura

- Integração com sensores e wearables
- Histórico longitudinal por atleta
- Relatórios técnicos
- Ajuste de parâmetros por perfil de atleta
- Modelos preditivos avançados

---

## 10. Resumo

Este projeto é um **sistema profissional de monitoramento de carga e preparação esportiva**, voltado ao treinador, com foco em ciência, clareza e suporte à tomada de decisão, servindo tanto como produto quanto como um projeto técnico sólido para estudos avançados de arquitetura, domínio e regras de negócio.

