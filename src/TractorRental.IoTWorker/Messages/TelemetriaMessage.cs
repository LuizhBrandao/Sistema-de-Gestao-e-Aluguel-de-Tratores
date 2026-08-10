namespace TractorRental.Messages; // <-- A mesma etiqueta mágica que colocamos no Worker

public record TelemetriaMessage(
    Guid TratorId,
    double TemperaturaMotor,
    double PressaoPneus,
    double NivelCombustivel,
    double NivelOleo,
    double RotacaoMotor,
    double Velocidade
);