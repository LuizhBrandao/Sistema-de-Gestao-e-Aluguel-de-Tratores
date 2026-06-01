using TractorRental.Domain.Aggregates;

namespace TractorRental.Application.Interfaces;

public interface IRegistroManutencaoRepository
{
    Task SalvarAsync(RegistroManutencao registro, CancellationToken cancellationToken);
}