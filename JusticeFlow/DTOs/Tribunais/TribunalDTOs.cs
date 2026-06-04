using JusticeFlow.Models;

namespace JusticeFlow.DTOs.Tribunais;

public class CreateTribunalRequest
{
    public string Nome { get; set; } = string.Empty;
    public string Sigla { get; set; } = string.Empty;
    public TipoTribunal Tipo { get; set; }
    public string? Estado { get; set; }
    public string? CodigoCNJ { get; set; }
}

public class UpdateTribunalRequest
{
    public string? Nome { get; set; }
    public string? CodigoCNJ { get; set; }
}

public class TribunalResponse
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Sigla { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty;
    public string? Estado { get; set; }
    public string? CodigoCNJ { get; set; }
}

public class CreateVaraRequest
{
    public string Nome { get; set; } = string.Empty;
    public int Numero { get; set; }
    public string? Competencia { get; set; }
}

public class UpdateVaraRequest
{
    public string? Nome { get; set; }
    public string? Competencia { get; set; }
}

public class VaraResponse
{
    public int Id { get; set; }
    public int TribunalId { get; set; }
    public string Tribunal { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public int Numero { get; set; }
    public string? Competencia { get; set; }
}
