using MediatR;

namespace TractorRental.Frota.Application.Commands;

public record CadastrarTratorCommand(
    string Marca,
    string Modelo,
    int AnoFabricacao,
    int PotenciaCv,
    double HorimetroInicial,
    string NumeroSerie
) : IRequest<Guid>;
