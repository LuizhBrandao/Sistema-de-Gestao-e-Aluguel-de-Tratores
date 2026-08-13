using Microsoft.Extensions.DependencyInjection;

namespace TractorRental.Telemetria.Infrastructure;

public static class DependencyInjection
{
    /// <summary>
    /// O BC de Telemetria é stateless - não possui repositórios ou DbContext próprio.
    /// Este método existe para manter a simetria arquitetural e facilitar extensões futuras
    /// (ex: adicionar persistência de histórico de leituras).
    /// </summary>
    public static IServiceCollection AddTelemetriaInfrastructure(this IServiceCollection services)
    {
        // Reservado para futuras extensões (ex: repositório de histórico de leituras)
        return services;
    }
}
