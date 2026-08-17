using MediatR;
using Microsoft.EntityFrameworkCore;
using TractorRental.Locacao.Application.Commands;
using TractorRental.Locacao.Domain.Aggregates;
using TractorRental.Locacao.Infrastructure.Data;
using TractorRental.Frota.Domain.Enums;
using TractorRental.Frota.Infrastructure.Data;

namespace TractorRental.Api.Endpoints;

public static class ContratoEndpoints
{
    public static void MapContratoEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/contratos").WithTags("Gestão de Contratos de Aluguel");

        // 1. Alugar um Trator — Coordenação Cross-BC (Locação + Frota)
        group.MapPost("/", async (CriarContratoRequest request, IMediator mediator) =>
        {
            var command = new CriarContratoCommand(
                request.ClienteId,
                request.TratorId,
                request.ValorHora
            );

            try
            {
                var contratoId = await mediator.Send(command);
                return Results.Created($"/api/contratos/{contratoId}", new { Id = contratoId });
            }
            catch (ArgumentException ex)
            {
                return Results.BadRequest(new { Erro = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Results.BadRequest(new { Erro = ex.Message });
            }
        })
        .WithSummary("Abre um novo contrato de aluguel e atualiza o status do equipamento");

        // 2. Listar Contratos (BC: Locação)
        group.MapGet("/", async (LocacaoDbContext locacaoDb) =>
        {
            var contratos = await locacaoDb.ContratosAluguel.ToListAsync();
            return Results.Ok(contratos);
        })
        .WithSummary("Lista todo o histórico de contratos da empresa");
    }
}

public record CriarContratoRequest(Guid ClienteId, Guid TratorId, decimal ValorHora);