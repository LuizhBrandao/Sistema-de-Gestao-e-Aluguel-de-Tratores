namespace TractorRental.Locacao.Application.Interfaces;

public interface ITratorLocacaoAcl
{
    /// <summary>
    /// Verifica se o trator existe e se o status dele é "Operacional" no BC de Frota.
    /// </summary>
    Task<bool> IsTratorOperacionalAsync(Guid tratorId, CancellationToken cancellationToken);
    
    Task<bool> TratorExisteAsync(Guid tratorId, CancellationToken cancellationToken);
}
