using JusticeFlow.Data;
using JusticeFlow.DTOs.Tribunais;
using JusticeFlow.Models;
using JusticeFlow.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JusticeFlow.Controllers;

[ApiController]
[Route("api/tribunais")]
[Authorize]
public class TribunaisController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ICnjService _cnj;

    public TribunaisController(AppDbContext context, ICnjService cnj)
    {
        _context = context;
        _cnj = cnj;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll()
    {
        var list = await _context.Tribunais.AsNoTracking().ToListAsync();
        return Ok(list.Select(MapResponse));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var t = await _context.Tribunais.AsNoTracking().FirstOrDefaultAsync(t => t.Id == id);
        if (t == null) return NotFound();
        return Ok(MapResponse(t));
    }

    [HttpPost]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Create([FromBody] CreateTribunalRequest req)
    {
        var tribunal = new Tribunal
        {
            Nome      = req.Nome,
            Sigla     = req.Sigla,
            Tipo      = req.Tipo,
            Estado    = req.Estado,
            CodigoCNJ = req.CodigoCNJ
        };
        _context.Tribunais.Add(tribunal);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = tribunal.Id }, MapResponse(tribunal));
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateTribunalRequest req)
    {
        var t = await _context.Tribunais.FindAsync(id);
        if (t == null) return NotFound();

        if (req.Nome != null)      t.Nome = req.Nome;
        if (req.CodigoCNJ != null) t.CodigoCNJ = req.CodigoCNJ;

        await _context.SaveChangesAsync();
        return Ok(MapResponse(t));
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Delete(int id)
    {
        var t = await _context.Tribunais.FindAsync(id);
        if (t == null) return NotFound();
        _context.Tribunais.Remove(t);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    // Varas

    [HttpGet("{tribunalId}/varas")]
    public async Task<IActionResult> GetVaras(int tribunalId)
    {
        var list = await _context.Varas
            .Include(v => v.Tribunal)
            .Where(v => v.TribunalId == tribunalId)
            .AsNoTracking()
            .ToListAsync();

        return Ok(list.Select(MapVaraResponse));
    }

    [HttpPost("{tribunalId}/varas")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> CreateVara(int tribunalId, [FromBody] CreateVaraRequest req)
    {
        if (!await _context.Tribunais.AnyAsync(t => t.Id == tribunalId))
            return NotFound(new { mensagem = "Tribunal não encontrado." });

        var vara = new Vara
        {
            TribunalId  = tribunalId,
            Nome        = req.Nome,
            Numero      = req.Numero,
            Competencia = req.Competencia
        };
        _context.Varas.Add(vara);
        await _context.SaveChangesAsync();

        var varaComTribunal = await _context.Varas.Include(v => v.Tribunal).FirstAsync(v => v.Id == vara.Id);
        return CreatedAtAction(nameof(GetVaras), new { tribunalId }, MapVaraResponse(varaComTribunal));
    }

    [HttpPut("{tribunalId}/varas/{id}")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> UpdateVara(int tribunalId, int id, [FromBody] UpdateVaraRequest req)
    {
        var vara = await _context.Varas.FirstOrDefaultAsync(v => v.Id == id && v.TribunalId == tribunalId);
        if (vara == null) return NotFound();

        if (req.Nome != null)        vara.Nome = req.Nome;
        if (req.Competencia != null) vara.Competencia = req.Competencia;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{tribunalId}/varas/{id}")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> DeleteVara(int tribunalId, int id)
    {
        var vara = await _context.Varas.FirstOrDefaultAsync(v => v.Id == id && v.TribunalId == tribunalId);
        if (vara == null) return NotFound();
        _context.Varas.Remove(vara);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpPost("importar-cnj")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> ImportarCnj()
    {
        List<CnjTribunalRecord> registros;
        try
        {
            registros = await _cnj.BuscarTribunaisAsync();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(502, new { mensagem = "Erro ao consultar CNJ Dados Abertos.", detalhe = ex.Message });
        }

        int criados = 0, atualizados = 0;

        foreach (var r in registros)
        {
            var existente = await _context.Tribunais.FirstOrDefaultAsync(t => t.Sigla == r.Sigla);
            if (existente == null)
            {
                _context.Tribunais.Add(new Tribunal
                {
                    Nome      = r.Nome,
                    Sigla     = r.Sigla,
                    Tipo      = ParseTipo(r.Tipo),
                    Estado    = r.Estado,
                    CodigoCNJ = r.Sigla
                });
                criados++;
            }
            else
            {
                existente.Nome      = r.Nome;
                existente.Tipo      = ParseTipo(r.Tipo);
                existente.Estado    = r.Estado ?? existente.Estado;
                existente.CodigoCNJ ??= r.Sigla;
                atualizados++;
            }
        }

        await _context.SaveChangesAsync();
        return Ok(new { criados, atualizados, total = registros.Count });
    }

    private static TipoTribunal ParseTipo(string? tipo) => tipo?.ToLowerInvariant() switch
    {
        "superior"    => TipoTribunal.Superior,
        "federal"     => TipoTribunal.Federal,
        "estadual"    => TipoTribunal.Estadual,
        "trabalhista" => TipoTribunal.Trabalhista,
        "eleitoral"   => TipoTribunal.Eleitoral,
        _             => TipoTribunal.Estadual
    };

    private static TribunalResponse MapResponse(Tribunal t) => new()
    {
        Id        = t.Id,
        Nome      = t.Nome,
        Sigla     = t.Sigla,
        Tipo      = t.Tipo.ToString(),
        Estado    = t.Estado,
        CodigoCNJ = t.CodigoCNJ
    };

    private static VaraResponse MapVaraResponse(Vara v) => new()
    {
        Id          = v.Id,
        TribunalId  = v.TribunalId,
        Tribunal    = v.Tribunal.Nome,
        Nome        = v.Nome,
        Numero      = v.Numero,
        Competencia = v.Competencia
    };
}
