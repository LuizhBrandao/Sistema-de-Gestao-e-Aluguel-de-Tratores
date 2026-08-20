using MassTransit;
using TractorRental.Frota.Infrastructure.Data;
using TractorRental.SharedKernel.Contracts;
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
            await Task.Delay(10000, stoppingToken);

            try
            {
                using var scope = scopeFactory.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<FrotaDbContext>();
                var bus = scope.ServiceProvider.GetRequiredService<IBus>();

                var tratores = dbContext.Tratores.ToList();

                foreach(var trator in tratores)
                {
                    var random = new Random();
                    bool simularPicoCritico = random.Next(1, 100) <= 4;

                    double temp = simularPicoCritico && random.Next(2) == 0
                        ? 111.0 + (random.NextDouble() * 5.0)
                        : 82.0 + (random.NextDouble() * 15.0);

                    double pressao = simularPicoCritico && random.Next(2) == 0
                        ? 22.0 + (random.NextDouble() * 3.5)
                        : 29.0 + (random.NextDouble() * 5.0);

                    double oleo = simularPicoCritico && random.Next(2) == 0
                        ? 8.0 + (random.NextDouble() * 6.0)
                        : 70.0 + (random.NextDouble() * 25.0);

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
