namespace JusticeFlow.Models;

public enum StatusDocumento { Ativo, Arquivado }

public class Documento
{
    public int Id { get; set; }
    public int ProcessoId { get; set; }
    public int TipoDocumentoId { get; set; }
    public string UploadUsuarioId { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public string NomeArquivo { get; set; } = string.Empty;
    public string ConteudoBase64 { get; set; } = string.Empty;
    public string MimeType { get; set; } = string.Empty;
    public DateTime DataUpload { get; set; } = DateTime.UtcNow;
    public StatusDocumento Status { get; set; } = StatusDocumento.Ativo;

    public Processo Processo { get; set; } = null!;
    public TipoDocumento TipoDocumento { get; set; } = null!;
    public Usuario UploadUsuario { get; set; } = null!;
}
