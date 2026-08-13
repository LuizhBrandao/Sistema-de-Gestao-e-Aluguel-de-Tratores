using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TractorRental.Frota.Application.Interfaces;
using TractorRental.Frota.Infrastructure.Data;
using TractorRental.Frota.Infrastructure.Data.Queries;
using TractorRental.Frota.Infrastructure.Data.Repositories;

namespace TractorRental.Frota.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddFrotaInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<FrotaDbContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddScoped<ITratorRepository, TratorRepository>();
        services.AddScoped<IRegistroManutencaoRepository, RegistroManutencaoRepository>();
        services.AddScoped<ITratorQueries>(sp => new TratorQueries(connectionString!));

        return services;
    }
}
