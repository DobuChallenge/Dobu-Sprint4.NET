using Dobu.Domain.Commons;

namespace Dobu.Domain.Entities;

public class Pagamento : BaseEntity
{
    public decimal Valor { get; private set; }
    public string FormaPagamento { get; private set; } = string.Empty;
    public DateTime DataPagamento { get; private set; }

    public Guid ConsultaId { get; private set; }
    public Consulta Consulta { get; private set; } = null!;

    private Pagamento()
    {
    }

    public Pagamento(decimal valor, string formaPagamento, DateTime dataPagamento, Guid consultaId)
    {
        Atualizar(valor, formaPagamento, dataPagamento, consultaId);
    }

    public void Atualizar(decimal valor, string formaPagamento, DateTime dataPagamento, Guid consultaId)
    {
        if (valor <= 0)
            throw new ArgumentException("Valor do pagamento invalido");

        if (string.IsNullOrWhiteSpace(formaPagamento))
            throw new ArgumentException("Forma de pagamento invalida");

        Valor = valor;
        FormaPagamento = formaPagamento;
        DataPagamento = dataPagamento;
        ConsultaId = consultaId;
    }
}
