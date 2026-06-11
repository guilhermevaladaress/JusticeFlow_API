using System.Text.Json;
using System.Text.Json.Serialization;
using JusticeFlow.DTOs.Clientes;

namespace JusticeFlow.Services;

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

        try
        {
            var json = await _http.GetStringAsync(url);
            var opts = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            return JsonSerializer.Deserialize<CnpjResponse>(json, opts);
        }
        catch
        {
            return null;
        }
    }
}
