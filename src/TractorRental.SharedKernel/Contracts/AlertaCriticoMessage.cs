namespace TractorRental.SharedKernel.Contracts;

/// <summary>
/// Contrato MassTransit (RabbitMQ) para notificar o Frontend via SignalR sobre alertas críticos.
/// Usado pela fila "alertas-frontend".
/// </summary>
public record AlertaCriticoMessage(Guid TratorId, double Temperatura, string Mensagem);
