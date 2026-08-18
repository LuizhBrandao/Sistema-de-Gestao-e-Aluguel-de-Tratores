using MassTransit;
using TractorRental.Telemetria.Application.Commands;
using TractorRental.Frota.Infrastructure;
using TractorRental.IoTWorker;
using TractorRental.IoTWorker.Consumers;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();

// Bounded Contexts
builder.Services.AddFrotaInfrastructure(builder.Configuration);

// MediatR: Registra handlers de Telemetria + Frota (para as policies reagirem)
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(
    typeof(RegistrarTelemetriaCommand).Assembly,
    typeof(TractorRental.Frota.Application.Policies.AtualizarTelemetriaPolicy).Assembly
));

builder.Services.AddHostedService<Worker>();

// MassTransit + RabbitMQ
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<TelemetriaConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        var rabbitHost = builder.Configuration.GetConnectionString("rabbitmq") ?? "amqp://guest:guest@localhost:5672";

        cfg.Host(new Uri(rabbitHost));

        cfg.ReceiveEndpoint("telemetria-tratores", e =>
        {
            e.ConfigureConsumer<TelemetriaConsumer>(context);
        });
    });
});

var host = builder.Build();
host.Run();