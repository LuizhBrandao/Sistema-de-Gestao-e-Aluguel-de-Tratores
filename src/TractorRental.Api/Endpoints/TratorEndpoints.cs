using MediatR;
using TractorRental.Frota.Application.Commands;
using TractorRental.Telemetria.Application.Commands;
using TractorRental.Frota.Application.Interfaces;
using TractorRental.Frota.Domain.Aggregates;
using TractorRental.Frota.Infrastructure.Data;
using TractorRental.Frota.Domain.Enums;

namespace TractorRental.Api.Endpoints;

public static class TratorEndpoints
{
    public static void MapTratorEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/tratores").WithTags("Gestão de Tratores e Telemetria");

        // 1. Cadastrar trator (BC: Frota)
        group.MapPost("/", async (CriarTratorRequest request, IMediator mediator) =>
        {
            var command = new CadastrarTratorCommand(
                request.Marca,
                request.Modelo,
                request.AnoFabricacao,
                request.PotenciaCv,
                request.HorimetroInicial,
                request.NumeroSerie
            );

            try
            {
                var tratorId = await mediator.Send(command);
                return Results.Created($"/api/tratores/{tratorId}", new { Id = tratorId });
            }
            catch (ArgumentException ex)
            {
                return Results.BadRequest(new { Mensagem = ex.Message });
            }
        })
        .WithSummary("Cadastra um novo equipamento na frota");

        // 2. Consultar trator por ID (BC: Frota)
        group.MapGet("/{id:guid}", async (Guid id, FrotaDbContext db) =>
        {
            var trator = await db.Tratores.FindAsync(id);
            return trator is not null ? Results.Ok(trator) : Results.NotFound();
        })
        .WithSummary("Consulta o status atual e as últimas métricas do trator");

        // 3. Dashboard otimizado com Dapper (BC: Frota - CQRS Read Side)
        group.MapGet("/dashboard", async (ITratorQueries queries) =>
        {
            var tratores = await queries.ObterDashboardTratoresAsync();
            return Results.Ok(tratores);
        })
        .WithSummary("Lista todos os tratores e métricas em alta performance (Dapper)");

        // 4. Endpoint de Telemetria IoT (BC: Telemetria)
        group.MapPost("/telemetria", async (TelemetriaRequest request, IMediator mediator) =>
        {
            var command = new RegistrarTelemetriaCommand(
                request.TratorId,
                request.TemperaturaMotor,
                request.PressaoPneus,
                request.NivelCombustivel,
                request.NivelOleo,
                request.RotacaoMotor,
                request.Velocidade
            );

            var sucesso = await mediator.Send(command);

            if (!sucesso)
                return Results.NotFound(new { Mensagem = "Trator não encontrado na base de dados." });

            return Results.Ok(new { Mensagem = "Telemetria processada e eventos disparados com sucesso." });
        })
        .WithSummary("Recebe carga de dados dos sensores IoT do equipamento");
    }
}

public record CriarTratorRequest(
    string Marca,
    string Modelo,
    int AnoFabricacao,
    int PotenciaCv,
    double HorimetroInicial,
    string NumeroSerie
);
public record TelemetriaRequest(Guid TratorId, double TemperaturaMotor, double PressaoPneus, double NivelCombustivel, double NivelOleo, double RotacaoMotor, double Velocidade);