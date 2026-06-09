using MassTransit;
using TractorRental.Application.Commands;
using TractorRental.Infrastructure;
using TractorRental.IoTWorker.Consumers;

var builder = Host.CreateApplicationBuilder(args);

// 1. Liga as camadas da arquitetura
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(RegistrarTelemetriaCommand).Assembly));

// Configura o MassTransit para o Worker
builder.Services.AddMassTransit(x =>
{
    // 1. Registra o seu consumidor de telemetria
    x.AddConsumer<TelemetriaConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        // 👇 AQUI ESTÁ A MUDANÇA: Lê da variável de ambiente ou usa localhost
        var rabbitHost = builder.Configuration["RabbitHost"] ?? "localhost";

        cfg.Host(rabbitHost, "/", h => {
            h.Username("guest");
            h.Password("guest");
        });

        // 2. AQUI ESTÁ O SEGREDO: Você precisa configurar o endpoint 
        // para a fila que a API está usando para PUBLISH
        cfg.ReceiveEndpoint("telemetria-tratores", e =>
        {
            e.ConfigureConsumer<TelemetriaConsumer>(context);
        });
    });
});

var host = builder.Build();
host.Run();