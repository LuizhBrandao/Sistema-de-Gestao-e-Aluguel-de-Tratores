using Microsoft.EntityFrameworkCore;
using TractorRental.Application.Interfaces;
using TractorRental.Domain.Aggregates;

namespace TractorRental.Infrastructure.Data.Repositories;

public class ClienteRepository(TractorRentalDbContext context) : IClienteRepository
{
    public async Task<Cliente?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await context.Set<Cliente>().FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task SalvarAsync(Cliente cliente, CancellationToken cancellationToken)
    {
        await context.Set<Cliente>().AddAsync(cliente, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task AtualizarAsync(Cliente cliente, CancellationToken cancellationToken)
    {
        context.Set<Cliente>().Update(cliente);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeletarAsync(Guid id, CancellationToken cancellationToken)
    {
        var cliente = await ObterPorIdAsync(id, cancellationToken);
        if (cliente is not null)
        {
            context.Set<Cliente>().Remove(cliente);
            await context.SaveChangesAsync(cancellationToken);
        }
    }
}