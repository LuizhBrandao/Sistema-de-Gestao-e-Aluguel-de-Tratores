using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TractorRental.Application.Interfaces;
using TractorRental.Infrastructure.Data;
using TractorRental.Infrastructure.Data.Queries; // <-- Adicione o using
using TractorRental.Infrastructure.Data.Repositories;

namespace TractorRental.Infrastructure;

public static class DependencyInjection
{
    // Em TractorRental.Infrastructure/DependencyInjection.cs
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<TractorRentalDbContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddScoped<ITratorRepository, TratorRepository>();

        services.AddScoped<IRegistroManutencaoRepository, RegistroManutencaoRepository>();

        services.AddScoped<ITratorQueries>(sp => new TratorQueries(connectionString!));

        services.AddScoped<IClienteRepository, ClienteRepository>();

        return services;
    }

}