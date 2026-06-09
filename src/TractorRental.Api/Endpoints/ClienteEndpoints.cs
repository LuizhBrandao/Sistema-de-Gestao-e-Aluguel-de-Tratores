using Microsoft.EntityFrameworkCore;
using TractorRental.Domain.Aggregates;
using TractorRental.Infrastructure.Data;

namespace TractorRental.Api.Endpoints;

public static class ClienteEndpoints
{
    public static void MapClienteEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/clientes").WithTags("Gestão de Clientes");

        // 1. Cadastrar Cliente
        group.MapPost("/", async (CriarClienteRequest request, TractorRentalDbContext db) =>
        {
            // O domínio garante que nome e documento não sejam nulos
            var cliente = new Cliente(Guid.NewGuid(), request.Nome, request.Documento);

            db.Clientes.Add(cliente);
            await db.SaveChangesAsync();

            return Results.Created($"/api/clientes/{cliente.Id}", cliente);
        })
        .WithSummary("Cadastra um novo cliente no sistema");

        // 2. Listar Clientes
        group.MapGet("/", async (TractorRentalDbContext db) =>
        {
            var clientes = await db.Clientes.ToListAsync();
            return Results.Ok(clientes);
        })
        .WithSummary("Lista todos os clientes cadastrados");
    }
}

// DTO de entrada
public record CriarClienteRequest(string Nome, string Documento);