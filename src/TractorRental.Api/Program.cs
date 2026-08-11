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
app.UseStaticFiles();
app.UseCors("CorsPolicy");

// As nossas 3 "gavetas" de endpoints organizadas
app.MapTratorEndpoints();
app.MapClienteEndpoints();   
app.MapContratoEndpoints();  

// 3. Mapeia a URL do SignalR
app.MapHub<MonitoramentoHub>("/hubs/monitoramento");

// 4. Redireciona rotas desconhecidas para o index.html (SPA fallback)
app.MapFallbackToFile("index.html");

// Executa as Migrations automaticamente ao subir a API
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<TractorRentalDbContext>();
    try
    {
        dbContext.Database.Migrate();
    }
    catch (Exception ex)
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("DatabaseMigration");
        logger.LogWarning(ex, "Erro ao executar migrations. O banco de dados pode já existir. Verificando compatibilidade...");

        // Garante que o banco está acessível mesmo que as migrations falhem
        if (!dbContext.Database.CanConnect())
        {
            throw; // Se não consegue nem conectar, algo está realmente errado
        }
    }
}

app.Run();