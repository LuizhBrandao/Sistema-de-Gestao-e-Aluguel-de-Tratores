using Microsoft.EntityFrameworkCore;
using TractorRental.Locacao.Application.Interfaces;
using TractorRental.Locacao.Domain.Aggregates;

namespace TractorRental.Locacao.Infrastructure.Data.Repositories;

public class ClienteRepository(LocacaoDbContext context) : IClienteRepository
{
    public async Task<Cliente?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await context.Clientes.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task SalvarAsync(Cliente cliente, CancellationToken cancellationToken)
    {
        await context.Clientes.AddAsync(cliente, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task AtualizarAsync(Cliente cliente, CancellationToken cancellationToken)
    {
        context.Clientes.Update(cliente);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeletarAsync(Guid id, CancellationToken cancellationToken)
    {
        var cliente = await ObterPorIdAsync(id, cancellationToken);
        if (cliente is not null)
        {
            context.Clientes.Remove(cliente);
            await context.SaveChangesAsync(cancellationToken);
        }
    }
}
