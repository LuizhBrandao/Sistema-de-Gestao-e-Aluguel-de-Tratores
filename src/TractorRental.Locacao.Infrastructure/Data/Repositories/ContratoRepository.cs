using Microsoft.EntityFrameworkCore;
using TractorRental.Locacao.Application.Interfaces;
using TractorRental.Locacao.Domain.Aggregates;

namespace TractorRental.Locacao.Infrastructure.Data.Repositories;

public class ContratoRepository(LocacaoDbContext context) : IContratoRepository
{
    public async Task<ContratoAluguel?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await context.ContratosAluguel.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task SalvarAsync(ContratoAluguel contrato, CancellationToken cancellationToken)
    {
        await context.ContratosAluguel.AddAsync(contrato, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }
}
