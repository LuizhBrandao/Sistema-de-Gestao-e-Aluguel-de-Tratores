using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TractorRental.Locacao.Application.Interfaces;
using TractorRental.Locacao.Infrastructure.Data;
using TractorRental.Locacao.Infrastructure.Data.Repositories;

namespace TractorRental.Locacao.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddLocacaoInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<LocacaoDbContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddScoped<IClienteRepository, ClienteRepository>();
        services.AddScoped<IContratoRepository, ContratoRepository>();

        return services;
    }
}
