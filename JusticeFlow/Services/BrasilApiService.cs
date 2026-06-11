using System.Text.Json;
using System.Text.Json.Serialization;
using JusticeFlow.DTOs.Clientes;

namespace JusticeFlow.Services;

public class BrasilApiRateLimitException : Exception
{
    public BrasilApiRateLimitException()
        : base("Limite de requisições da BrasilAPI atingido. Aguarde alguns segundos e tente novamente.") { }
}

public interface IBrasilApiService
{
    Task<CnpjResponse?> ConsultarCnpjAsync(string cnpj);
}

public class BrasilApiService : IBrasilApiService
{
    private readonly HttpClient _http;
    private readonly string _baseUrl;

    public BrasilApiService(HttpClient http, IConfiguration config)
    {
        _http = http;
        _baseUrl = config["ExternalApis:BrasilApiCnpj"] ?? "https://brasilapi.com.br/api/cnpj/v1/{0}";
    }

    public async Task<CnpjResponse?> ConsultarCnpjAsync(string cnpj)
    {
        var cnpjLimpo = new string(cnpj.Where(char.IsDigit).ToArray());
        if (cnpjLimpo.Length != 14) return null;

        var url = string.Format(_baseUrl, cnpjLimpo);

        var response = await _http.GetAsync(url);

        if ((int)response.StatusCode == 429)
            throw new BrasilApiRateLimitException();

        if (!response.IsSuccessStatusCode)
            return null;

        try
        {
            var json = await response.Content.ReadAsStringAsync();
            var opts = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var raw = JsonSerializer.Deserialize<BrasilApiRaw>(json, opts);
            if (raw == null) return null;

            return new CnpjResponse
            {
                Cnpj              = raw.Cnpj,
                RazaoSocial       = raw.RazaoSocial,
                NomeFantasia      = raw.NomeFantasia,
                SituacaoCadastral = raw.DescricaoSituacaoCadastral,
                NaturezaJuridica  = raw.NaturezaJuridica,
                Porte             = raw.Porte,
                Email             = raw.Email,
                Telefone          = raw.DddTelefone1,
                Logradouro        = raw.Logradouro,
                Numero            = raw.Numero,
                Complemento       = raw.Complemento,
                Bairro            = raw.Bairro,
                Municipio         = raw.Municipio,
                Uf                = raw.Uf,
                Cep               = raw.Cep,
            };
        }
        catch
        {
            return null;
        }
    }

    // Modelo interno para deserializar a resposta snake_case da BrasilAPI
    private class BrasilApiRaw
    {
        public string? Cnpj { get; set; }

        [JsonPropertyName("razao_social")]
        public string? RazaoSocial { get; set; }

        [JsonPropertyName("nome_fantasia")]
        public string? NomeFantasia { get; set; }

        // BrasilAPI retorna situacao_cadastral como int (ex: 2), não string
        [JsonPropertyName("situacao_cadastral")]
        public int? SituacaoCadastralCodigo { get; set; }

        [JsonPropertyName("descricao_situacao_cadastral")]
        public string? DescricaoSituacaoCadastral { get; set; }

        [JsonPropertyName("natureza_juridica")]
        public string? NaturezaJuridica { get; set; }

        [JsonPropertyName("ddd_telefone_1")]
        public string? DddTelefone1 { get; set; }

        public string? Porte { get; set; }
        public string? Email { get; set; }
        public string? Logradouro { get; set; }
        public string? Numero { get; set; }
        public string? Complemento { get; set; }
        public string? Bairro { get; set; }
        public string? Municipio { get; set; }
        public string? Uf { get; set; }
        public string? Cep { get; set; }
    }
}
