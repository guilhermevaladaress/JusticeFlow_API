using System.Text.Json;

namespace JusticeFlow.Services;

public interface ICnjService
{
    Task<List<CnjTribunalRecord>> BuscarTribunaisAsync();
}

public record CnjTribunalRecord(string Sigla, string Nome, string? Tipo, string? Estado);

public class CnjService : ICnjService
{
    private readonly HttpClient _http;
    private readonly string _baseUrl;
    private readonly string? _resourceId;

    public CnjService(HttpClient http, IConfiguration config)
    {
        _http = http;
        _baseUrl = config["ExternalApis:CnjDadosAbertos"]
            ?? "https://dadosabertos.cnj.jus.br/api/3/action/datastore_search";
        _resourceId = config["ExternalApis:CnjTribunaisResourceId"];
    }

    public async Task<List<CnjTribunalRecord>> BuscarTribunaisAsync()
    {
        if (string.IsNullOrEmpty(_resourceId))
            throw new InvalidOperationException(
                "Configure 'ExternalApis:CnjTribunaisResourceId' no appsettings.json com o resource_id do dataset de tribunais do CNJ.");

        var result = new List<CnjTribunalRecord>();
        int offset = 0;
        const int limit = 100;

        while (true)
        {
            var url = $"{_baseUrl}?resource_id={_resourceId}&limit={limit}&offset={offset}";

            string json;
            try
            {
                json = await _http.GetStringAsync(url);
            }
            catch (HttpRequestException ex)
            {
                throw new HttpRequestException($"Falha ao conectar ao CNJ Dados Abertos: {ex.Message}", ex);
            }

            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            if (!root.TryGetProperty("success", out var success) || !success.GetBoolean())
                break;

            var records = root.GetProperty("result").GetProperty("records");
            int count = 0;

            foreach (var el in records.EnumerateArray())
            {
                count++;
                var record = MapRecord(el);
                if (record != null)
                    result.Add(record);
            }

            if (count < limit) break;
            offset += limit;
        }

        return result;
    }

    private static CnjTribunalRecord? MapRecord(JsonElement el)
    {
        string? Get(params string[] keys)
        {
            foreach (var k in keys)
                if (el.TryGetProperty(k, out var v) && v.ValueKind == JsonValueKind.String)
                    return v.GetString();
            return null;
        }

        var sigla = Get("sigla", "cod_tribunal", "codigo", "sg_tribunal");
        var nome  = Get("nome", "descricao", "nome_tribunal", "ds_tribunal");

        if (string.IsNullOrEmpty(sigla) || string.IsNullOrEmpty(nome)) return null;

        return new CnjTribunalRecord(
            Sigla:  sigla,
            Nome:   nome,
            Tipo:   Get("tipo", "tipo_tribunal", "ds_tipo"),
            Estado: Get("uf", "estado", "sg_uf", "ds_uf")
        );
    }
}
