![Desafio Original](<Desafio Analise de projetos com DDD.jpeg>)
# 🚜 Sistema de Gestão e Aluguel de Tratores (Telemetria IoT & DDD)
---

## 📌 O Desafio (Contexto de Negócio)

### O Problema
Uma empresa de engenharia e logística contratou o desenvolvimento de uma plataforma escalável capaz de realizar a gestão completa dos contratos de locação de sua frota de tratores. Simultaneamente, o sistema deve ser capaz de processar um fluxo massivo de informações críticas de telemetria enviadas diretamente por sensores **IoT (Internet of Things)** acoplados aos equipamentos.

Os sensores realizam leituras frequentes de indicadores estruturais:
* Nível de combustível e nível de óleo;
* Pressão dos pneus;
* Rotação e temperatura do motor;
* Velocidade e geolocalização via GPS.

---

## 🧠 Engenharia de Software: Fluxo de Event Storming

Abaixo está o mapeamento lógico e visual do comportamento da aplicação. O fluxo foi construído utilizando a sintaxe **Mermaid** e é renderizado nativamente pelo GitHub.

```mermaid
flowchart TD
    %% Cores Padrão de Mercado para Event Storming
    classDef command fill:#add8e6,stroke:#333,stroke-width:1px;
    classDef event fill:#ffb347,stroke:#333,stroke-width:1px;
    classDef aggregate fill:#fdfd96,stroke:#333,stroke-width:1px;
    classDef policy fill:#cbaacb,stroke:#333,stroke-width:1px;
    classDef readmodel fill:#77dd77,stroke:#333,stroke-width:1px;
    classDef actor fill:#ffcccb,stroke:#333,stroke-width:1px;

    %% Fluxo de Contrato
    Admin([Administrador]):::actor --> Cmd1
    Cmd1[Comando: Iniciar Contrato]:::command --> Agg1
    Agg1{Agregado: ContratoAluguel}:::aggregate --> Evt1
    Evt1(Evento: Contrato Iniciado):::event

    %% Fluxo de Telemetria IoT
    Sensor([Sensor do Trator]):::actor --> Cmd2
    Cmd2[Comando: Registrar Metricas]:::command --> Agg2
    Agg2{Agregado: SessaoTelemetria}:::aggregate --> Evt2
    Evt2(Evento: Leitura Recebida):::event

    %% Política e Reação (Arquitetura Reativa)
    Evt2 --> Pol1
    Pol1[[Política: Risco de Manutenção]]:::policy --> Cmd3
    Cmd3[Comando: Gerar Alerta]:::command --> Agg3
    Agg3{Agregado: Trator}:::aggregate --> Evt3
    Evt3(Evento: Alerta Gerado):::event

    %% Read Models / Interfaces Prontas
    Evt3 --> Read1
    Read1[(Portal Admin: Painel de Alertas)]:::readmodel
    Evt1 --> Read2
    Read2[(Portal Cliente: Gestão de Contratos e Custos)]:::readmodel
````
## 🎨 Legenda do Modelo de Domínio

🟦 Azul (Command): Uma intenção ou ação direta disparada por um usuário ou sistema externo.

🟨 Amarelo (Aggregate): Cluster de entidades que encapsula regras de validação e estado.

🟧 Laranja (Domain Event): Mudança significativa ocorrida no negócio. É um dado histórico e imutável.

🟪 Lilás (Policy): Regras automáticas disparadas por eventos assíncronos.

🟩 Verde (Read Model): Projeções e dashboards otimizados para rápida resposta de leitura.
