using JusticeFlow.Models;

namespace JusticeFlow.DTOs.Processos;

public class CreateProcessoRequest
{
    public string NumeroProcesso { get; set; } = string.Empty;
    public string Titulo { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public DateOnly DataAbertura { get; set; }
    public int TipoProcessoId { get; set; }
    public int TribunalId { get; set; }
    public int? VaraId { get; set; }
}

public class UpdateProcessoRequest
{
    public string? Titulo { get; set; }
    public string? Descricao { get; set; }
    public int? VaraId { get; set; }
}

public class AlterarStatusProcessoRequest
{
    public StatusProcesso Status { get; set; }
    public DateOnly? DataEncerramento { get; set; }
}

public class ProcessoResponse
{
    public int Id { get; set; }
    public string NumeroProcesso { get; set; } = string.Empty;
    public string Titulo { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateOnly DataAbertura { get; set; }
    public DateOnly? DataEncerramento { get; set; }
    public int TipoProcessoId { get; set; }
    public string TipoProcessoNome { get; set; } = string.Empty;
    public int TribunalId { get; set; }
    public string TribunalNome { get; set; } = string.Empty;
    public int? VaraId { get; set; }
    public string? VaraNome { get; set; }
    public List<string> Advogados { get; set; } = [];
    public List<string> Clientes { get; set; } = [];
}

public class ProcessoAdvogadoResponse
{
    public int AdvogadoId { get; set; }
    public string AdvogadoNome { get; set; } = string.Empty;
    public string NumeroOAB { get; set; } = string.Empty;
    public string TipoVinculo { get; set; } = string.Empty;
    public DateTime DataVinculo { get; set; }
    public bool Ativo { get; set; }
}

public class ProcessoClienteResponse
{
    public int ClienteId { get; set; }
    public string ClienteNome { get; set; } = string.Empty;
    public string TipoParte { get; set; } = string.Empty;
    public DateTime DataVinculo { get; set; }
}

public class VincularAdvogadoRequest
{
    public int AdvogadoId { get; set; }
    public TipoVinculoAdvogado TipoVinculo { get; set; } = TipoVinculoAdvogado.Titular;
}

public class VincularClienteRequest
{
    public int ClienteId { get; set; }
    public TipoParteProcesso TipoParte { get; set; } = TipoParteProcesso.Autor;
}
