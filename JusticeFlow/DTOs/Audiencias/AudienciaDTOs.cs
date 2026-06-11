using JusticeFlow.Models;

namespace JusticeFlow.DTOs.Audiencias;

public class CreateAudienciaRequest
{
    public int ProcessoId { get; set; }
    public int TipoAudienciaId { get; set; }
    public DateTime DataHora { get; set; }
    public string? Local { get; set; }
    public string? LinkVirtual { get; set; }
    public string? Observacoes { get; set; }
}

public class UpdateAudienciaRequest
{
    public DateTime? DataHora { get; set; }
    public string? Local { get; set; }
    public string? LinkVirtual { get; set; }
    public string? Observacoes { get; set; }
}

public class AlterarStatusAudienciaRequest
{
    public StatusAudiencia Status { get; set; }
}

public class AudienciaResponse
{
    public int Id { get; set; }
    public int ProcessoId { get; set; }
    public string NumeroProcesso { get; set; } = string.Empty;
    public string ProcessoTitulo { get; set; } = string.Empty;
    public int TipoAudienciaId { get; set; }
    public string TipoAudienciaNome { get; set; } = string.Empty;
    public DateTime DataHora { get; set; }
    public string? Local { get; set; }
    public string? LinkVirtual { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? Observacoes { get; set; }
}
