using System.Text.RegularExpressions;
using TractorRental.Locacao.Domain.Enums;

namespace TractorRental.Locacao.Domain.Aggregates;

public class DocumentoIdentificacao
{
    public string Numero { get; private set; }
    public TipoPessoa Tipo { get; private set; }

    // EF Core / serialization require parameterless constructor sometimes
    protected DocumentoIdentificacao() { }

    public DocumentoIdentificacao(string numero, TipoPessoa tipo)
    {
        if (string.IsNullOrWhiteSpace(numero))
            throw new ArgumentException("Documento não pode ser vazio.");

        var limpo = Regex.Replace(numero, "[^0-9]", "");

        if (tipo == TipoPessoa.Fisica && limpo.Length != 11)
            throw new ArgumentException("CPF inválido (tamanho incorreto).");

        if (tipo == TipoPessoa.Juridica && limpo.Length != 14)
            throw new ArgumentException("CNPJ inválido (tamanho incorreto).");

        Numero = limpo;
        Tipo = tipo;
    }
}
