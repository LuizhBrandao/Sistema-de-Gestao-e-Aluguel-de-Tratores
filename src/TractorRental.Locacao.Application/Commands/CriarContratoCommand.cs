using MediatR;

namespace TractorRental.Locacao.Application.Commands;

public record CriarContratoCommand(
    Guid ClienteId,
    Guid TratorId,
    decimal ValorHora
) : IRequest<Guid>;
