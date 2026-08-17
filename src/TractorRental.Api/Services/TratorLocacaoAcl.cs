using Microsoft.EntityFrameworkCore;
using TractorRental.Frota.Domain.Enums;
using TractorRental.Frota.Infrastructure.Data;
using TractorRental.Locacao.Application.Interfaces;

namespace TractorRental.Api.Services;

public class TratorLocacaoAcl(FrotaDbContext frotaDb) : ITratorLocacaoAcl
{
    public async Task<bool> IsTratorOperacionalAsync(Guid tratorId, CancellationToken cancellationToken)
    {
        var trator = await frotaDb.Tratores
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == tratorId, cancellationToken);
            
        return trator?.Status == StatusTrator.Operacional;
    }

    public async Task<bool> TratorExisteAsync(Guid tratorId, CancellationToken cancellationToken)
    {
        return await frotaDb.Tratores.AnyAsync(t => t.Id == tratorId, cancellationToken);
    }
}
