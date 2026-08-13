using MediatR;
using Microsoft.EntityFrameworkCore;
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
        group.MapPost("/", async (CriarContratoRequest request, LocacaoDbContext locacaoDb, FrotaDbContext frotaDb, IMediator mediator) =>
        {
            // Validação 1: O cliente existe? (BC: Locação)
            var clienteExiste = await locacaoDb.Clientes.AnyAsync(c => c.Id == request.ClienteId);
            if (!clienteExiste)
                return Results.BadRequest(new { Erro = "Cliente não encontrado." });

            // Validação 2: O trator existe e está disponível? (BC: Frota)
            var trator = await frotaDb.Tratores.FindAsync(request.TratorId);
            if (trator is null)
                return Results.BadRequest(new { Erro = "Trator não encontrado." });

            if (trator.Status != StatusTrator.Operacional)
                return Results.BadRequest(new { Erro = $"Trator indisponível para aluguel. Status atual: {trator.Status}" });

            // Cria o contrato (BC: Locação — gera ContratoIniciadoIntegrationEvent internamente)
            var contrato = new ContratoAluguel(Guid.NewGuid(), request.ClienteId, request.TratorId, request.ValorHora);

            locacaoDb.ContratosAluguel.Add(contrato);
            await locacaoDb.SaveChangesAsync();

            // Dispara Integration Events via MediatR (SharedKernel → Frota reage)
            var eventos = contrato.DomainEvents.ToList();
            contrato.LimparEventos();

            foreach (var domainEvent in eventos)
            {
                await mediator.Publish(domainEvent);
            }

            return Results.Created($"/api/contratos/{contrato.Id}", contrato);
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