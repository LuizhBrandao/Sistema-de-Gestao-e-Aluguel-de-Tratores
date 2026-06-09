using Dapper;
using Microsoft.Data.SqlClient;
using TractorRental.Application.Interfaces;
using TractorRental.Application.Queries;

namespace TractorRental.Infrastructure.Data.Queries;

public class TratorQueries(string connectionString) : ITratorQueries
{
    public async Task<IEnumerable<TratorDto>> ObterDashboardTratoresAsync()
    {
        // Abre a conexão bruta com o SQL Server
        using var connection = new SqlConnection(connectionString);

        // Uma query limpa e direta. O Dapper mapeia os nomes das colunas direto para o TratorDto
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