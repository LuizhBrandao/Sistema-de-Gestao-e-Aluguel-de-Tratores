using TractorRental.Domain.Enums;
using TractorRental.Domain.Events;

namespace TractorRental.Domain.Aggregates;

public class ContratoAluguel
{
    public Guid Id { get; private set; }
    public Guid ClienteId { get; private set; }
    public Guid TratorId { get; private set; }
    public DateTime DataInicio { get; private set; }
    public DateTime? DataFim { get; private set; }
    public decimal ValorHora { get; private set; }
    public StatusContrato Status { get; private set; }

    private readonly List<object> _domainEvents = new();
    public IReadOnlyCollection<object> DomainEvents => _domainEvents.AsReadOnly();

    protected ContratoAluguel() { }

    public ContratoAluguel(Guid id, Guid clienteId, Guid tratorId, decimal valorHora)
    {
        Id = id;
        ClienteId = clienteId;
        TratorId = tratorId;
        ValorHora = valorHora;
        Status = StatusContrato.Ativo;
        DataInicio = DateTime.UtcNow;

        _domainEvents.Add(new ContratoIniciadoEvent(Id, TratorId, ClienteId, DataInicio));
    }

    public void FinalizarContrato()
    {
        if (Status == StatusContrato.Finalizado)
            throw new InvalidOperationException("Contrato já está finalizado.");
            
        Status = StatusContrato.Finalizado;
        DataFim = DateTime.UtcNow;
    }

    public decimal CalcularFaturamento(DateTime instante)
    {
        var fim = DataFim ?? instante;
        if (fim < DataInicio) return 0;
        
        var totalHoras = (fim - DataInicio).TotalHours;
        return (decimal)totalHoras * ValorHora;
    }

    public void LimparEventos() => _domainEvents.Clear();
}