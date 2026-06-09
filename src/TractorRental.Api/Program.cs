using MassTransit;
using TractorRental.Api.Consumers; 
using TractorRental.Api.Endpoints;
using TractorRental.Api.Hubs; 
using TractorRental.Api.Services;
using TractorRental.Application.Commands;
using TractorRental.Infrastructure;
using Microsoft.EntityFrameworkCore;
using TractorRental.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(RegistrarTelemetriaCommand).Assembly));

// 1. Adiciona os serviços do SignalR no contêiner
builder.Services.AddSignalR();

builder.Services.AddMassTransit(x =>
{
    // Registra o nosso consumidor de alertas
    x.AddConsumer<AlertaCriticoConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        // Lê da variável de ambiente ou usa localhost
        var rabbitHost = builder.Configuration["RabbitHost"] ?? "localhost";

        cfg.Host(rabbitHost, "/", h => {
            h.Username("guest");
            h.Password("guest");
        });

        // 2. Cria a fila para escutar os alertas que vêm do Worker
        cfg.ReceiveEndpoint("alertas-frontend", e =>
        {
            e.ConfigureConsumer<AlertaCriticoConsumer>(context);
        });
    });
});

builder.Services.AddHostedService<SensorSimulatorWorker>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options => options.SwaggerEndpoint("/openapi/v1.json", "Tractor Rental API"));
}

app.UseHttpsRedirection();

// As nossas 3 "gavetas" de endpoints organizadas 
app.MapTratorEndpoints();
app.MapClienteEndpoints();   
app.MapContratoEndpoints();  

// 3. Mapeia a URL do SignalR
app.MapHub<MonitoramentoHub>("/hubs/monitoramento");

// 4. Cria uma interface visual rápida para testarmos o Tempo Real
app.MapGet("/painel", () => Results.Content(@"
<!DOCTYPE html>
<html lang='pt-BR'>
<head>
    <meta charset='UTF-8'>
    <title>Painel de Monitoramento 🚜</title>
    <style>
        body { font-family: Arial; background-color: #1e1e1e; color: #fff; padding: 20px; }
        .alerta { background-color: #ff4c4c; padding: 15px; margin: 10px 0; border-radius: 5px; font-weight: bold; }
    </style>
    <script src='https://cdnjs.cloudflare.com/ajax/libs/microsoft-signalr/8.0.0/signalr.min.js'></script>
</head>
<body>
    <h1>Painel de Alertas (v2)</h1>
    <p>Aguardando eventos dos tratores...</p>
    <div id='alertas-container'></div>

    <script>
        const conn = new signalR.HubConnectionBuilder().withUrl('/hubs/monitoramento').build();

        conn.on('ReceberAlerta', (alerta) => {
            console.log('ALERTA RECEBIDO DO SIGNALR:', alerta); 
            
            // Pega os valores exatamente como o SignalR enviou (minúsculo)
            const temp = alerta.temperatura;
            const msg = alerta.mensagem;
            const trator = alerta.tratorId;

            // Garantia extra: Só chama o toFixed se temp for realmente um número
            const tempFormatada = temp ? Number(temp).toFixed(1) : 'Erro na leitura';

            const div = document.createElement('div');
            div.className = 'alerta';
            div.innerText = `🔥 ${msg} | Trator: ${trator} | Temp: ${tempFormatada}ºC`;
            
            const container = document.getElementById('alertas-container');
            container.insertBefore(div, container.firstChild);
        });

        conn.start().then(() => console.log('Conectado ao SignalR com sucesso!'));
    </script>
</body>
</html>
", "text/html")).ExcludeFromDescription();

// 5. Portal Administrativo (O "Front" do Fullstack)
app.MapGet("/portal", () => Results.Content(@"
<!DOCTYPE html>
<html lang='pt-BR'>
<head>
    <meta charset='UTF-8'>
    <title>Portal Admin - Frota 🚜</title>
    <link href='https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css' rel='stylesheet'>
    <style>
        body { background-color: #f8f9fa; padding: 20px; }
        .card-trator { transition: transform 0.2s; }
        .card-trator:hover { transform: scale(1.02); }
        .status-Operacional { color: #198754; font-weight: bold; }
        .status-Alugado { color: #0dcaf0; font-weight: bold; }
        .status-EmManutencao { color: #dc3545; font-weight: bold; }
    </style>
</head>
<body>
    <div class='container'>
        <h2 class='mb-4'>Dashboard da Frota</h2>
        <div class='row' id='frota-container'>
            </div>
    </div>

    <script>
        // Consome a sua API (Dapper) assim que a tela abre
        fetch('/api/tratores/dashboard')
            .then(response => response.json())
            .then(tratores => {
                const container = document.getElementById('frota-container');
                
                if(tratores.length === 0) {
                    container.innerHTML = '<p>Nenhum trator cadastrado na base de dados.</p>';
                    return;
                }

                tratores.forEach(t => {
                    const card = document.createElement('div');
                    card.className = 'col-md-4 mb-4';
                    
                    // Formata as métricas
                    const temp = Number(t.temperaturaAtualMotor).toFixed(1);
                    const pressao = Number(t.pressaoAtualPneus).toFixed(1);
                    const oleo = Number(t.nivelOleo).toFixed(1);

                    card.innerHTML = `
                        <div class='card shadow-sm card-trator'>
                            <div class='card-body'>
                                <h5 class='card-title'>🚜 ${t.modelo}</h5>
                                <p class='card-text status-${t.status}'>Status: ${t.status}</p>
                                <hr>
                                <p class='mb-1'><strong>Temperatura:</strong> ${temp} ºC</p>
                                <p class='mb-1'><strong>Pressão Pneus:</strong> ${pressao} PSI</p>
                                <p class='mb-1'><strong>Nível Óleo:</strong> ${oleo}%</p>
                                <p class='mb-0'><strong>RPM:</strong> ${t.rotacaoMotor} | <strong>Velocidade:</strong> ${t.velocidade} km/h</p>
                            </div>
                        </div>
                    `;
                    container.appendChild(card);
                });
            });
    </script>
</body>
</html>
", "text/html")).ExcludeFromDescription();

// Executa as Migrations automaticamente ao subir a API
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<TractorRentalDbContext>();
    dbContext.Database.Migrate();
}

app.Run();