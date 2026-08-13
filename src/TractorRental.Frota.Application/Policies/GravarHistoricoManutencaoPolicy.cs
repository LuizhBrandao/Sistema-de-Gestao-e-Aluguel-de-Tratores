using MediatR;
using TractorRental.Frota.Application.Interfaces;
using TractorRental.Frota.Domain.Aggregates;
using TractorRental.SharedKernel.Events;

namespace TractorRental.Frota.Application.Policies;

/// <summary>
/// Escuta AnomaliaDetectadaIntegrationEvent (do BC Telemetria via SharedKernel)
/// e cria um RegistroManutencao + coloca o Trator em manutenção.
/// </summary>
public class GravarHistoricoManutencaoPolicy(
    IRegistroManutencaoRepository repository,
    ITratorRepository tratorRepository) : INotificationHandler<AnomaliaDetectadaIntegrationEvent>
{
    public async Task Handle(AnomaliaDetectadaIntegrationEvent notification, CancellationToken cancellationToken)
    {
        // 1. Cria o registro de manutenção
        var historico = new RegistroManutencao(
            Guid.NewGuid(),
            notification.TratorId,
            $"Manutenção Automática IoT: {notification.Motivo} (Criticidade: {notification.Criticidade})"
        );

        await repository.SalvarAsync(historico, cancellationToken);

        // 2. Coloca o trator em manutenção
        var trator = await tratorRepository.ObterPorIdAsync(notification.TratorId, cancellationToken);
        if (trator is not null)
        {
            trator.RegistrarAlertaManutencao($"Risco Crítico: {notification.Motivo}");
            trator.LimparEventos(); // Limpa os eventos internos pois já reagimos
            await tratorRepository.AtualizarAsync(trator, cancellationToken);
        }
    }
}
