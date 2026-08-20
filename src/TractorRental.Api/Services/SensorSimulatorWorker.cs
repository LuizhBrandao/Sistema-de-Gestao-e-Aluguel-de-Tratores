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
            await Task.Delay(10000, stoppingToken);

            try
            {
                using var scope = scopeFactory.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<FrotaDbContext>();
                var bus = scope.ServiceProvider.GetRequiredService<IBus>();

                var tratores = dbContext.Tratores.ToList();

                foreach (var trator in tratores)
                {
                    var random = new Random();
                    // Chance baixa de anomalia crítica (~4%)
                    bool simularPicoCritico = random.Next(1, 100) <= 4;

                    double temp = simularPicoCritico && random.Next(2) == 0
                        ? 111.0 + (random.NextDouble() * 5.0)  // Superaquecimento > 110°C
                        : 82.0 + (random.NextDouble() * 15.0); // Normal: 82°C a 97°C

                    double pressao = simularPicoCritico && random.Next(2) == 0
                        ? 22.0 + (random.NextDouble() * 3.5)  // Pressão baixa < 26 PSI
                        : 29.0 + (random.NextDouble() * 5.0);  // Normal: 29 a 34 PSI

                    double oleo = simularPicoCritico && random.Next(2) == 0
                        ? 8.0 + (random.NextDouble() * 6.0)   // Óleo baixo < 15%
                        : 70.0 + (random.NextDouble() * 25.0); // Normal: 70% a 95%

                    var telemetria = new TelemetriaMessage(
                        trator.Id,
                        temp,
                        pressao,
                        random.Next(25, 100),
                        oleo,
                        random.Next(1200, 2400),
                        random.Next(5, 30)
                    );

                    await bus.Publish(telemetria, stoppingToken);

                    logger.LogInformation("📡 Telemetria enviada para o Trator {TratorId} | Temp: {Temp:F1}ºC | Óleo: {Oleo:F1}%", trator.Id, telemetria.TemperaturaMotor, telemetria.NivelOleo);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Erro ao simular telemetria.");
            }
        }
    }
}