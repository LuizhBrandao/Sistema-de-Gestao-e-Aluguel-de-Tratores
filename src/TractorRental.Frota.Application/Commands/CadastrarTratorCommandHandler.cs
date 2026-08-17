using MediatR;
using TractorRental.Frota.Application.Interfaces;
using TractorRental.Frota.Domain.Aggregates;

namespace TractorRental.Frota.Application.Commands;

public class CadastrarTratorCommandHandler(
    ITratorRepository tratorRepository) : IRequestHandler<CadastrarTratorCommand, Guid>
{
    public async Task<Guid> Handle(CadastrarTratorCommand request, CancellationToken cancellationToken)
    {
        var trator = new Trator(
            Guid.NewGuid(),
            request.Marca,
            request.Modelo,
            request.AnoFabricacao,
            request.PotenciaCv,
            request.HorimetroInicial,
            request.NumeroSerie
        );

        await tratorRepository.SalvarAsync(trator, cancellationToken);

        return trator.Id;
    }
}
