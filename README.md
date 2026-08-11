#  Sistema de Gestão e Aluguel de Tratores (Telemetria IoT & DDD)
---

##  O Desafio (Contexto de Negócio)

### O Problema
Desenvolvimento de uma plataforma escalável capaz de realizar a gestão completa dos contratos de locação de sua frota de tratores. Simultaneamente, o sistema deve ser capaz de processar um fluxo massivo de informações críticas de telemetria enviadas diretamente por sensores **IoT (Internet of Things)** acoplados aos equipamentos.

Os sensores realizam leituras frequentes de indicadores estruturais:
* Nível de combustível e nível de óleo;
* Pressão dos pneus;
* Rotação e temperatura do motor;
* Velocidade e geolocalização via GPS.

### Requisitos de Arquitetura e Negócio
* **Processamento em Tempo Real:** Ingestão contínua de dados de sensores para garantir o monitoramento ativo.
* **Portal Administrativo:** Visão analítica global da frota, com fila de alertas críticos de manutenção para os operadores.
* **Portal do Cliente:** Transparência total para o locatário, exibindo mostradores de status do equipamento alugado e cálculo dinâmico de custos parciais acumulados até o exato momento.

---

##  Objetivos do Projeto

Para solucionar a alta complexidade do domínio e os desafios técnicos de concorrência de dados, a modelagem foi segmentada seguindo as etapas táticas do Event Storming:
1. **Eventos de Domínio:** Mapeamento e descoberta de fatos imutáveis do passado (`ContratoIniciado`, `LeituraDeSensorRecebida`).
2. **Definição de Agregados (Aggregates):** Estabelecimento de fronteiras lógicas de consistência para regras de negócio (`ContratoAluguel`, `Trator`, `SessaoTelemetria`).
3. **Políticas de Negócio (Reações):** Automações orientadas a eventos baseadas na premissa *"Sempre que o Evento X ocorrer, dispare o Comando Y"*.
4. **Modelos de Leitura (Read Models):** Projeção e segregação de dados otimizadas para suprir as telas das interfaces finais sem onerar o banco de dados principal.

---

##  Engenharia de Software: Fluxo de Event Storming

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
##  Legenda do Modelo de Domínio

🟦 Azul (Command): Uma intenção ou ação direta disparada por um usuário ou sistema externo.

🟨 Amarelo (Aggregate): Cluster de entidades que encapsula regras de validação e estado.

🟧 Laranja (Domain Event): Mudança significativa ocorrida no negócio. É um dado histórico e imutável.

🟪 Lilás (Policy): Regras automáticas disparadas por eventos assíncronos.

🟩 Verde (Read Model): Projeções e dashboards otimizados para rápida resposta de leitura.

## 🚀 Como Rodar o Projeto

### Pré-requisitos
- [Docker Desktop](https://www.docker.com/products/docker-desktop/) instalado e em execução.

### Subindo a aplicação
```bash
docker-compose up -d --build
```
Esse comando sobe **4 containers**: SQL Server, RabbitMQ, a API e o Worker IoT.

---

## 🗺️ Rotas e Endpoints Disponíveis

Após subir a aplicação, todas as rotas ficam disponíveis a partir de `http://localhost:5257`.

### Interfaces Web (Front-End)

| Rota | Descrição |
|---|---|
| [`/portal`](http://localhost:5257/portal) | 🖥️ **Dashboard Administrativo** — Visão geral da frota com cards de status, temperatura, pressão, óleo e RPM de cada trator. |
| [`/painel`](http://localhost:5257/painel) | 🔔 **Painel de Alertas em Tempo Real** — Recebe alertas críticos via SignalR quando sensores detectam condições de risco. |
| [`/swagger`](http://localhost:5257/swagger) | 📚 **Documentação Interativa (Swagger UI)** — Permite testar todos os endpoints da API diretamente pelo navegador. |

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

## 📐 Conceitos e Padrões de Arquitetura Praticados

* **Domain-Driven Design (DDD):** Foco estratégico nas regras de negócio da aplicação, permitindo o isolamento de contextos delimitados (Bounded Contexts) para Contratos e Telemetria.

* **Event-Driven Architecture (EDA):** Modelagem focada em fluxos de eventos para suportar alta escalabilidade e desacoplamento na recepção contínua de sinais de IoT.

* **CQRS (Command Query Responsibility Segregation):** Estrutura conceitual voltada à separação completa de fluxos de modificação de dados (comandos dos sensores) e fluxos analíticos de exibição (consultas de painéis e extratos de custo dinâmicos).
