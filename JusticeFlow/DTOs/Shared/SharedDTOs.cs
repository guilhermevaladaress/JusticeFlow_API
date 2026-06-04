namespace JusticeFlow.DTOs.Shared;

public class TipoResponse
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
}

public class CreateTipoRequest
{
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
}

public class UpdateTipoRequest
{
    public string? Nome { get; set; }
    public string? Descricao { get; set; }
}

public class MovimentacaoResponse
{
    public int Id { get; set; }
    public int ProcessoId { get; set; }
    public string Usuario { get; set; } = string.Empty;
    public DateTime DataHora { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty;
}

public class NotificacaoResponse
{
    public int Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string Mensagem { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty;
    public bool Lida { get; set; }
    public DateTime DataCriacao { get; set; }
    public int? ProcessoId { get; set; }
}

public class ConfiguracaoEscritorioResponse
{
    public int Id { get; set; }
    public string NomeEscritorio { get; set; } = string.Empty;
    public string CNPJ { get; set; } = string.Empty;
    public string? Telefone { get; set; }
    public string Email { get; set; } = string.Empty;
    public string? LogoBase64 { get; set; }
    public string? LogoMimeType { get; set; }
}

public class UpdateConfiguracaoRequest
{
    public string? NomeEscritorio { get; set; }
    public string? CNPJ { get; set; }
    public string? Telefone { get; set; }
    public string? Email { get; set; }
    public string? LogoBase64 { get; set; }
    public string? LogoMimeType { get; set; }
}

public class DashboardResponse
{
    public int TotalProcessosAtivos { get; set; }
    public int TotalAudienciasHoje { get; set; }
    public int TotalPrazosVencendo { get; set; }
    public int TotalDocumentos { get; set; }
    public int TotalClientes { get; set; }
    public int TotalAdvogados { get; set; }
    public int HonariosAtrasados { get; set; }
}
