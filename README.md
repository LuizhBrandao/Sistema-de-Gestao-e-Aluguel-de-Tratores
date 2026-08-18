<div align="center">

<!-- Linguagens e Frameworks Principais -->
![.NET 10](https://img.shields.io/badge/.NET%2010-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![C#](https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=c-sharp&logoColor=white)
![Blazor](https://img.shields.io/badge/Blazor-512BD4?style=for-the-badge&logo=blazor&logoColor=white)
![.NET Aspire](https://img.shields.io/badge/.NET%20Aspire-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)

<br>

<!-- Infraestrutura, Banco de Dados e Mensageria -->
![Entity Framework Core](https://img.shields.io/badge/EF_Core-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![Dapper](https://img.shields.io/badge/Dapper-FE0803?style=for-the-badge)
![SQL Server](https://img.shields.io/badge/SQL_Server-CC292B?style=for-the-badge&logo=microsoft-sql-server&logoColor=white)
![RabbitMQ](https://img.shields.io/badge/RabbitMQ-FF6600?style=for-the-badge&logo=rabbitmq&logoColor=white)
![MassTransit](https://img.shields.io/badge/MassTransit-000000?style=for-the-badge)
![SignalR](https://img.shields.io/badge/SignalR-0078D4?style=for-the-badge&logo=microsoft&logoColor=white)

<br>

<!-- Arquitetura, Padrões, Documentação e Testes -->
![Clean Architecture](https://img.shields.io/badge/Clean_Architecture-007ACC?style=for-the-badge)
![DDD](https://img.shields.io/badge/DDD_Architecture-007ACC?style=for-the-badge)
![CQRS](https://img.shields.io/badge/CQRS_Pattern-007ACC?style=for-the-badge)
![EDA](https://img.shields.io/badge/Event_Driven-007ACC?style=for-the-badge)
![Swagger](https://img.shields.io/badge/Swagger-85EA2D?style=for-the-badge&logo=swagger&logoColor=black)
![xUnit](https://img.shields.io/badge/xUnit-Testing-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)

</div>

# Sistema de Gestão e Aluguel de Tratores (Telemetria IoT, DDD & Aspire)
---

## O Desafio (Contexto de Negócio)

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

## Conceitos e Padrões de Arquitetura Praticados

* **Domain-Driven Design (DDD):** Foco estratégico nas regras de negócio da aplicação, permitindo o isolamento de contextos delimitados (Bounded Contexts) para Contratos e Telemetria.
* **Event-Driven Architecture (EDA):** Modelagem focada em fluxos de eventos para suportar alta escalabilidade e desacoplamento na recepção contínua de sinais de IoT.
* **CQRS (Command Query Responsibility Segregation):** Estrutura conceitual voltada à separação completa de fluxos de modificação de dados (comandos dos sensores) e fluxos analíticos de exibição (consultas de painéis e extratos de custo dinâmicos).
* **.NET Aspire:** Orquestração nativa de microsserviços, injeção automática de Service Discovery e Telemetria distribuída (OpenTelemetry).

---

## Como Rodar o Projeto

Este projeto utiliza **.NET Aspire** para orquestração local, dispensando a necessidade de configurar portas manualmente ou rodar via `docker-compose` para o ambiente de desenvolvimento. O próprio Aspire se encarregará de subir os contêineres necessários (SQL Server, RabbitMQ) via Docker Desktop, além dos projetos .NET da solução.

### Pré-requisitos
- [Docker Desktop](https://www.docker.com/products/docker-desktop/) instalado e em execução (para que o Aspire suba o SQL e o RabbitMQ).
- SDK do .NET 10 instalado.

### Subindo a aplicação

Você pode rodar diretamente via CLI ou pelo seu Visual Studio/Rider.

**Via CLI:**
```bash
dotnet run --project src/TractorRental.AppHost/TractorRental.AppHost.csproj
```

**Via Visual Studio:**
1. Defina o projeto `TractorRental.AppHost` como projeto de inicialização (*Startup Project*).
2. Aperte `F5` ou clique em `Run`.

Após compilar, um navegador abrirá o **Painel do .NET Aspire**. Através desse painel, você terá acesso imediato aos endpoints de:
- **`frontend`** (Portal Administrativo em Blazor)
- **`api`** (Backend REST e Swagger)
- Logs, métricas e traces em tempo real de toda a arquitetura.

---

## Interfaces e Rotas Principais (Disponíveis via Painel Aspire)

### 💻 Portal Administrativo (Blazor Server)
O Front-End foi modernizado e centralizado no ecossistema .NET através de **Blazor**.
As rotas da interface incluem:
- `/` - **Dashboard:** Visão geral com KPIs, distribuição e alertas.
- `/frota` - **Monitoramento da Frota:** Lista de tratores e status.
- `/clientes` - **Gestão de Clientes:** Listagem e cadastro de clientes.
- `/contratos` - **Contratos de Aluguel:** Histórico de locações de maquinário.
- `/alertas` - **Central de Alertas:** Feed em tempo real de anomalias detectadas (alimentado via SignalR).

### ⚙️ API REST e Swagger (`api`)
A API central de operações fornece endpoints documentados via Swagger.
- **`GET /swagger`** - Interface interativa de testes.
- **`POST /api/tratores/telemetria`** - Ponto focal para recepção de dados dos sensores IoT.
- Outros mapeamentos essenciais de CRUDs (Tratores, Contratos, Clientes).

### 📡 Tempo Real — SignalR Hub
- `/hubs/monitoramento` - Hub WebSocket para transmissão ao vivo dos alertas críticos para o frontend Blazor (conectado de forma fluida via Service Discovery do Aspire).

---

## Tecnologias Utilizadas (Tech Stack)

Este projeto foi construído com foco em **Alta Performance, Escalabilidade e Arquitetura Limpa**, unificando tudo com a robustez do .NET:

* **Frontend:** Blazor Web App (Interactive Server Mode) integrado com Bootstrap.
* **Backend:** .NET 10 (Minimal APIs, Background Services / IoT Worker).
* **Orquestração e Observabilidade:** .NET Aspire (OpenTelemetry, Service Defaults).
* **Arquitetura:** Domain-Driven Design (DDD), Clean Architecture, CQRS, Event-Driven Architecture (EDA).
* **Mensageria (IoT):** RabbitMQ e MassTransit.
* **Tempo Real:** SignalR (WebSockets).
* **Banco de Dados:** SQL Server via Entity Framework Core + Dapper para leituras de alta performance.
* **Testes Automatizados:** xUnit, Moq, FluentAssertions (Cobertura no Core Domain).

---

## Engenharia de Software: Fluxo de Event Storming

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
    Read1[(Portal Admin: Blazor Server + SignalR)]:::readmodel
```

<br>

🟨 Amarelo (Aggregate): Cluster de entidades que encapsula regras de validação e estado.
🟧 Laranja (Domain Event): Mudança significativa ocorrida no negócio. É um dado histórico e imutável.
🟪 Lilás (Policy): Regras automáticas disparadas por eventos assíncronos.
🟩 Verde (Read Model): Projeções e dashboards otimizados para rápida resposta de leitura.


<p align="center">
Desenvolvido por <strong>Luiz Brandão</strong> como projeto prático e de portfólio de arquitetura de software.
</p>
