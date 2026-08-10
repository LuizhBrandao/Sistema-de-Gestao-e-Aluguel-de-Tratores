using MassTransit;
using TractorRental.Api.Consumers; // <-- Novo
using TractorRental.Api.Endpoints;
using TractorRental.Api.Hubs; // <-- Novo
using TractorRental.Application.Commands;
using TractorRental.Infrastructure;

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
        cfg.Host("localhost", "/", h => {
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
app.UseCors("CorsPolicy");
app.MapTratorEndpoints();

// 3. Mapeia a URL do SignalR
app.MapHub<MonitoramentoHub>("/hubs/monitoramento");


app.Run();