using Dobu.Domain.Commons;

namespace Dobu.Domain.Entities;

public class Especie : BaseEntity
{
    public string Nome { get; private set; } = string.Empty;
    public string Descricao { get; private set; } = string.Empty;

    public List<Raca> Racas { get; private set; } = new();

    private Especie()
    {
    }

    public Especie(string nome, string descricao)
    {
        Atualizar(nome, descricao);
    }

    public void Atualizar(string nome, string descricao)
    {
        if (string.IsNullOrWhiteSpace(nome) || nome.Length < 2)
            throw new ArgumentException("Nome da especie invalido");

        if (string.IsNullOrWhiteSpace(descricao) || descricao.Length < 5)
            throw new ArgumentException("Descricao da especie invalida");

        Nome = nome;
        Descricao = descricao;
    }
}
