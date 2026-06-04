using System.Text.Json;
using JusticeFlow.DTOs.Enderecos;

namespace JusticeFlow.Services;

public interface IViaCepService
{
    Task<ViaCepResponse?> ConsultarCepAsync(string cep);
}

public class ViaCepService : IViaCepService
{
    private readonly HttpClient _http;
    private readonly string _urlTemplate;

    public ViaCepService(HttpClient http, IConfiguration config)
    {
        _http = http;
        _urlTemplate = config["ExternalApis:ViaCep"] ?? "https://viacep.com.br/ws/{0}/json/";
    }

    public async Task<ViaCepResponse?> ConsultarCepAsync(string cep)
    {
        var cepLimpo = new string(cep.Where(char.IsDigit).ToArray());
        if (cepLimpo.Length != 8) return null;

        var url = string.Format(_urlTemplate, cepLimpo);

        try
        {
            var json = await _http.GetStringAsync(url);
            var opts = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var resultado = JsonSerializer.Deserialize<ViaCepResponse>(json, opts);
            return resultado?.Erro == true ? null : resultado;
        }
        catch
        {
            return null;
        }
    }
}
