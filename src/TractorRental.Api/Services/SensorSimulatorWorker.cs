using MassTransit;
using TractorRental.Frota.Infrastructure.Data;
using TractorRental.SharedKernel.Contracts;

namespace TractorRental.Api.Services;

public class SensorSimulatorWorker(
    IServiceScopeFactory scopeFactory,
    ILogger<SensorSimulatorWorker> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("🚜 Robô de simulação de sensores iniciado!");

        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(5000, stoppingToken);

            try
            {
                using var scope = scopeFactory.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<FrotaDbContext>();
                var bus = scope.ServiceProvider.GetRequiredService<IBus>();

                var trator = dbContext.Tratores.FirstOrDefault();

                if (trator is not null)
                {
                    var random = new Random();

                    var telemetria = new TelemetriaMessage(
                    trator.Id,
                    80.0 + (random.NextDouble() * 35.0),
                    25.0 + (random.NextDouble() * 10.0),
                    random.Next(5, 100),
                    10.0 + (random.NextDouble() * 90.0),
                    random.Next(800, 4000),
                    random.Next(0, 40)
);

                    await bus.Publish(telemetria);

                    logger.LogInformation("📡 Telemetria enviada para o Trator {TratorId} | Temp: {Temp:F1}ºC", trator.Id, telemetria.TemperaturaMotor);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Erro ao simular telemetria.");
            }
        }
    }
}