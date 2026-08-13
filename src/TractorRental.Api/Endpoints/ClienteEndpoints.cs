using Microsoft.EntityFrameworkCore;
using TractorRental.Locacao.Domain.Aggregates;
using TractorRental.Locacao.Infrastructure.Data;

namespace TractorRental.Api.Endpoints;

public static class ClienteEndpoints
{
    public static void MapClienteEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/clientes").WithTags("Gestão de Clientes");

        // 1. Cadastrar Cliente (BC: Locação)
        group.MapPost("/", async (CriarClienteRequest request, LocacaoDbContext db) =>
        {
            var cliente = new Cliente(Guid.NewGuid(), request.Nome, request.Documento);

            db.Clientes.Add(cliente);
            await db.SaveChangesAsync();

            return Results.Created($"/api/clientes/{cliente.Id}", cliente);
        })
        .WithSummary("Cadastra um novo cliente no sistema");

        // 2. Listar Clientes (BC: Locação)
        group.MapGet("/", async (LocacaoDbContext db) =>
        {
            var clientes = await db.Clientes.ToListAsync();
            return Results.Ok(clientes);
        })
        .WithSummary("Lista todos os clientes cadastrados");
    }
}

public record CriarClienteRequest(string Nome, string Documento);