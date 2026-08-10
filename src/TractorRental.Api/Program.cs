using MassTransit;
using TractorRental.Api.Consumers; 
using TractorRental.Api.Endpoints;
<<<<<<< HEAD
using TractorRental.Api.Hubs; // <-- Novo
=======
using TractorRental.Api.Hubs; 
using TractorRental.Api.Services;
>>>>>>> 8e0d6d9e8781febc2de2e6bd4ab63041d945357e
using TractorRental.Application.Commands;
using TractorRental.Infrastructure;
using Microsoft.EntityFrameworkCore;
using TractorRental.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(RegistrarTelemetriaCommand).Assembly));

// CORS Configuration
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", policy =>
    {
        policy.WithOrigins("http://localhost:5173", "http://127.0.0.1:5173", "http://localhost") // Added http://localhost for Docker Nginx
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials(); // Required for SignalR
    });
});

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


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options => options.SwaggerEndpoint("/openapi/v1.json", "Tractor Rental API"));
}

app.UseHttpsRedirection();
<<<<<<< HEAD
app.UseCors("CorsPolicy");
=======

// As nossas 3 "gavetas" de endpoints organizadas 
>>>>>>> 8e0d6d9e8781febc2de2e6bd4ab63041d945357e
app.MapTratorEndpoints();
app.MapClienteEndpoints();   
app.MapContratoEndpoints();  

// 3. Mapeia a URL do SignalR
app.MapHub<MonitoramentoHub>("/hubs/monitoramento");


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