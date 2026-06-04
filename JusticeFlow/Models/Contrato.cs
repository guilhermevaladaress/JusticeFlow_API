namespace JusticeFlow.Models;

public enum FormaHonorarios { Fixo, PercentualExito, Misto }
public enum StatusContrato { Ativo, Encerrado, Cancelado }

public class Contrato
{
    public int Id { get; set; }
    public int AdvogadoId { get; set; }
    public int ClienteId { get; set; }
    public DateOnly DataAssinatura { get; set; }
    public DateOnly? DataValidade { get; set; }
    public decimal ValorHonorarios { get; set; }
    public FormaHonorarios FormaPagamento { get; set; } = FormaHonorarios.Fixo;
    public StatusContrato Status { get; set; } = StatusContrato.Ativo;
    public string? ConteudoBase64 { get; set; }
    public string? MimeType { get; set; }

    public Advogado Advogado { get; set; } = null!;
    public Cliente Cliente { get; set; } = null!;
    public ICollection<Honorario> Honorarios { get; set; } = [];
}
