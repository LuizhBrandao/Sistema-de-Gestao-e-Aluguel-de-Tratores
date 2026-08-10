<div align="center">

![.NET 10](https://img.shields.io/badge/.NET%2010-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![C#](https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=c-sharp&logoColor=white)
![SQL Server](https://img.shields.io/badge/SQL_Server-CC292B?style=for-the-badge&logo=microsoft-sql-server&logoColor=white)
![EF Core](https://img.shields.io/badge/EF_Core-336791?style=for-the-badge&logo=nuget&logoColor=white)
![Dapper](https://img.shields.io/badge/Dapper-E34F26?style=for-the-badge&logo=nuget&logoColor=white)
![RabbitMQ](https://img.shields.io/badge/RabbitMQ-FF6600?style=for-the-badge&logo=rabbitmq&logoColor=white)
![SignalR](https://img.shields.io/badge/SignalR-0078D4?style=for-the-badge&logo=microsoft&logoColor=white)
![Docker](https://img.shields.io/badge/Docker-2496ED?style=for-the-badge&logo=docker&logoColor=white)
![Bootstrap](https://img.shields.io/badge/Bootstrap-563D7C?style=for-the-badge&logo=bootstrap&logoColor=white)
<br>
![DDD](https://img.shields.io/badge/DDD_Architecture-007ACC?style=for-the-badge)
![CQRS](https://img.shields.io/badge/CQRS_Pattern-007ACC?style=for-the-badge)
![EDA](https://img.shields.io/badge/Event_Driven-007ACC?style=for-the-badge)
![xUnit](https://img.shields.io/badge/xUnit-Testing-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)

</div>

#  Sistema de Gestão e Aluguel de Tratores (Telemetria IoT & DDD)
---

##  O Desafio (Contexto de Negócio)

### O Problema
Desenvolvimento de uma plataforma escalável capaz de realizar a gestão completa dos contratos de locação de uma frota de tratores. Simultaneamente, o sistema deve ser capaz de processar um fluxo massivo de informações críticas de telemetria enviadas diretamente por sensores **IoT (Internet of Things)** acoplados aos equipamentos.

Os sensores realizam leituras frequentes de indicadores estruturais:
* Nível de combustível e nível de óleo;
* Pressão dos pneus;
* Rotação e temperatura do motor;
* Velocidade e geolocalização via GPS.

### Requisitos de Arquitetura e Negócio
* **Processamento em Tempo Real:** Ingestão contínua de dados de sensores para garantir o monitoramento ativo.
* **Portal Administrativo:** Visão analítica global da frota, com fila de alertas críticos de manutenção para os operadores.
* **Portal do Cliente:** Transparência total para o locatário, exibindo mostradores de status do equipamento alugado e cálculo dinâmico de custos.

---

##  Tecnologias Utilizadas (Tech Stack)

Este projeto foi construído com foco em **Alta Performance e Arquitetura Limpa**, utilizando padrões consolidados da engenharia de software:

* **Backend:** .NET 10 (Minimal APIs), C# 14.
* **Arquitetura:** Domain-Driven Design (DDD), Clean Architecture, CQRS, Event-Driven Architecture (EDA).
* **Banco de Dados:** SQL Server.
* **ORM & Micro-ORM:** Entity Framework Core (Escrita/Comandos) e Dapper (Leitura/Consultas de alta performance).
* **Mensageria (IoT):** RabbitMQ e MassTransit.
* **Tempo Real & Frontend:** SignalR (WebSockets), HTML5, JavaScript (Fetch API) e Bootstrap 5.
* **Testes de Unidade:** xUnit, Moq, FluentAssertions (100% de cobertura no Core Domain).
* **DevOps:** Docker e Docker Compose (Multi-stage builds, Auto-Migrations).

---

##  Como Rodar o Projeto (Quick Start)

Graças ao Docker, você pode rodar a infraestrutura completa da aplicação (Banco de Dados, Mensageria, Worker de IoT e API/Frontend) com um único comando.

**Pré-requisitos:**
* [Docker Desktop](https://www.docker.com/products/docker-desktop/) rodando na máquina.

**Passo a passo:**
1. Clone o repositório:
   ```bash
   git clone [https://github.com/SEU_USUARIO/Sistema-de-Gestao-e-Aluguel-de-Tratores.git](https://github.com/SEU_USUARIO/Sistema-de-Gestao-e-Aluguel-de-Tratores.git)
   cd Sistema-de-Gestao-e-Aluguel-de-Tratores
Suba os contêineres e a infraestrutura:

Bash
docker-compose up --build -d
(A API rodará as Migrations automaticamente e criará as tabelas no SQL Server).

Acesse as Interfaces do Sistema:

 Swagger (API): http://localhost:5257/swagger

 Dashboard Fullstack da Frota: http://localhost:5257/portal

 Painel de Alertas em Tempo Real (SignalR): http://localhost:5257/painel

 Painel do RabbitMQ: http://localhost:15672 (Login: guest / Senha: guest)
 
 ---

## Engenharia de Software: Fluxo de Event Storming

Para solucionar a alta complexidade do domínio e os desafios técnicos de concorrência de dados, a modelagem foi segmentada seguindo as etapas táticas do Event Storming:

Eventos de Domínio: Mapeamento e descoberta de fatos imutáveis do passado (ContratoIniciado, LeituraDeSensorRecebida).

Definição de Agregados: Estabelecimento de fronteiras lógicas de consistência para regras de negócio (ContratoAluguel, Trator).

Políticas de Negócio: Automações orientadas a eventos baseadas na premissa "Sempre que o Evento X ocorrer, dispare o Comando Y".

Modelos de Leitura: Projeção via Dapper otimizada para suprir as telas finais sem onerar o banco de dados principal (CQRS).

Abaixo está o mapeamento lógico e visual do comportamento da aplicação:


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
    Sensor([Robô de Telemetria IoT]):::actor --> Cmd2
    Cmd2[Comando: Registrar Metricas]:::command --> Agg2
    Agg2{Agregado: SessaoTelemetria}:::aggregate --> Evt2
    Evt2(Evento: Leitura Recebida):::event

    %% Política e Reação (Arquitetura Reativa)
    Evt2 --> Pol1
    Pol1[[Política: Risco de Múltipla Manutenção]]:::policy --> Cmd3
    Cmd3[Comando: Gerar Alerta Crítico]:::command --> Agg3
    Agg3{Agregado: Trator}:::aggregate --> Evt3
    Evt3(Evento: Alerta Gerado no RabbitMQ):::event

    %% Read Models / Interfaces Prontas
    Evt3 --> Read1
    Read1[(Portal Admin: Painel de Alertas em Tempo Real)]:::readmodel
    Evt1 --> Read2
    Read2[(Portal Cliente: Dashboard CQRS com Dapper)]:::readmodel
````
Legenda do Modelo de Domínio

🟦 Azul (Command): Uma intenção ou ação direta disparada por um usuário ou sistema externo.

🟨 Amarelo (Aggregate): Cluster de entidades que encapsula regras de validação e estado.

🟧 Laranja (Domain Event): Mudança significativa ocorrida no negócio. É um dado histórico e imutável.

🟪 Lilás (Policy): Regras automáticas disparadas por eventos assíncronos.

🟩 Verde (Read Model): Projeções e dashboards otimizados para rápida resposta de leitura.

<<<<<<< HEAD
##  Conceitos e Padrões de Arquitetura Praticados

Domain-Driven Design (DDD): Foco estratégico nas regras de negócio da aplicação, permitindo o isolamento de contextos delimitados (Bounded Contexts) para Contratos e Telemetria.

Event-Driven Architecture (EDA): Modelagem focada em fluxos de eventos para suportar alta escalabilidade e desacoplamento na recepção contínua de sinais de IoT.

CQRS (Command Query Responsibility Segregation): Estrutura conceitual voltada à separação completa de fluxos de modificação de dados (comandos dos sensores) e fluxos analíticos de exibição (consultas de painéis e extratos de custo dinâmicos).

---

## 🚀 Como Rodar o Projeto (Ambiente Local)

Este projeto foi containerizado para garantir que **qualquer avaliador técnico** consiga rodá-lo localmente com facilidade, sem instalar SDKs do .NET ou Node.js.

### Pré-requisitos
- **Docker** e **Docker Compose** instalados na sua máquina.

### Passo a Passo

1. Clone o repositório e navegue até a pasta raiz do projeto.
2. No terminal, execute o seguinte comando para construir as imagens e subir a arquitetura completa:
   ```bash
   docker-compose up -d --build
   ```
3. Aguarde alguns segundos para os serviços subirem. O Docker Compose iniciará:
   * **SQL Server** (Banco de dados).
   * **RabbitMQ** (Mensageria e eventos).
   * **TractorRental.Api** (Backend principal na porta `5000`).
   * **TractorRental.IoTWorker** (Simulador de sensores IoT gerando telemetria em background).
   * **TractorRental.Frontend** (Dashboard Web UI responsiva).

4. Acesse o Frontend no seu navegador:
   🔗 **http://localhost:80**

Lá você poderá visualizar em **Tempo Real** os tratores recebendo telemetria via WebSocket (SignalR), alertas sendo gerados baseados em políticas de domínio, e gestão visual dos contratos de aluguel.
=======
Desenvolvido por Luiz Brandão como projeto de portfólio.
>>>>>>> 8e0d6d9e8781febc2de2e6bd4ab63041d945357e
