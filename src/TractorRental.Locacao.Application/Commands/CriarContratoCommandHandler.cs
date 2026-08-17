using MediatR;
using TractorRental.Locacao.Application.Interfaces;
using TractorRental.Locacao.Domain.Aggregates;

namespace TractorRental.Locacao.Application.Commands;

public class CriarContratoCommandHandler(
    IClienteRepository clienteRepository,
    IContratoRepository contratoRepository,
    ITratorLocacaoAcl tratorLocacaoAcl,
    IMediator mediator) : IRequestHandler<CriarContratoCommand, Guid>
{
    public async Task<Guid> Handle(CriarContratoCommand request, CancellationToken cancellationToken)
    {
        var cliente = await clienteRepository.ObterPorIdAsync(request.ClienteId, cancellationToken);
        if (cliente is null)
        {
            throw new ArgumentException("Cliente não encontrado.");
        }

        var tratorExiste = await tratorLocacaoAcl.TratorExisteAsync(request.TratorId, cancellationToken);
        if (!tratorExiste)
        {
            throw new ArgumentException("Trator não encontrado.");
        }

        var isTratorOperacional = await tratorLocacaoAcl.IsTratorOperacionalAsync(request.TratorId, cancellationToken);

        var contrato = new ContratoAluguel(
            Guid.NewGuid(), 
            request.ClienteId, 
            request.TratorId, 
            request.ValorHora, 
            isTratorOperacional
        );

        await contratoRepository.SalvarAsync(contrato, cancellationToken);

        // Dispara Integration Events via MediatR (SharedKernel -> Frota reage)
        var eventos = contrato.DomainEvents.ToList();
        contrato.LimparEventos();

        foreach (var domainEvent in eventos)
        {
            await mediator.Publish(domainEvent, cancellationToken);
        }

        return contrato.Id;
    }
}
