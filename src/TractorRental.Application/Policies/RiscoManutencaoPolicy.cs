using MediatR;
using TractorRental.Application.Interfaces;
using TractorRental.Domain.Events;

namespace TractorRental.Application.Policies;

public class RiscoManutencaoPolicy(
    ITratorRepository repository,
    IMediator mediator) : INotificationHandler<LeituraRecebidaEvent>
{
    public async Task Handle(LeituraRecebidaEvent notification, CancellationToken cancellationToken)
    {
        // Centralizamos TODAS as regras de negócio aqui (Blindando o Domínio)
        var falhas = new List<string>();

        if (notification.TemperaturaMotor > 110.0)
            falhas.Add($"Motor superaquecendo ({notification.TemperaturaMotor:F1}°C)");

        if (notification.PressaoPneus < 26.0)
            falhas.Add($"Pressão baixa ({notification.PressaoPneus:F1} PSI)");

        if (notification.NivelOleo < 15.0)
            falhas.Add($"Óleo crítico ({notification.NivelOleo:F1}%)");

        if (notification.RotacaoMotor > 3500 && notification.Velocidade < 10)
            falhas.Add("Falha na transmissão (RPM alto)");

        // Se o domínio detectou qualquer anomalia física, bloqueia o trator
        if (falhas.Any())
        {
            var trator = await repository.ObterPorIdAsync(notification.TratorId, cancellationToken);

            if (trator is not null)
            {
                var motivoUnificado = string.Join(" | ", falhas);

                // Altera o status para EmManutencao e gera o AlertaGeradoEvent
                trator.RegistrarAlertaManutencao($"Risco Crítico: {motivoUnificado}");

                await repository.AtualizarAsync(trator, cancellationToken);

                // Dispara para a GravarHistoricoManutencaoPolicy salvar no banco
                var eventos = trator.DomainEvents.ToList();
                trator.LimparEventos();

                foreach (var domainEvent in eventos)
                {
                    await mediator.Publish(domainEvent, cancellationToken);
                }
            }
        }
    }
}