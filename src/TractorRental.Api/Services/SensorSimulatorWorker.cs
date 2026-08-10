using MassTransit;
using TractorRental.Infrastructure.Data;
using TractorRental.Messages;

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
                var dbContext = scope.ServiceProvider.GetRequiredService<TractorRentalDbContext>();
                var bus = scope.ServiceProvider.GetRequiredService<IBus>();

                var trator = dbContext.Tratores.FirstOrDefault();

                if (trator is not null)
                {
                    var random = new Random();

                    var telemetria = new TelemetriaMessage(
                    trator.Id,
                    80.0 + (random.NextDouble() * 35.0), // Temp normal até 115
                    25.0 + (random.NextDouble() * 10.0), // Pressão entre 25 (crítico) e 35
                    random.Next(5, 100),                 // Combustível
                    10.0 + (random.NextDouble() * 90.0), // Óleo (pode cair pra 10%)
                    random.Next(800, 4000),              // RPM
                    random.Next(0, 40)                   // Velocidade (Tratores são lentos)
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