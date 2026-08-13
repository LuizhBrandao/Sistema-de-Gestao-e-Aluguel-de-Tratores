using MassTransit;
using MediatR;
using TractorRental.Telemetria.Application.Commands;
using TractorRental.SharedKernel.Contracts;

namespace TractorRental.IoTWorker.Consumers;

public class TelemetriaConsumer(
    IMediator mediator,
    ILogger<TelemetriaConsumer> logger) : IConsumer<TelemetriaMessage>
{
    public async Task Consume(ConsumeContext<TelemetriaMessage> context)
    {
        var mensagem = context.Message;

        logger.LogInformation("🔍 Mensagem recebida no Consumer: Trator {Id}, Temp: {Temp}ºC",
            mensagem.TratorId, mensagem.TemperaturaMotor);

        try
        {
            // Despacha para o BC de Telemetria processar
            var command = new RegistrarTelemetriaCommand(
                mensagem.TratorId,
                mensagem.TemperaturaMotor,
                mensagem.PressaoPneus,
                mensagem.NivelCombustivel,
                mensagem.NivelOleo,
                mensagem.RotacaoMotor,
                mensagem.Velocidade
            );

            var sucesso = await mediator.Send(command);

            if (sucesso)
            {
                logger.LogInformation("✅ Telemetria do trator {TratorId} processada.", mensagem.TratorId);
            }
            else
            {
                logger.LogWarning("⚠️ Falha ao processar telemetria.");
            }

            // Detecção de anomalias para alertar o Frontend via RabbitMQ → SignalR
            var alertas = new List<string>();

            if (mensagem.TemperaturaMotor > 110.0)
                alertas.Add($"Motor superaquecendo");

            if (mensagem.PressaoPneus < 26.0)
                alertas.Add($"Pressão dos pneus baixa ({mensagem.PressaoPneus:F1} PSI)");

            if (mensagem.NivelOleo < 15.0)
                alertas.Add($"Óleo crítico ({mensagem.NivelOleo:F1}%)");

            if (mensagem.RotacaoMotor > 3500 && mensagem.Velocidade < 10)
                alertas.Add("Falha na transmissão (RPM muito alto)");

            if (alertas.Any())
            {
                var mensagemUnificada = string.Join(" | ", alertas);
                logger.LogWarning("🔥 Anomalia múltipla detectada! Disparando alerta via RabbitMQ...");

                var endpoint = await context.GetSendEndpoint(new Uri("queue:alertas-frontend"));

                await endpoint.Send(new AlertaCriticoMessage(
                    mensagem.TratorId,
                    mensagem.TemperaturaMotor,
                    mensagemUnificada
                ));
            }
        }
        catch (Exception ex)
        {
            logger.LogError("❌ ERRO CRÍTICO AO PROCESSAR {Temp}ºC: {Erro}", mensagem.TemperaturaMotor, ex.Message);
            throw;
        }
    }
}