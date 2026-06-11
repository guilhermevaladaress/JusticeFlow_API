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
    public int ProcessosAtivos { get; set; }
    public int AudienciasHoje { get; set; }
    public int PrazosVencendo { get; set; }
    public int TotalDocumentos { get; set; }
    public int TotalClientes { get; set; }
    public int TotalAdvogados { get; set; }
    public int HonariosAtrasados { get; set; }
    public int ProcessosEncerrados { get; set; }
    public int AudienciasMes { get; set; }
    public int PrazosCumpridos { get; set; }
    public int NovosClientesMes { get; set; }
    public List<PrazoProximoItem> PrazosProximos { get; set; } = new();
    public List<AudienciaProximaItem> AudienciasProximas { get; set; } = new();
    public List<ProcessoRecenteItem> ProcessosRecentes { get; set; } = new();
}

public class ProcessoRecenteItem
{
    public int Id { get; set; }
    public string Numero { get; set; } = string.Empty;
    public string Titulo { get; set; } = string.Empty;
    public string AreaDireito { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
}

public class PrazoProximoItem
{
    public int Id { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public string ProcessoTitulo { get; set; } = string.Empty;
    public DateTime DataVencimento { get; set; }
    public string Status { get; set; } = string.Empty;
}

public class AudienciaProximaItem
{
    public int Id { get; set; }
    public string ProcessoTitulo { get; set; } = string.Empty;
    public string TipoAudienciaNome { get; set; } = string.Empty;
    public DateTime DataHora { get; set; }
    public string Status { get; set; } = string.Empty;
}
