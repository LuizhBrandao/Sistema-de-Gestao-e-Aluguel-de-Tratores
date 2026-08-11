using MassTransit;
using TractorRental.Infrastructure.Data;
using TractorRental.Messages;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace TractorRental.IoTWorker;

public class Worker(
    IServiceScopeFactory scopeFactory,
    ILogger<Worker> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("🚜 IoT Worker de simulação de sensores iniciado!");

        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(5000, stoppingToken);

            try
            {
                using var scope = scopeFactory.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<TractorRentalDbContext>();
                var bus = scope.ServiceProvider.GetRequiredService<IBus>();

                // In a real scenario, this worker would represent many edge devices.
                // Here we fetch all active tractors to simulate data for them.
                var tratores = dbContext.Tratores.ToList();

                foreach(var trator in tratores)
                {
                    var random = new Random();

                    var telemetria = new TelemetriaMessage(
                        trator.Id,
                        80.0 + (random.NextDouble() * 40.0), // Temp: 80 - 120
                        30.0 + (random.NextDouble() * 5.0),  // Pneu: 30 - 35
                        random.Next(10, 100)
                    );

                    await bus.Publish(telemetria, stoppingToken);
                    logger.LogInformation("📡 Telemetria enviada | Trator: {TratorId} | Temp: {Temp:F1}ºC", trator.Id, telemetria.TemperaturaMotor);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Erro ao simular telemetria no IoTWorker.");
            }
        }
    }
}
