using MediatR;

namespace TractorRental.SharedKernel.Events;

/// <summary>
/// Evento de integração: Telemetria → Frota.
/// Disparado quando uma nova leitura de sensores é processada pelo BC de Telemetria.
/// O BC de Frota reage atualizando os valores de sensor no cache do Trator.
/// </summary>
public record TelemetriaProcessadaIntegrationEvent(
    Guid TratorId,
    double TemperaturaMotor,
    double PressaoPneus,
    double NivelCombustivel,
    double NivelOleo,
    double RotacaoMotor,
    double Velocidade,
    DateTime Timestamp
) : INotification;
