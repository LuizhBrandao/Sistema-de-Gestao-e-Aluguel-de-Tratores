// Em TractorRental.Application/Interfaces/IClienteRepository.cs
using TractorRental.Domain.Aggregates;

namespace TractorRental.Application.Interfaces;

public interface IClienteRepository
{
    Task<Cliente?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken);
    Task SalvarAsync(Cliente cliente, CancellationToken cancellationToken);
    Task AtualizarAsync(Cliente cliente, CancellationToken cancellationToken);
    Task DeletarAsync(Guid id, CancellationToken cancellationToken);
}