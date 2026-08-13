using Dapper;
using Microsoft.Data.SqlClient;
using TractorRental.Frota.Application.Interfaces;
using TractorRental.Frota.Application.Queries;

namespace TractorRental.Frota.Infrastructure.Data.Queries;

public class TratorQueries(string connectionString) : ITratorQueries
{
    public async Task<IEnumerable<TratorDto>> ObterDashboardTratoresAsync()
    {
        using var connection = new SqlConnection(connectionString);

        const string sql = @"
            SELECT 
                Id, 
                Modelo, 
                Status, 
                TemperaturaAtualMotor, 
                PressaoAtualPneus, 
                NivelCombustivel,
                NivelOleo,        
                RotacaoMotor,     
                Velocidade        
            FROM Tratores
            ORDER BY Status DESC, Modelo ASC";

        return await connection.QueryAsync<TratorDto>(sql);
    }
}
