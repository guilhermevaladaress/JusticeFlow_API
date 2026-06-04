using JusticeFlow.Models;

namespace JusticeFlow.DTOs.Documentos;

public class CreateDocumentoRequest
{
    public int ProcessoId { get; set; }
    public int TipoDocumentoId { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public string NomeArquivo { get; set; } = string.Empty;
    public string ConteudoBase64 { get; set; } = string.Empty;
    public string MimeType { get; set; } = string.Empty;
}

public class UpdateDocumentoRequest
{
    public string? Nome { get; set; }
    public string? Descricao { get; set; }
}

public class DocumentoResponse
{
    public int Id { get; set; }
    public int ProcessoId { get; set; }
    public string NumeroProcesso { get; set; } = string.Empty;
    public string TipoDocumento { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public string NomeArquivo { get; set; } = string.Empty;
    public string MimeType { get; set; } = string.Empty;
    public DateTime DataUpload { get; set; }
    public string UploadPor { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
}

public class DocumentoArquivoResponse
{
    public string NomeArquivo { get; set; } = string.Empty;
    public string ConteudoBase64 { get; set; } = string.Empty;
    public string MimeType { get; set; } = string.Empty;
}
