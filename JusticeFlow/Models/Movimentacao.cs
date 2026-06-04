namespace JusticeFlow.Models;

public enum TipoMovimentacao { Criacao, Atualizacao, Audiencia, Prazo, Documento, StatusChange }

public class Movimentacao
{
    public int Id { get; set; }
    public int ProcessoId { get; set; }
    public string UsuarioId { get; set; } = string.Empty;
    public DateTime DataHora { get; set; } = DateTime.UtcNow;
    public string Descricao { get; set; } = string.Empty;
    public TipoMovimentacao Tipo { get; set; }

    public Processo Processo { get; set; } = null!;
    public Usuario Usuario { get; set; } = null!;
}
