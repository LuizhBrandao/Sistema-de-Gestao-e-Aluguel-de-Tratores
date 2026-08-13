using MediatR;
using TractorRental.SharedKernel.Events;

namespace TractorRental.Telemetria.Application.Commands;

/// <summary>
/// O BC de Telemetria é stateless. Ele recebe dados brutos, processa e publica
/// um Integration Event para que o BC de Frota atualize o Trator.
/// </summary>
public class RegistrarTelemetriaCommandHandler(
    IMediator mediator) : IRequestHandler<RegistrarTelemetriaCommand, bool>
{
    public async Task<bool> Handle(RegistrarTelemetriaCommand request, CancellationToken cancellationToken)
    {
        // 1. Publica o evento de telemetria processada para o BC de Frota atualizar o Trator
        await mediator.Publish(new TelemetriaProcessadaIntegrationEvent(
            request.TratorId,
            request.TemperaturaMotor,
            request.PressaoPneus,
            request.NivelCombustivel,
            request.NivelOleo,
            request.RotacaoMotor,
            request.Velocidade,
            DateTime.UtcNow
        ), cancellationToken);

        return true;
    }
}
