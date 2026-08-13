using MediatR;
using TractorRental.SharedKernel.Events;

namespace TractorRental.Telemetria.Application.Policies;

/// <summary>
/// Política de detecção de anomalias. Escuta TelemetriaProcessadaIntegrationEvent
/// e analisa os thresholds dos sensores. Se detectar risco, publica AnomaliaDetectadaIntegrationEvent.
/// Esta policy pertence ao BC de Telemetria porque a lógica de detecção é responsabilidade
/// do departamento de monitoramento, não da gestão de frota.
/// </summary>
public class RiscoManutencaoPolicy(
    IMediator mediator) : INotificationHandler<TelemetriaProcessadaIntegrationEvent>
{
    public async Task Handle(TelemetriaProcessadaIntegrationEvent notification, CancellationToken cancellationToken)
    {
        var falhas = new List<string>();

        if (notification.TemperaturaMotor > 110.0)
            falhas.Add($"Motor superaquecendo ({notification.TemperaturaMotor:F1}°C)");

        if (notification.PressaoPneus < 26.0)
            falhas.Add($"Pressão baixa ({notification.PressaoPneus:F1} PSI)");

        if (notification.NivelOleo < 15.0)
            falhas.Add($"Óleo crítico ({notification.NivelOleo:F1}%)");

        if (notification.RotacaoMotor > 3500 && notification.Velocidade < 10)
            falhas.Add("Falha na transmissão (RPM alto)");

        if (falhas.Any())
        {
            var motivoUnificado = string.Join(" | ", falhas);

            // Publica Integration Event para o BC de Frota reagir
            await mediator.Publish(new AnomaliaDetectadaIntegrationEvent(
                notification.TratorId,
                $"Risco Crítico: {motivoUnificado}",
                "ALTA",
                DateTime.UtcNow
            ), cancellationToken);
        }
    }
}
