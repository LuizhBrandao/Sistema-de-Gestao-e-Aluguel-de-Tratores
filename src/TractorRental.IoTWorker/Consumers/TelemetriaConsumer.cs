using MassTransit;
using MediatR;
using TractorRental.Application.Commands;
using TractorRental.Messages;

namespace TractorRental.IoTWorker.Consumers;

public class TelemetriaConsumer(
    IMediator mediator,
    ILogger<TelemetriaConsumer> logger) : IConsumer<TelemetriaMessage>
{
    public async Task Consume(ConsumeContext<TelemetriaMessage> context)
    {
        var mensagem = context.Message;

        // 1. INSPEÇÃO: Loga tudo que chega, para sabermos se o RabbitMQ está entregando
        logger.LogInformation("🔍 Mensagem recebida no Consumer: Trator {Id}, Temp: {Temp}ºC",
            mensagem.TratorId, mensagem.TemperaturaMotor);

        try
        {
            // Passo 2. TENTA SALVAR NO BANCO
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
                logger.LogInformation("✅ Telemetria do trator {TratorId} salva.", mensagem.TratorId);
            }
            else
            {
                logger.LogWarning("⚠️ Falha ao salvar telemetria no banco (Regra de negócio impediu).");
            }

            // 3. O ALERTA É INDEPENDENTE: Valida TODOS os sensores para o Painel em Tempo Real
            var alertas = new List<string>();

            if (mensagem.TemperaturaMotor > 110.0)
                alertas.Add($"Motor superaquecendo");

            if (mensagem.PressaoPneus < 26.0)
                alertas.Add($"Pressão dos pneus baixa ({mensagem.PressaoPneus:F1} PSI)");

            if (mensagem.NivelOleo < 15.0)
                alertas.Add($"Óleo crítico ({mensagem.NivelOleo:F1}%)");

            if (mensagem.RotacaoMotor > 3500 && mensagem.Velocidade < 10)
                alertas.Add("Falha na transmissão (RPM muito alto)");

            // Se existir pelo menos um alerta, dispara para o frontend!
            if (alertas.Any())
            {
                var mensagemUnificada = string.Join(" | ", alertas);
                logger.LogWarning("🔥 Anomalia múltipla detectada! Disparando alerta via RabbitMQ...");

                var endpoint = await context.GetSendEndpoint(new Uri("queue:alertas-frontend"));

                await endpoint.Send(new AlertaCriticoMessage(
                    mensagem.TratorId,
                    mensagem.TemperaturaMotor, // Mantemos a temperatura para o ecrã não dar erro
                    mensagemUnificada          // A nova mensagem com todos os defeitos juntos!
                ));
            }
        }
        catch (Exception ex)
        {
            // 4. ARMADILHA DE ERRO: Se o código explodir por qualquer motivo, saberemos aqui
            logger.LogError("❌ ERRO CRÍTICO AO PROCESSAR {Temp}ºC: {Erro}", mensagem.TemperaturaMotor, ex.Message);
            throw; // Relança para o MassTransit tentar novamente se necessário
        }
    }
}