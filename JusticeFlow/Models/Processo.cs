namespace JusticeFlow.Models;

public enum StatusProcesso { Ativo, Suspenso, Encerrado, Arquivado }

public class Processo
{
    public int Id { get; set; }
    public string NumeroProcesso { get; set; } = string.Empty;
    public string Titulo { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public StatusProcesso Status { get; set; } = StatusProcesso.Ativo;
    public DateOnly DataAbertura { get; set; }
    public DateOnly? DataEncerramento { get; set; }
    public int TipoProcessoId { get; set; }
    public int TribunalId { get; set; }
    public int? VaraId { get; set; }

    public TipoProcesso TipoProcesso { get; set; } = null!;
    public Tribunal Tribunal { get; set; } = null!;
    public Vara? Vara { get; set; }
    public ICollection<ProcessoAdvogado> ProcessosAdvogado { get; set; } = [];
    public ICollection<ProcessoCliente> ProcessosCliente { get; set; } = [];
    public ICollection<Audiencia> Audiencias { get; set; } = [];
    public ICollection<Prazo> Prazos { get; set; } = [];
    public ICollection<Documento> Documentos { get; set; } = [];
    public ICollection<Movimentacao> Movimentacoes { get; set; } = [];
    public ICollection<Notificacao> Notificacoes { get; set; } = [];
}
