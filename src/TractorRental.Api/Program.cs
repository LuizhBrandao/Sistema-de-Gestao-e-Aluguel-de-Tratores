using MassTransit;
using TractorRental.Api.Consumers;
using TractorRental.Api.Endpoints;
using TractorRental.Api.Hubs;
using TractorRental.Api.Services;
// Bounded Contexts
using TractorRental.Locacao.Infrastructure;
using TractorRental.Frota.Infrastructure;
using TractorRental.Frota.Infrastructure.Data;
using TractorRental.Locacao.Infrastructure.Data;
using TractorRental.Telemetria.Infrastructure;
using TractorRental.SharedKernel.Contracts;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

// ===== Bounded Contexts: Injeção de Dependências =====
builder.Services.AddLocacaoInfrastructure(builder.Configuration);
builder.Services.AddFrotaInfrastructure(builder.Configuration);
builder.Services.AddTelemetriaInfrastructure();

// MediatR: Registra handlers de TODOS os BCs
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(
    typeof(TractorRental.Telemetria.Application.Commands.RegistrarTelemetriaCommand).Assembly,
    typeof(TractorRental.Frota.Application.Policies.AtualizarTelemetriaPolicy).Assembly
));

// CORS Configuration
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", policy =>
    {
        policy.WithOrigins("http://localhost:5173", "http://127.0.0.1:5173", "http://localhost")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// SignalR
builder.Services.AddSignalR();

// Simulador de sensores (Background Service)
builder.Services.AddHostedService<SensorSimulatorWorker>();

// MassTransit + RabbitMQ
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<AlertaCriticoConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        var rabbitHost = builder.Configuration["RabbitHost"] ?? "localhost";

        cfg.Host(rabbitHost, "/", h => {
            h.Username("guest");
            h.Password("guest");
        });

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
app.UseCors("CorsPolicy");

// Endpoints organizados por Bounded Context
app.MapTratorEndpoints();    // BC: Frota
app.MapClienteEndpoints();   // BC: Locação
app.MapContratoEndpoints();  // BC: Locação + Frota (cross-BC via Integration Events)

// SignalR Hub
app.MapHub<MonitoramentoHub>("/hubs/monitoramento");

// Startup: Garante que o banco está acessível (tabelas já existem das migrations anteriores)
using (var scope = app.Services.CreateScope())
{
    try
    {
        var frotaDb = scope.ServiceProvider.GetRequiredService<FrotaDbContext>();
        frotaDb.Database.Migrate();
    }
    catch (Exception ex)
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("DatabaseMigration");
        logger.LogWarning(ex, "Migration do contexto Frota falhou. As tabelas podem já existir.");
    }

    try
    {
        var locacaoDb = scope.ServiceProvider.GetRequiredService<LocacaoDbContext>();
        locacaoDb.Database.Migrate();
    }
    catch (Exception ex)
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("DatabaseMigration");
        logger.LogWarning(ex, "Migration do contexto Locação falhou. As tabelas podem já existir.");
    }
}

app.Run();