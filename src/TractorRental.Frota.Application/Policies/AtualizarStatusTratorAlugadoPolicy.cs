using MediatR;
using TractorRental.Frota.Application.Interfaces;
using TractorRental.SharedKernel.Events;

namespace TractorRental.Frota.Application.Policies;

/// <summary>
/// Escuta ContratoIniciadoIntegrationEvent (do BC Locação via SharedKernel)
/// e marca o Trator como Alugado no BC Frota.
/// </summary>
public class AtualizarStatusTratorAlugadoPolicy(
    ITratorRepository tratorRepository) : INotificationHandler<ContratoIniciadoIntegrationEvent>
{
    public async Task Handle(ContratoIniciadoIntegrationEvent notification, CancellationToken cancellationToken)
    {
        var trator = await tratorRepository.ObterPorIdAsync(notification.TratorId, cancellationToken);

        if (trator is not null)
        {
            trator.Alugar();
            await tratorRepository.AtualizarAsync(trator, cancellationToken);
        }
    }
}
