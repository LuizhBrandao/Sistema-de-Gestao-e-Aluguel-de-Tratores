using MediatR;

namespace TractorRental.Application.Commands;

// O IRequest<bool> indica que este comando retornará um booleano de sucesso/falha
public record RegistrarTelemetriaCommand(
    Guid TratorId,
    double TemperaturaMotor,
    double PressaoPneus,
    double NivelCombustivel,
    double NivelOleo,
    double RotacaoMotor,
    double Velocidade
) : IRequest<bool>;