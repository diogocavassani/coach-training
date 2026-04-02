# Roadmap de Implementação – Tarefas por Semana

Este documento consolida **todas as tarefas do projeto**, organizadas por **semanas/sprints**, considerando uma dedicação média de **5 horas semanais**.

O objetivo é fornecer um **panorama completo**, com início, meio e fim bem definidos, permitindo acompanhamento simples do progresso.

---

## Legenda de status (revisão geral — mar/2026)

| Símbolo | Significado |
|--------|-------------|
| `[x]` | Implementado no código / entregue no repositório |
| `[ ]` | Pendente ou só parcialmente atendido |

*A revisão foi feita cruzando este roadmap com a solução em `CoachTraining.sln`, testes em `tests/CoachTraining.Domain.Tests` e documentação em `docs/` e `README.md`.*

---

## 📅 Semana 1 — Setup, visão e arquitetura
**Objetivo:** Criar a base técnica e organizacional do projeto.  
**Status:** Concluída.

### Tarefas
- [x] Criar repositório do projeto
- [x] Definir e documentar a stack tecnológica
- [x] Criar solução e projetos (`.Domain`, `CoachTraining.App` como Application, `.Infra`, `.API`)
- [x] Configurar referências corretas entre camadas
- [x] Configurar API base (`Program.cs`)
- [x] Criar endpoint de health-check (`GET api/healthcheck`)
- [x] Configurar injeção de dependência básica
- [x] Criar README inicial (visão do projeto)
- [x] Documentar princípios arquiteturais (Clean / DDD) — `README.md`, `ARCHITECTURE.md`, `docs/ARQUITETURA.md`
- [x] Configurar padrão de código (`.editorconfig`, nullable)
- [x] Criar commits iniciais organizados (histórico git ativo)

**Entregável:** Projeto compilando, arquitetura definida e documentação inicial criada.

---

## 📅 Semana 2 — Modelagem de domínio
**Objetivo:** Traduzir o conhecimento científico para um modelo de domínio sólido.  
**Status:** Concluída (testes: ver pendências — cobertura ampla, sem testes dedicados para `Coach` e `Pace`).

### Tarefas
- [x] Criar entidade Coach
- [x] Criar entidade Atleta
- [x] Criar entidade ProvaAlvo
- [x] Criar entidade SessaoDeTreino
- [x] Criar Value Object RPE
- [x] Criar Value Object CargaTreino
- [x] Criar Value Object Pace
- [x] Definir enums de TipoDeTreino e FaseDoCiclo
- [x] Implementar validações de domínio (VOs, construtores das entidades)
- [x] Criar testes unitários de entidades e VOs (`DomainTests` + testes indiretos via serviços; *opcional futuro:* testes dedicados `Coach` / `Pace`)

**Entregável:** Domínio modelado, validado e testado.

---

## 📅 Semana 3 — Cálculo de carga de treino
**Objetivo:** Implementar o núcleo científico do sistema.  
**Status:** Concluída.

### Tarefas
- [x] Implementar cálculo de carga (session-RPE)
- [x] Implementar agregação de carga diária
- [x] Implementar cálculo de carga semanal
- [x] Implementar cálculo de carga crônica (média 4 semanas)
- [x] Criar Domain Service `CalculadoraDeCarga`
- [x] Criar testes com cenários reais simulados (`CalculadoraDeCargaTests`)

**Entregável:** Serviço de cálculo validado por testes.

---

## 📅 Semana 4 — ACWR e progressão de carga
**Objetivo:** Detectar risco e progressão inadequada.  
**Status:** Concluída.

### Tarefas
- [x] Implementar cálculo de carga aguda
- [x] Implementar cálculo de ACWR
- [x] Implementar cálculo de delta percentual semanal
- [x] Definir enum StatusDeRisco (Normal, Atenção, Risco)
- [x] Criar Domain Service `AvaliadorDeRisco`
- [x] Criar testes cobrindo limiares científicos (`AvaliadorDeRiscoTests`)

**Entregável:** Avaliação de risco funcional e testada.

---

## 📅 Semana 5 — Fases do treinamento e taper
**Objetivo:** Identificar o momento fisiológico do atleta.  
**Status:** Concluída.

### Tarefas
- [x] Implementar identificação de tendência de carga
- [x] Implementar classificação da fase do ciclo (Base, Construção, Pico, Polimento)
- [x] Implementar detecção de janela de taper
- [x] Implementar validação de redução de volume pré-prova
- [x] Criar Domain Service `ClassificadorDeFase`
- [x] Criar testes para cenários de prova próxima (`ClassificadorDeFaseTests`)

**Entregável:** Fase do ciclo corretamente identificada.

---

## 📅 Semana 6 — Read model e queries de dashboard
**Objetivo:** Preparar dados consolidados para visualização.  
**Status:** Concluída — dashboard do atleta integrado à persistência real e evoluído com aderência ao planejamento.

### Tarefas
- [x] Definir DTO do dashboard do atleta (`DashboardAtletaDto`)
- [x] Consolidar métricas no Application Layer (`ObterDashboardAtletaService`)
- [x] Criar query de dashboard por atleta (serviço de aplicação + testes `ObterDashboardAtletaServiceTests`)
- [x] Implementar endpoint **funcional** `GET` de dashboard por atleta — integrado ao `ObterDashboardAtletaService` com recuperação persistida do atleta
- [ ] Validar performance e clareza dos dados (sem medição formal / carga real)

**Entregável:** Endpoint de dashboard funcional.

---

## 📅 Semana 7 — Geração de insights para o treinador
**Objetivo:** Transformar métricas em informação acionável.  
**Status:** Concluída.

### Tarefas
- [x] Mapear alertas técnicos para insights textuais (`GeradorDeInsights`)
- [x] Criar mensagens explicáveis e baseadas em evidência
- [x] Priorizar insights por criticidade
- [x] Ajustar linguagem técnica para o treinador
- [x] Criar testes de geração de insights (`GeradorDeInsightsTests`)

**Entregável:** Dashboard com insights claros e úteis.

---

## 📅 Semana 8 — Testes de cenários reais
**Objetivo:** Garantir confiabilidade das regras científicas.  
**Status:** Concluída — cenários implementados e suíte unitária verde.

### Tarefas
- [x] Criar cenário de atleta iniciante
- [x] Criar cenário de atleta intermediário
- [x] Criar cenário de atleta avançado
- [x] Criar cenário de overreaching
- [x] Criar cenário de taper bem executado
- [x] Ajustar limiares conforme resultados (iterações em PRs recentes)
- [x] Garantir suíte verde (`dotnet test`) — suíte atual sem falhas

**Entregável:** Regras validadas com múltiplos perfis.

---

## 📅 Semana 9 — Refinamento e robustez
**Objetivo:** Tornar o projeto estável e apresentável.  
**Status:** Parcial — com evolução em regras defensivas no dashboard.

### Tarefas
- [ ] Revisar mensagens de insights (passível de novo ciclo)
- [x] Tratar dados inconsistentes (regras defensivas no dashboard para ignorar sessões inválidas no cálculo)
- [ ] Ajustar performance de queries (relevante após persistência)
- [ ] Revisar código (clean code) — contínuo
- [ ] Pequenos refactors orientados à clareza — contínuo

**Entregável:** Sistema estável e refinado.

---

## 📅 Semana 10 — Documentação e fechamento
**Objetivo:** Finalizar o projeto com qualidade profissional.  
**Status:** Concluída no escopo de documentação do MVP.

### Tarefas
- [x] Documentar regras científicas utilizadas (`docs/projeto_sistema_de_monitoramento_de_carga_e_preparacao_de_atletas.md` — secs. 5–6)
- [x] Documentar limitações do sistema (escopo MVP / fora do MVP no mesmo doc)
- [x] Documentar possíveis evoluções futuras (sec. 9 do doc de projeto)
- [x] Atualizar README (visão, arquitetura, stack; status em evolução)
- [x] Criar overview do projeto (portfólio) — `docs/overview_do_projeto.md`

**Entregável final:** Projeto completo, documentado e pronto para apresentação.

---

## ⚠️ Pendências consolidadas (além dos itens `[ ]` acima)

1. **Performance e qualidade de dados:** medir carga real, validar volumes maiores e evoluir observabilidade.
2. **Observabilidade operacional:** ampliar logs, health checks e métricas de suporte.
3. **Evoluções pós-MVP:** integrações externas, relatórios e novos refinamentos de UX.

---

## ✅ Resumo Final

- **Duração planejada:** 10 semanas · **Dedicação média:** 5 h/semana
- **Progresso estimado:** Semanas 1–10 **concluídas no escopo do MVP**, com Semana 9 permanecendo aberta para novos refinamentos incrementais.
- **Resultado alvo:** Sistema funcional de apoio à decisão para treinadores, baseado em ciência do esporte, com arquitetura limpa e regras bem documentadas.
