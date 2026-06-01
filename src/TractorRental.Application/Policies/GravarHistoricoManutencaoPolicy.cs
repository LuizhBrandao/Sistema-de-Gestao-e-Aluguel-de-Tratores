using MediatR;
using TractorRental.Application.Interfaces;
using TractorRental.Domain.Aggregates;
using TractorRental.Domain.Events;

namespace TractorRental.Application.Policies;

// Agora injetamos a Interface (Abstração) em vez do DbContext (Implementação concreta)
public class GravarHistoricoManutencaoPolicy(IRegistroManutencaoRepository repository) : INotificationHandler<AlertaGeradoEvent>
{
    public async Task Handle(AlertaGeradoEvent notification, CancellationToken cancellationToken)
    {
        var historico = new RegistroManutencao(
            Guid.NewGuid(),
            notification.TratorId,
            $"Manutenção Automática IoT: {notification.Motivo} (Criticidade: {notification.Criticidade})"
        );

        // Salva utilizando o repositório
        await repository.SalvarAsync(historico, cancellationToken);
    }
}