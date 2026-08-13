using TractorRental.Locacao.Domain.Aggregates;

namespace TractorRental.Locacao.Application.Interfaces;

public interface IContratoRepository
{
    Task<ContratoAluguel?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken);
    Task SalvarAsync(ContratoAluguel contrato, CancellationToken cancellationToken);
}
