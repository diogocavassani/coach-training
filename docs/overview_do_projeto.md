# Overview do Projeto — CoachTraining

## Proposta

O **CoachTraining** e um sistema de apoio a decisao para treinadores de corrida, com foco em consolidar dados de treino e transformar indicadores tecnicos em informacao acionavel.

O produto **nao prescreve treinos** e **nao substitui o treinador**. O objetivo e reduzir subjetividade no acompanhamento de carga e risco, apoiando decisao com base em ciencia do esporte.

## Problema que resolve

Treinadores lidam com planilhas dispersas e sinais de risco pouco padronizados. Isso dificulta:

- perceber progressao abrupta de carga;
- comparar semana atual vs semana anterior;
- comunicar ao atleta o nivel de risco com clareza.

## Solucao implementada (MVP atual)

- Cadastro basico de atleta via API;
- Dashboard consolidado por atleta;
- Calculo de carga por `session-RPE`;
- Indicadores de carga aguda, carga cronica e ACWR;
- Avaliacao de risco (Normal, Atencao, Risco);
- Classificacao de fase do ciclo e verificacao de janela de taper;
- Geracao de insights textuais orientados ao treinador.

## Diferenciais tecnicos

- Arquitetura limpa com separacao clara de responsabilidades;
- Dominio modelado com DDD (Entities, Value Objects, Domain Services);
- Regras de negocio cobertas por testes unitarios;
- API com endpoints HTTP e validacoes basicas.

## Estado atual

- Suíte de testes unitarios verde;
- Endpoint de dashboard funcional para atletas cadastrados no ciclo de vida da aplicacao;
- Persistencia em banco ainda pendente (proxima evolucao natural do MVP).

## Proximos passos sugeridos

1. Implementar persistencia (Infra + repositórios + EF Core);
2. Expor cadastro e consulta de sessoes de treino via API;
3. Adicionar prova alvo por atleta e planejamento semanal;
4. Evoluir observabilidade (logs estruturados e metricas).
