using TractorRental.Frota.Application.Queries;

namespace TractorRental.Frota.Application.Interfaces;

public interface ITratorQueries
{
    Task<IEnumerable<TratorDto>> ObterDashboardTratoresAsync();
}
