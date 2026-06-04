namespace JusticeFlow.Models;

public enum StatusAdvogado { Ativo, Inativo }

public class Advogado
{
    public int Id { get; set; }
    public string UsuarioId { get; set; } = string.Empty;
    public string NumeroOAB { get; set; } = string.Empty;
    public string UF { get; set; } = string.Empty;
    public string? Especialidade { get; set; }
    public DateOnly DataAdmissao { get; set; }
    public StatusAdvogado Status { get; set; } = StatusAdvogado.Ativo;

    public Usuario Usuario { get; set; } = null!;
    public ICollection<ProcessoAdvogado> ProcessosAdvogado { get; set; } = [];
    public ICollection<Contrato> Contratos { get; set; } = [];
    public ICollection<Prazo> Prazos { get; set; } = [];
}
