using MediatR;
using TractorRental.Application.Interfaces;
using TractorRental.Domain.Enums;
using TractorRental.Domain.Events;

namespace TractorRental.Application.Policies;

public class AtualizarStatusTratorAlugadoPolicy(
    ITratorRepository tratorRepository) : INotificationHandler<ContratoIniciadoEvent>
{
    public async Task Handle(ContratoIniciadoEvent notification, CancellationToken cancellationToken)
    {
        var trator = await tratorRepository.ObterPorIdAsync(notification.TratorId, cancellationToken);

        if (trator is not null)
        {
            // Executa a regra de transição interna do Aggregate Root
            trator.Alugar();

            // Persiste o novo estado do trator alugado
            await tratorRepository.AtualizarAsync(trator, cancellationToken);
        }
    }
}