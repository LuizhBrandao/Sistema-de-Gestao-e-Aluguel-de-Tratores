using TractorRental.Frota.Domain.Aggregates;

namespace TractorRental.Frota.Application.Interfaces;

public interface ITratorRepository
{
    Task<Trator?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken);
    Task SalvarAsync(Trator trator, CancellationToken cancellationToken);
    Task AtualizarAsync(Trator trator, CancellationToken cancellationToken);
}
