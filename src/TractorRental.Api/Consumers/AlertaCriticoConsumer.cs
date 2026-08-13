using MassTransit;
using Microsoft.AspNetCore.SignalR;
using TractorRental.Api.Hubs;
using TractorRental.SharedKernel.Contracts;

namespace TractorRental.Api.Consumers;

public class AlertaCriticoConsumer(
    IHubContext<MonitoramentoHub> hubContext,
    ILogger<AlertaCriticoConsumer> logger) : IConsumer<AlertaCriticoMessage>
{
    public async Task Consume(ConsumeContext<AlertaCriticoMessage> context)
    {
        var alerta = context.Message;
        logger.LogWarning("🚨 Alerta do RabbitMQ recebido na API. Notificando Frontend: Trator {Id}", alerta.TratorId);

        // O SignalR dispara o evento "ReceberAlerta" para TODOS os navegadores conectados
        await hubContext.Clients.All.SendAsync("ReceberAlerta", alerta);
    }
}