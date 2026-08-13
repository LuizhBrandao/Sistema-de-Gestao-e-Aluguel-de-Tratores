using MediatR;
using TractorRental.Frota.Application.Interfaces;
using TractorRental.SharedKernel.Events;

namespace TractorRental.Frota.Application.Policies;

/// <summary>
/// Escuta TelemetriaProcessadaIntegrationEvent (do BC Telemetria via SharedKernel)
/// e atualiza os valores de sensor cacheados no Trator.
/// </summary>
public class AtualizarTelemetriaPolicy(
    ITratorRepository tratorRepository) : INotificationHandler<TelemetriaProcessadaIntegrationEvent>
{
    public async Task Handle(TelemetriaProcessadaIntegrationEvent notification, CancellationToken cancellationToken)
    {
        var trator = await tratorRepository.ObterPorIdAsync(notification.TratorId, cancellationToken);

        if (trator is not null)
        {
            trator.AtualizarSensores(
                notification.TemperaturaMotor,
                notification.PressaoPneus,
                notification.NivelCombustivel,
                notification.NivelOleo,
                notification.RotacaoMotor,
                notification.Velocidade
            );

            await tratorRepository.AtualizarAsync(trator, cancellationToken);
        }
    }
}
