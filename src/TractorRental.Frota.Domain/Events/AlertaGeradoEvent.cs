using MediatR;

namespace TractorRental.Frota.Domain.Events;

/// <summary>
/// Evento INTERNO do BC de Frota.
/// Disparado quando o Trator entra em manutenção.
/// A GravarHistoricoManutencaoPolicy escuta para criar o RegistroManutencao.
/// </summary>
public record AlertaGeradoEvent(
    Guid TratorId,
    string Motivo,
    string Criticidade,
    DateTime Timestamp
) : INotification;
