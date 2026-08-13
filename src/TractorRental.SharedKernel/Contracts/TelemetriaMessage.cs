namespace TractorRental.SharedKernel.Contracts;

/// <summary>
/// Contrato MassTransit (RabbitMQ) para transmissão de dados de telemetria entre serviços.
/// O SensorSimulatorWorker (API) e o IoTWorker publicam; o TelemetriaConsumer consome.
/// </summary>
public record TelemetriaMessage(
    Guid TratorId,
    double TemperaturaMotor,
    double PressaoPneus,
    double NivelCombustivel,
    double NivelOleo,
    double RotacaoMotor,
    double Velocidade
);
