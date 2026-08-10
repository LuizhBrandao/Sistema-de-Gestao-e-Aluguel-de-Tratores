namespace TractorRental.Application.Queries;

// Um objeto leve e imutável apenas para jogar na tela do portal
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