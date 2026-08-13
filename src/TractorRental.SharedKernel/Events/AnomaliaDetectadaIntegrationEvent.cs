using MediatR;

namespace TractorRental.SharedKernel.Events;

/// <summary>
/// Evento de integração: Telemetria → Frota.
/// Disparado quando o BC de Telemetria detecta uma anomalia nos sensores.
/// O BC de Frota reage colocando o Trator em manutenção e criando um RegistroManutencao.
/// </summary>
public record AnomaliaDetectadaIntegrationEvent(
    Guid TratorId,
    string Motivo,
    string Criticidade,
    DateTime Timestamp
) : INotification;
