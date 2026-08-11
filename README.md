<div align="center">

<!-- Linguagens e Frameworks Principais -->
![.NET 10](https://img.shields.io/badge/.NET%2010-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![C#](https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=c-sharp&logoColor=white)
![TypeScript](https://img.shields.io/badge/TypeScript-007ACC?style=for-the-badge&logo=typescript&logoColor=white)
![React](https://img.shields.io/badge/React-20232A?style=for-the-badge&logo=react&logoColor=61DAFB)
![Vite](https://img.shields.io/badge/Vite-B73BFE?style=for-the-badge&logo=vite&logoColor=FFD62E)

<br>

<!-- Infraestrutura, Banco de Dados e Mensageria -->
![Entity Framework Core](https://img.shields.io/badge/EF_Core-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![Dapper](https://img.shields.io/badge/Dapper-FE0803?style=for-the-badge)
![SQL Server](https://img.shields.io/badge/SQL_Server-CC292B?style=for-the-badge&logo=microsoft-sql-server&logoColor=white)
![RabbitMQ](https://img.shields.io/badge/RabbitMQ-FF6600?style=for-the-badge&logo=rabbitmq&logoColor=white)
![MassTransit](https://img.shields.io/badge/MassTransit-000000?style=for-the-badge)
![SignalR](https://img.shields.io/badge/SignalR-0078D4?style=for-the-badge&logo=microsoft&logoColor=white)
![Docker](https://img.shields.io/badge/Docker-2496ED?style=for-the-badge&logo=docker&logoColor=white)

<br>

<!-- Arquitetura, Padrões, Documentação e Testes -->
![Clean Architecture](https://img.shields.io/badge/Clean_Architecture-007ACC?style=for-the-badge)
![DDD](https://img.shields.io/badge/DDD_Architecture-007ACC?style=for-the-badge)
![CQRS](https://img.shields.io/badge/CQRS_Pattern-007ACC?style=for-the-badge)
![EDA](https://img.shields.io/badge/Event_Driven-007ACC?style=for-the-badge)
![Swagger](https://img.shields.io/badge/Swagger-85EA2D?style=for-the-badge&logo=swagger&logoColor=black)
![xUnit](https://img.shields.io/badge/xUnit-Testing-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)

</div>

#  Sistema de Gestão e Aluguel de Tratores (Telemetria IoT & DDD)
---

##  O Desafio (Contexto de Negócio)

Desenvolvimento de uma plataforma escalável capaz de realizar a gestão completa dos contratos de locação de uma frota de tratores. Simultaneamente, o sistema deve ser capaz de processar um fluxo massivo de informações críticas de telemetria enviadas diretamente por sensores **IoT (Internet of Things)** acoplados aos equipamentos.

Os sensores realizam leituras frequentes de indicadores como:
* Nível de combustível e nível de óleo;
* Pressão dos pneus;
* Rotação e temperatura do motor;
* Velocidade e geolocalização via GPS.

### Requisitos de Arquitetura e Negócio
* **Processamento em Tempo Real:** Ingestão contínua de dados de sensores para garantir o monitoramento ativo.
* **Portal Administrativo:** Visão analítica global da frota, com fila de alertas críticos de manutenção em tempo real.
* **Portal do Cliente:** Transparência total para o locatário e gestão visual dos contratos de aluguel.

  ##  Conceitos e Padrões de Arquitetura Praticados

* **Domain-Driven Design (DDD):** Foco estratégico nas regras de negócio da aplicação, permitindo o isolamento de contextos delimitados (Bounded Contexts) para Contratos e Telemetria.

* **Event-Driven Architecture (EDA):** Modelagem focada em fluxos de eventos para suportar alta escalabilidade e desacoplamento na recepção contínua de sinais de IoT.

* **CQRS (Command Query Responsibility Segregation):** Estrutura conceitual voltada à separação completa de fluxos de modificação de dados (comandos dos sensores) e fluxos analíticos de exibição (consultas de painéis e extratos de custo dinâmicos).

---

##  Como Rodar o Projeto

### Pré-requisitos
- [Docker Desktop](https://www.docker.com/products/docker-desktop/) instalado e em execução.

### Subindo a aplicação
```bash
docker-compose up -d --build
```
Esse comando sobe **4 containers**: SQL Server, RabbitMQ, a API e o Worker IoT.

---

##  Rotas e Endpoints Disponíveis

Após subir a aplicação, todas as rotas ficam disponíveis a partir de `http://localhost:5257`.

### Interfaces Web (Front-End)

| Rota | Descrição |
|---|---|
| [`/`](http://localhost:5257/) |  **Dashboard Administrativo** — Visão geral com KPIs, distribuição e alertas. |
| [`/tratores.html`](http://localhost:5257/tratores.html) |  **Monitoramento da Frota** — Gauges em tempo real de temperatura, pressão, óleo e RPM. |
| [`/clientes.html`](http://localhost:5257/clientes.html) |  **Gestão de Clientes** — Listagem e cadastro de clientes. |
| [`/contratos.html`](http://localhost:5257/contratos.html) |  **Contratos de Aluguel** — Histórico e abertura de novos contratos. |
| [`/alertas.html`](http://localhost:5257/alertas.html) |  **Central de Alertas** — Feed em tempo real de anomalias detectadas via SignalR. |
| [`/swagger`](http://localhost:5257/swagger) |  **Documentação Interativa (Swagger UI)** — Permite testar endpoints da API. |

### API REST — Gestão de Tratores e Telemetria (`/api/tratores`)

| Método | Rota | Descrição |
|---|---|---|
| `POST` | `/api/tratores` | Cadastra um novo equipamento na frota. |
| `GET` | `/api/tratores/{id}` | Consulta o status atual e as últimas métricas de um trator específico. |
| `GET` | `/api/tratores/dashboard` | Lista todos os tratores e métricas em alta performance (Dapper + CQRS). |
| `POST` | `/api/tratores/telemetria` | Recebe carga de dados dos sensores IoT do equipamento. |

### API REST — Gestão de Clientes (`/api/clientes`)

| Método | Rota | Descrição |
|---|---|---|
| `POST` | `/api/clientes` | Cadastra um novo cliente no sistema. |
| `GET` | `/api/clientes` | Lista todos os clientes cadastrados. |

### API REST — Gestão de Contratos de Aluguel (`/api/contratos`)

| Método | Rota | Descrição |
|---|---|---|
| `POST` | `/api/contratos` | Abre um novo contrato de aluguel e atualiza o status do equipamento. |
| `GET` | `/api/contratos` | Lista todo o histórico de contratos da empresa. |

### Tempo Real — SignalR Hub

| Rota | Descrição |
|---|---|
| `/hubs/monitoramento` | 📡 Hub SignalR para recebimento de alertas críticos em tempo real. Evento: `ReceberAlerta`. |

### Serviços de Infraestrutura

| Serviço | URL | Descrição |
|---|---|---|
| RabbitMQ Management | [`http://localhost:15672`](http://localhost:15672) | Painel de controle do RabbitMQ (usuário: `guest`, senha: `guest`). |
| SQL Server | `localhost:1433` | Banco de dados (usuário: `sa`, senha: `TractorAdmin@123!`). |

---

##  Tecnologias Utilizadas (Tech Stack)

Este projeto foi construído com foco em **Alta Performance, Escalabilidade e Arquitetura Limpa**:

* **Frontend:** React, Vite, TypeScript e Vanilla CSS (Design *Glassmorphism*).
* **Backend:** .NET 10 (Minimal APIs, Background Services).
* **Arquitetura:** Domain-Driven Design (DDD), Clean Architecture, CQRS, Event-Driven Architecture (EDA).
* **Mensageria (IoT):** RabbitMQ e MassTransit.
* **Tempo Real:** SignalR (WebSockets).
* **Banco de Dados:** SQL Server via Entity Framework Core.
* **Testes Automatizados:** xUnit, Moq, FluentAssertions (Cobertura 100% no Core Domain).
* **DevOps:** Docker e Docker Compose (Multi-stage builds isolados).

---

##  Engenharia de Software: Fluxo de Event Storming

Para solucionar a alta complexidade do domínio, a modelagem seguiu as etapas do Event Storming:

1. **Eventos de Domínio:** Fatos imutáveis (`ContratoIniciado`, `LeituraDeSensorRecebida`).
2. **Agregados:** Fronteiras lógicas de negócio (`ContratoAluguel`, `Trator`).
3. **Políticas de Negócio:** Automações assíncronas (Risco de Manutenção dispara Alertas Críticos).
4. **Modelos de Leitura:** Projeções otimizadas (CQRS).

### Mapeamento Lógico (Mermaid)

```mermaid
flowchart TD
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
    Sensor([Worker de Telemetria IoT]):::actor --> Cmd2
    Cmd2[Comando: Registrar Metricas]:::command --> Agg2
    Agg2{Agregado: SessaoTelemetria}:::aggregate --> Evt2
    Evt2(Evento: Leitura Recebida):::event

    %% Política e Reação (Arquitetura Reativa)
    Evt2 --> Pol1
    Pol1[[Política: Risco de Manutenção]]:::policy --> Cmd3
    Cmd3[Comando: Gerar Alerta Crítico]:::command --> Agg3
    Agg3{Agregado: Trator}:::aggregate --> Evt3
    Evt3(Evento: Alerta Gerado no RabbitMQ):::event

    %% Read Models / Interfaces Prontas
    Evt3 --> Read1
    Read1[(Portal Admin: Dashboard React + SignalR)]:::readmodel
```

<br>

🟨 Amarelo (Aggregate): Cluster de entidades que encapsula regras de validação e estado.

🟧 Laranja (Domain Event): Mudança significativa ocorrida no negócio. É um dado histórico e imutável.

🟪 Lilás (Policy): Regras automáticas disparadas por eventos assíncronos.

🟩 Verde (Read Model): Projeções e dashboards otimizados para rápida resposta de leitura.




<p align="center">
Desenvolvido por <strong>Luiz Brandão</strong> como projeto prático e de portfólio de arquitetura de software.
</p>
