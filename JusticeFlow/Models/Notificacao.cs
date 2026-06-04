namespace JusticeFlow.Models;

public enum TipoNotificacao { Prazo, Audiencia, Movimentacao, Geral }

public class Notificacao
{
    public int Id { get; set; }
    public string UsuarioId { get; set; } = string.Empty;
    public int? ProcessoId { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string Mensagem { get; set; } = string.Empty;
    public TipoNotificacao Tipo { get; set; }
    public bool Lida { get; set; } = false;
    public DateTime DataCriacao { get; set; } = DateTime.UtcNow;
    public DateTime? DataLeitura { get; set; }

    public Usuario Usuario { get; set; } = null!;
    public Processo? Processo { get; set; }
}
