using TractorRental.Frota.Application.Interfaces;
using TractorRental.Frota.Domain.Aggregates;

namespace TractorRental.Frota.Infrastructure.Data.Repositories;

public class RegistroManutencaoRepository(FrotaDbContext context) : IRegistroManutencaoRepository
{
    public async Task SalvarAsync(RegistroManutencao registro, CancellationToken cancellationToken)
    {
        await context.RegistrosManutencao.AddAsync(registro, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }
}
