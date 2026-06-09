using MediatR;
using TractorRental.Application.Commands;
using TractorRental.Application.Interfaces;
using TractorRental.Domain.Aggregates;
using TractorRental.Infrastructure.Data;

namespace TractorRental.Api.Endpoints;

public static class TratorEndpoints
{
    public static void MapTratorEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/tratores").WithTags("Gestão de Tratores e Telemetria");

        // 1. Endpoint auxiliar para cadastrar um trator (para termos dados para testar)
        group.MapPost("/", async (CriarTratorRequest request, TractorRentalDbContext db) =>
        {
            var trator = new Trator(Guid.NewGuid(), request.Modelo);
            db.Tratores.Add(trator);
            await db.SaveChangesAsync();

            return Results.Created($"/api/tratores/{trator.Id}", trator);
        })
        .WithSummary("Cadastra um novo equipamento na frota");

        // 2. Endpoint de Consulta (Para vermos o status mudando em tempo real)
        group.MapGet("/{id:guid}", async (Guid id, TractorRentalDbContext db) =>
        {
            var trator = await db.Tratores.FindAsync(id);
            return trator is not null ? Results.Ok(trator) : Results.NotFound();
        })
        .WithSummary("Consulta o status atual e as últimas métricas do trator");

        // Endpoint Otimizado com CQRS e Dapper para o Portal Web
        group.MapGet("/dashboard", async (ITratorQueries queries) =>
        {
            var tratores = await queries.ObterDashboardTratoresAsync();
            return Results.Ok(tratores);
        })
        .WithSummary("Lista todos os tratores e métricas em alta performance (Dapper)");

        // 3. Endpoint Principal: Recebendo a Telemetria IoT (Isolado com CQRS)
        group.MapPost("/telemetria", async (TelemetriaRequest request, IMediator mediator) =>
        {
            // Transforma o JSON da requisição na nossa "intenção" de negócio
            var command = new RegistrarTelemetriaCommand(
                request.TratorId,
                request.TemperaturaMotor,
                request.PressaoPneus,
                request.NivelCombustivel,
                request.NivelOleo,      // <- Novo
                request.RotacaoMotor,   // <- Novo
                request.Velocidade      // <- Novo
            );

            // O MediatR roteia para o Handler, que carrega o Agregado, valida e salva no banco
            var sucesso = await mediator.Send(command);

            if (!sucesso)
                return Results.NotFound(new { Mensagem = "Trator não encontrado na base de dados." });

            return Results.Ok(new { Mensagem = "Telemetria processada e eventos disparados com sucesso." });
        })
        .WithSummary("Recebe carga de dados dos sensores IoT do equipamento");
    }
}

// DTOs (Records são perfeitos para mapear o JSON de entrada de forma imutável)
public record CriarTratorRequest(string Modelo);
public record TelemetriaRequest(Guid TratorId, double TemperaturaMotor, double PressaoPneus, double NivelCombustivel, double NivelOleo, double RotacaoMotor, double Velocidade);