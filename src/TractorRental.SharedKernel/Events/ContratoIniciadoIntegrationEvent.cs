using MediatR;

namespace TractorRental.SharedKernel.Events;

/// <summary>
/// Evento de integração: Locação → Frota.
/// Disparado quando um novo contrato de aluguel é aberto.
/// O BC de Frota reage marcando o Trator como "Alugado".
/// </summary>
public record ContratoIniciadoIntegrationEvent(
    Guid ContratoId,
    Guid TratorId,
    Guid ClienteId,
    DateTime DataInicio
) : INotification;
