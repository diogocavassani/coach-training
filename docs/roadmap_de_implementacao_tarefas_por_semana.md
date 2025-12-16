# Roadmap de Implementação – Tarefas por Semana

Este documento consolida **todas as tarefas do projeto**, organizadas por **semanas/sprints**, considerando uma dedicação média de **5 horas semanais**.

O objetivo é fornecer um **panorama completo**, com início, meio e fim bem definidos, permitindo acompanhamento simples do progresso.

---

## 📅 Semana 1 — Setup, visão e arquitetura
**Objetivo:** Criar a base técnica e organizacional do projeto.

### Tarefas
- Criar repositório do projeto
- Definir e documentar a stack tecnológica
- Criar solução e projetos (.Domain, .Application, .Infra, .API)
- Configurar referências corretas entre camadas
- Configurar API base (Program.cs)
- Criar endpoint de health-check
- Configurar injeção de dependência básica
- Criar README inicial (visão do projeto)
- Documentar princípios arquiteturais (Clean / DDD)
- Configurar padrão de código (.editorconfig, nullable)
- Criar commits iniciais organizados

**Entregável:** Projeto compilando, arquitetura definida e documentação inicial criada.

---

## 📅 Semana 2 — Modelagem de domínio
**Objetivo:** Traduzir o conhecimento científico para um modelo de domínio sólido.

### Tarefas
- Criar entidade Coach
- Criar entidade Atleta
- Criar entidade ProvaAlvo
- Criar entidade SessaoDeTreino
- Criar Value Object RPE
- Criar Value Object CargaTreino
- Criar Value Object Pace
- Definir enums de TipoDeTreino e FaseDoCiclo
- Implementar validações de domínio
- Criar testes unitários de entidades e VOs

**Entregável:** Domínio modelado, validado e testado.

---

## 📅 Semana 3 — Cálculo de carga de treino
**Objetivo:** Implementar o núcleo científico do sistema.

### Tarefas
- Implementar cálculo de carga (session-RPE)
- Implementar agregação de carga diária
- Implementar cálculo de carga semanal
- Implementar cálculo de carga crônica (média 4 semanas)
- Criar Domain Service CalculadoraDeCarga
- Criar testes com cenários reais simulados

**Entregável:** Serviço de cálculo validado por testes.

---

## 📅 Semana 4 — ACWR e progressão de carga
**Objetivo:** Detectar risco e progressão inadequada.

### Tarefas
- Implementar cálculo de carga aguda
- Implementar cálculo de ACWR
- Implementar cálculo de delta percentual semanal
- Definir enum StatusDeRisco (Normal, Atenção, Risco)
- Criar Domain Service AvaliadorDeRisco
- Criar testes cobrindo limiares científicos

**Entregável:** Avaliação de risco funcional e testada.

---

## 📅 Semana 5 — Fases do treinamento e taper
**Objetivo:** Identificar o momento fisiológico do atleta.

### Tarefas
- Implementar identificação de tendência de carga
- Implementar classificação da fase do ciclo (Base, Construção, Pico, Polimento)
- Implementar detecção de janela de taper
- Implementar validação de redução de volume pré-prova
- Criar Domain Service ClassificadorDeFase
- Criar testes para cenários de prova próxima

**Entregável:** Fase do ciclo corretamente identificada.

---

## 📅 Semana 6 — Read model e queries de dashboard
**Objetivo:** Preparar dados consolidados para visualização.

### Tarefas
- Definir DTO do dashboard do atleta
- Consolidar métricas no Application Layer
- Criar query de dashboard por atleta
- Implementar endpoint GET /dashboard/atleta/{id}
- Validar performance e clareza dos dados

**Entregável:** Endpoint de dashboard funcional.

---

## 📅 Semana 7 — Geração de insights para o treinador
**Objetivo:** Transformar métricas em informação acionável.

### Tarefas
- Mapear alertas técnicos para insights textuais
- Criar mensagens explicáveis e baseadas em evidência
- Priorizar insights por criticidade
- Ajustar linguagem técnica para o treinador
- Criar testes de geração de insights

**Entregável:** Dashboard com insights claros e úteis.

---

## 📅 Semana 8 — Testes de cenários reais
**Objetivo:** Garantir confiabilidade das regras científicas.

### Tarefas
- Criar cenário de atleta iniciante
- Criar cenário de atleta intermediário
- Criar cenário de atleta avançado
- Criar cenário de overreaching
- Criar cenário de taper bem executado
- Ajustar limiares conforme resultados

**Entregável:** Regras validadas com múltiplos perfis.

---

## 📅 Semana 9 — Refinamento e robustez
**Objetivo:** Tornar o projeto estável e apresentável.

### Tarefas
- Revisar mensagens de insights
- Tratar dados inconsistentes
- Ajustar performance de queries
- Revisar código (clean code)
- Pequenos refactors orientados à clareza

**Entregável:** Sistema estável e refinado.

---

## 📅 Semana 10 — Documentação e fechamento
**Objetivo:** Finalizar o projeto com qualidade profissional.

### Tarefas
- Documentar regras científicas utilizadas
- Documentar limitações do sistema
- Documentar possíveis evoluções futuras
- Atualizar README final
- Criar overview do projeto (portfólio)

**Entregável final:** Projeto completo, documentado e pronto para apresentação.

---

## ✅ Resumo Final

- Duração total: 10 semanas
- Dedicação média: 5h/semana
- Resultado: Sistema funcional de apoio à decisão para treinadores, baseado em ciência do esporte, com arquitetura limpa e regras bem documentadas.

