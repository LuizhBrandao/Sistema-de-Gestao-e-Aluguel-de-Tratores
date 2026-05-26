using MediatR;

namespace TractorRental.Domain.Events;

public record LeituraRecebidaEvent(
    Guid TratorId,
    double TemperaturaMotor,
    double PressaoPneus,
    double NivelCombustivel,
    DateTime Timestamp
) : INotification;

public record AlertaGeradoEvent(
    Guid TratorId,
    string Motivo,
    string Criticidade,
    DateTime Timestamp
) : INotification;

public record ContratoIniciadoEvent(
    Guid ContratoId,
    Guid TratorId,
    Guid ClienteId,
    DateTime DataInicio
) : INotification;