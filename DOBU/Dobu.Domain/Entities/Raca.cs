using Dobu.Domain.Commons;

namespace Dobu.Domain.Entities;

public class Raca : BaseEntity
{
    public string Nome { get; private set; } = string.Empty;
    public string Porte { get; private set; } = string.Empty;
    public int ExpectativaVida { get; private set; }
    public string Descricao { get; private set; } = string.Empty;
    public string Cuidados { get; private set; } = string.Empty;

    public Guid EspecieId { get; private set; }
    public Especie Especie { get; private set; } = null!;
    public List<Pet> Pets { get; private set; } = new();

    private Raca()
    {
    }

    public Raca(string nome, string porte, int expectativaVida, string descricao, string cuidados, Guid especieId)
    {
        Atualizar(nome, porte, expectativaVida, descricao, cuidados, especieId);
    }

    public void Atualizar(string nome, string porte, int expectativaVida, string descricao, string cuidados, Guid especieId)
    {
        if (string.IsNullOrWhiteSpace(nome) || nome.Length < 2)
            throw new ArgumentException("Nome da raca invalido");

        if (string.IsNullOrWhiteSpace(porte))
            throw new ArgumentException("Porte invalido");

        if (expectativaVida <= 0)
            throw new ArgumentException("Expectativa de vida invalida");

        Nome = nome;
        Porte = porte;
        ExpectativaVida = expectativaVida;
        Descricao = descricao;
        Cuidados = cuidados;
        EspecieId = especieId;
    }
}
