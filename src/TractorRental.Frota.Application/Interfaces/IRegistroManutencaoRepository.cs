using TractorRental.Frota.Domain.Aggregates;

namespace TractorRental.Frota.Application.Interfaces;

public interface IRegistroManutencaoRepository
{
    Task SalvarAsync(RegistroManutencao registro, CancellationToken cancellationToken);
}
