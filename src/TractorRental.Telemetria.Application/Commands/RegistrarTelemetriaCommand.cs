using MediatR;

namespace TractorRental.Telemetria.Application.Commands;

public record RegistrarTelemetriaCommand(
    Guid TratorId,
    double TemperaturaMotor,
    double PressaoPneus,
    double NivelCombustivel,
    double NivelOleo,
    double RotacaoMotor,
    double Velocidade
) : IRequest<bool>;
