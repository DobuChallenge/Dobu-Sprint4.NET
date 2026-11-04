using Dobu.Domain.Commons;

namespace Dobu.Domain.Entities;

public class Consulta : BaseEntity
{
    public DateTime DataConsulta { get; private set; }
    public string Descricao { get; private set; } = string.Empty;
    public decimal Valor { get; private set; }

    public Guid PetId { get; private set; }
    public Pet Pet { get; private set; } = null!;

    public Guid VeterinarioId { get; private set; }
    public Usuario Veterinario { get; private set; } = null!;

    public Prontuario? Prontuario { get; private set; }
    public Pagamento? Pagamento { get; private set; }

    private Consulta()
    {
    }

    public Consulta(DateTime dataConsulta, string descricao, decimal valor, Guid petId, Guid veterinarioId)
    {
        Atualizar(dataConsulta, descricao, valor, petId, veterinarioId);
    }

    public void Atualizar(DateTime dataConsulta, string descricao, decimal valor, Guid petId, Guid veterinarioId)
    {
        if (string.IsNullOrWhiteSpace(descricao) || descricao.Length < 5)
            throw new ArgumentException("Descricao da consulta invalida");

        if (valor < 0)
            throw new ArgumentException("Valor da consulta invalido");

        DataConsulta = dataConsulta;
        Descricao = descricao;
        Valor = valor;
        PetId = petId;
        VeterinarioId = veterinarioId;
    }
}
