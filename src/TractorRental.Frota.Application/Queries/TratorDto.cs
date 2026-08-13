namespace TractorRental.Frota.Application.Queries;

public record TratorDto(
    Guid Id,
    string Modelo,
    string Status,
    double TemperaturaAtualMotor,
    double PressaoAtualPneus,
    double NivelCombustivel,
    double NivelOleo,
    double RotacaoMotor,
    double Velocidade
);
