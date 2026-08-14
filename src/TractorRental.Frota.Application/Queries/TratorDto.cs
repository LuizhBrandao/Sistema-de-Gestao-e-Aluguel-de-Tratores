namespace TractorRental.Frota.Application.Queries;

public record TratorDto(
    Guid Id,
    string Marca,
    string Modelo,
    int AnoFabricacao,
    int PotenciaCv,
    double HorimetroInicial,
    string NumeroSerie,
    string Status,
    double TemperaturaAtualMotor,
    double PressaoAtualPneus,
    double NivelCombustivel,
    double NivelOleo,
    double RotacaoMotor,
    double Velocidade
);
