using MediatR;
using Microsoft.EntityFrameworkCore;
using TractorRental.Domain.Aggregates;
using TractorRental.Domain.Enums;
using TractorRental.Infrastructure.Data;

namespace TractorRental.Api.Endpoints;

public static class ContratoEndpoints
{
    public static void MapContratoEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/contratos").WithTags("Gestão de Contratos de Aluguel");

        // 1. Alugar um Trator (Abertura de Contrato)
        group.MapPost("/", async (CriarContratoRequest request, TractorRentalDbContext db, IMediator mediator) =>
        {
            // Validação 1: O cliente existe?
            var clienteExiste = await db.Clientes.AnyAsync(c => c.Id == request.ClienteId);
            if (!clienteExiste)
                return Results.BadRequest(new { Erro = "Cliente não encontrado." });

            // Validação 2: O trator existe e está disponível?
            var trator = await db.Tratores.FindAsync(request.TratorId);
            if (trator is null)
                return Results.BadRequest(new { Erro = "Trator não encontrado." });

            if (trator.Status != StatusTrator.Operacional)
                return Results.BadRequest(new { Erro = $"Trator indisponível para aluguel. Status atual: {trator.Status}" });

            // Cria o contrato (O Agregado cria o evento 'ContratoIniciadoEvent' internamente)
            var contrato = new ContratoAluguel(Guid.NewGuid(), request.ClienteId, request.TratorId, request.ValorHora);

            db.ContratosAluguel.Add(contrato);
            await db.SaveChangesAsync();

            // O PULO DO GATO: Dispara os eventos para a aplicação reagir!
            // Isso fará a AtualizarStatusTratorAlugadoPolicy rodar e mudar o status do trator para "Alugado"
            var eventos = contrato.DomainEvents.ToList();
            contrato.LimparEventos();

            foreach (var domainEvent in eventos)
            {
                await mediator.Publish(domainEvent);
            }

            return Results.Created($"/api/contratos/{contrato.Id}", contrato);
        })
        .WithSummary("Abre um novo contrato de aluguel e atualiza o status do equipamento");

        // 2. Listar Contratos
        group.MapGet("/", async (TractorRentalDbContext db) =>
        {
            var contratos = await db.ContratosAluguel.ToListAsync();
            return Results.Ok(contratos);
        })
        .WithSummary("Lista todo o histórico de contratos da empresa");
    }
}

// DTO de entrada
public record CriarContratoRequest(Guid ClienteId, Guid TratorId, decimal ValorHora);