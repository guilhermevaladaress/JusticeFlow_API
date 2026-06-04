namespace JusticeFlow.Models;

public enum StatusHonorario { Pendente, Pago, Atrasado }

public class Honorario
{
    public int Id { get; set; }
    public int ContratoId { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public decimal Valor { get; set; }
    public DateOnly DataVencimento { get; set; }
    public DateOnly? DataPagamento { get; set; }
    public StatusHonorario Status { get; set; } = StatusHonorario.Pendente;

    public Contrato Contrato { get; set; } = null!;
}
