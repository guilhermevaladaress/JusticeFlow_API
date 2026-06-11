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
        // Se resource_id configurado, tenta a API CKAN do CNJ
        if (!string.IsNullOrWhiteSpace(_resourceId))
        {
            try
            {
                return await BuscarViaApiAsync();
            }
            catch
            {
                // Portal CNJ indisponível — usa lista oficial embutida
            }
        }

        return ListaOficialBrasileira();
    }

    private async Task<List<CnjTribunalRecord>> BuscarViaApiAsync()
    {
        var result = new List<CnjTribunalRecord>();
        int offset = 0;
        const int limit = 100;

        while (true)
        {
            var url = $"{_baseUrl}?resource_id={_resourceId}&limit={limit}&offset={offset}";
            var json = await _http.GetStringAsync(url);

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
                if (record != null) result.Add(record);
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

    // Lista oficial dos 91 tribunais brasileiros (fonte: CNJ / Resolução CNJ nº 65/2008 e atualizações)
    private static List<CnjTribunalRecord> ListaOficialBrasileira() =>
    [
        // Superiores
        new("STF",   "Supremo Tribunal Federal",                                  "Superior",    null),
        new("STJ",   "Superior Tribunal de Justiça",                              "Superior",    null),
        new("TST",   "Tribunal Superior do Trabalho",                             "Trabalhista", null),
        new("TSE",   "Tribunal Superior Eleitoral",                               "Eleitoral",   null),
        new("STM",   "Superior Tribunal Militar",                                 "Militar",     null),

        // Federais
        new("TRF1",  "Tribunal Regional Federal da 1ª Região",                   "Federal",     null),
        new("TRF2",  "Tribunal Regional Federal da 2ª Região",                   "Federal",     "RJ"),
        new("TRF3",  "Tribunal Regional Federal da 3ª Região",                   "Federal",     "SP"),
        new("TRF4",  "Tribunal Regional Federal da 4ª Região",                   "Federal",     "RS"),
        new("TRF5",  "Tribunal Regional Federal da 5ª Região",                   "Federal",     "PE"),
        new("TRF6",  "Tribunal Regional Federal da 6ª Região",                   "Federal",     "MG"),

        // Estaduais
        new("TJAC",  "Tribunal de Justiça do Acre",                              "Estadual",    "AC"),
        new("TJAL",  "Tribunal de Justiça de Alagoas",                           "Estadual",    "AL"),
        new("TJAP",  "Tribunal de Justiça do Amapá",                             "Estadual",    "AP"),
        new("TJAM",  "Tribunal de Justiça do Amazonas",                          "Estadual",    "AM"),
        new("TJBA",  "Tribunal de Justiça da Bahia",                             "Estadual",    "BA"),
        new("TJCE",  "Tribunal de Justiça do Ceará",                             "Estadual",    "CE"),
        new("TJDFT", "Tribunal de Justiça do Distrito Federal e dos Territórios","Estadual",    "DF"),
        new("TJES",  "Tribunal de Justiça do Espírito Santo",                    "Estadual",    "ES"),
        new("TJGO",  "Tribunal de Justiça de Goiás",                             "Estadual",    "GO"),
        new("TJMA",  "Tribunal de Justiça do Maranhão",                          "Estadual",    "MA"),
        new("TJMG",  "Tribunal de Justiça de Minas Gerais",                      "Estadual",    "MG"),
        new("TJMS",  "Tribunal de Justiça de Mato Grosso do Sul",                "Estadual",    "MS"),
        new("TJMT",  "Tribunal de Justiça de Mato Grosso",                       "Estadual",    "MT"),
        new("TJPA",  "Tribunal de Justiça do Pará",                              "Estadual",    "PA"),
        new("TJPB",  "Tribunal de Justiça da Paraíba",                           "Estadual",    "PB"),
        new("TJPE",  "Tribunal de Justiça de Pernambuco",                        "Estadual",    "PE"),
        new("TJPI",  "Tribunal de Justiça do Piauí",                             "Estadual",    "PI"),
        new("TJPR",  "Tribunal de Justiça do Paraná",                            "Estadual",    "PR"),
        new("TJRJ",  "Tribunal de Justiça do Rio de Janeiro",                    "Estadual",    "RJ"),
        new("TJRN",  "Tribunal de Justiça do Rio Grande do Norte",               "Estadual",    "RN"),
        new("TJRO",  "Tribunal de Justiça de Rondônia",                          "Estadual",    "RO"),
        new("TJRR",  "Tribunal de Justiça de Roraima",                           "Estadual",    "RR"),
        new("TJRS",  "Tribunal de Justiça do Rio Grande do Sul",                 "Estadual",    "RS"),
        new("TJSC",  "Tribunal de Justiça de Santa Catarina",                    "Estadual",    "SC"),
        new("TJSE",  "Tribunal de Justiça de Sergipe",                           "Estadual",    "SE"),
        new("TJSP",  "Tribunal de Justiça de São Paulo",                         "Estadual",    "SP"),
        new("TJTO",  "Tribunal de Justiça do Tocantins",                         "Estadual",    "TO"),

        // Trabalhistas
        new("TRT1",  "Tribunal Regional do Trabalho da 1ª Região",  "Trabalhista", "RJ"),
        new("TRT2",  "Tribunal Regional do Trabalho da 2ª Região",  "Trabalhista", "SP"),
        new("TRT3",  "Tribunal Regional do Trabalho da 3ª Região",  "Trabalhista", "MG"),
        new("TRT4",  "Tribunal Regional do Trabalho da 4ª Região",  "Trabalhista", "RS"),
        new("TRT5",  "Tribunal Regional do Trabalho da 5ª Região",  "Trabalhista", "BA"),
        new("TRT6",  "Tribunal Regional do Trabalho da 6ª Região",  "Trabalhista", "PE"),
        new("TRT7",  "Tribunal Regional do Trabalho da 7ª Região",  "Trabalhista", "CE"),
        new("TRT8",  "Tribunal Regional do Trabalho da 8ª Região",  "Trabalhista", "PA"),
        new("TRT9",  "Tribunal Regional do Trabalho da 9ª Região",  "Trabalhista", "PR"),
        new("TRT10", "Tribunal Regional do Trabalho da 10ª Região", "Trabalhista", "DF"),
        new("TRT11", "Tribunal Regional do Trabalho da 11ª Região", "Trabalhista", "AM"),
        new("TRT12", "Tribunal Regional do Trabalho da 12ª Região", "Trabalhista", "SC"),
        new("TRT13", "Tribunal Regional do Trabalho da 13ª Região", "Trabalhista", "PB"),
        new("TRT14", "Tribunal Regional do Trabalho da 14ª Região", "Trabalhista", "RO"),
        new("TRT15", "Tribunal Regional do Trabalho da 15ª Região", "Trabalhista", "SP"),
        new("TRT16", "Tribunal Regional do Trabalho da 16ª Região", "Trabalhista", "MA"),
        new("TRT17", "Tribunal Regional do Trabalho da 17ª Região", "Trabalhista", "ES"),
        new("TRT18", "Tribunal Regional do Trabalho da 18ª Região", "Trabalhista", "GO"),
        new("TRT19", "Tribunal Regional do Trabalho da 19ª Região", "Trabalhista", "AL"),
        new("TRT20", "Tribunal Regional do Trabalho da 20ª Região", "Trabalhista", "SE"),
        new("TRT21", "Tribunal Regional do Trabalho da 21ª Região", "Trabalhista", "RN"),
        new("TRT22", "Tribunal Regional do Trabalho da 22ª Região", "Trabalhista", "PI"),
        new("TRT23", "Tribunal Regional do Trabalho da 23ª Região", "Trabalhista", "MT"),
        new("TRT24", "Tribunal Regional do Trabalho da 24ª Região", "Trabalhista", "MS"),

        // Eleitorais
        new("TREAC", "Tribunal Regional Eleitoral do Acre",                 "Eleitoral", "AC"),
        new("TREAL", "Tribunal Regional Eleitoral de Alagoas",              "Eleitoral", "AL"),
        new("TREAP", "Tribunal Regional Eleitoral do Amapá",                "Eleitoral", "AP"),
        new("TREAM", "Tribunal Regional Eleitoral do Amazonas",             "Eleitoral", "AM"),
        new("TREBA", "Tribunal Regional Eleitoral da Bahia",                "Eleitoral", "BA"),
        new("TRECE", "Tribunal Regional Eleitoral do Ceará",                "Eleitoral", "CE"),
        new("TREDF", "Tribunal Regional Eleitoral do Distrito Federal",     "Eleitoral", "DF"),
        new("TREES", "Tribunal Regional Eleitoral do Espírito Santo",       "Eleitoral", "ES"),
        new("TREGO", "Tribunal Regional Eleitoral de Goiás",                "Eleitoral", "GO"),
        new("TREMA", "Tribunal Regional Eleitoral do Maranhão",             "Eleitoral", "MA"),
        new("TREMG", "Tribunal Regional Eleitoral de Minas Gerais",         "Eleitoral", "MG"),
        new("TREMS", "Tribunal Regional Eleitoral de Mato Grosso do Sul",   "Eleitoral", "MS"),
        new("TREMT", "Tribunal Regional Eleitoral de Mato Grosso",          "Eleitoral", "MT"),
        new("TREPA", "Tribunal Regional Eleitoral do Pará",                 "Eleitoral", "PA"),
        new("TREPB", "Tribunal Regional Eleitoral da Paraíba",              "Eleitoral", "PB"),
        new("TREPE", "Tribunal Regional Eleitoral de Pernambuco",           "Eleitoral", "PE"),
        new("TREPI", "Tribunal Regional Eleitoral do Piauí",                "Eleitoral", "PI"),
        new("TREPR", "Tribunal Regional Eleitoral do Paraná",               "Eleitoral", "PR"),
        new("TRERJ", "Tribunal Regional Eleitoral do Rio de Janeiro",       "Eleitoral", "RJ"),
        new("TRERN", "Tribunal Regional Eleitoral do Rio Grande do Norte",  "Eleitoral", "RN"),
        new("TRERO", "Tribunal Regional Eleitoral de Rondônia",             "Eleitoral", "RO"),
        new("TRERR", "Tribunal Regional Eleitoral de Roraima",              "Eleitoral", "RR"),
        new("TRERS", "Tribunal Regional Eleitoral do Rio Grande do Sul",    "Eleitoral", "RS"),
        new("TRESC", "Tribunal Regional Eleitoral de Santa Catarina",       "Eleitoral", "SC"),
        new("TRESE", "Tribunal Regional Eleitoral de Sergipe",              "Eleitoral", "SE"),
        new("TRESP", "Tribunal Regional Eleitoral de São Paulo",            "Eleitoral", "SP"),
        new("TRETO", "Tribunal Regional Eleitoral do Tocantins",            "Eleitoral", "TO"),

        // Militares estaduais
        new("TJMMG", "Tribunal de Justiça Militar do Estado de Minas Gerais",      "Militar", "MG"),
        new("TJMRS", "Tribunal de Justiça Militar do Estado do Rio Grande do Sul", "Militar", "RS"),
        new("TJMSP", "Tribunal de Justiça Militar do Estado de São Paulo",         "Militar", "SP"),
    ];
}
