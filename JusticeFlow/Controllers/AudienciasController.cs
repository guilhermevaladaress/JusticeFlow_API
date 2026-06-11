using JusticeFlow.Data;
using JusticeFlow.DTOs.Audiencias;
using JusticeFlow.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace JusticeFlow.Controllers;

[ApiController]
[Route("api/audiencias")]
[Authorize]
public class AudienciasController : ControllerBase
{
    private readonly AppDbContext _context;

    public AudienciasController(AppDbContext context) => _context = context;

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var isAdmin = User.IsInRole("Administrador");

        IQueryable<Audiencia> query = _context.Audiencias
            .Include(a => a.Processo)
            .Include(a => a.TipoAudiencia)
            .AsNoTracking();

        if (!isAdmin)
        {
            if (User.IsInRole("Advogado"))
            {
                var adv = await _context.Advogados.FirstOrDefaultAsync(a => a.UsuarioId == userId);
                if (adv != null)
                    query = query.Where(a => a.Processo.ProcessosAdvogado.Any(pa => pa.AdvogadoId == adv.Id && pa.Ativo));
            }
            else
            {
                var cli = await _context.Clientes.FirstOrDefaultAsync(c => c.UsuarioId == userId);
                if (cli != null)
                    query = query.Where(a => a.Processo.ProcessosCliente.Any(pc => pc.ClienteId == cli.Id));
            }
        }

        var list = await query.ToListAsync();
        return Ok(list.Select(MapResponse));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var a = await _context.Audiencias
            .Include(a => a.Processo)
            .Include(a => a.TipoAudiencia)
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == id);

        if (a == null) return NotFound();
        return Ok(MapResponse(a));
    }

    [HttpPost]
    [Authorize(Roles = "Administrador,Advogado")]
    public async Task<IActionResult> Create([FromBody] CreateAudienciaRequest req)
    {
        if (!await _context.Processos.AnyAsync(p => p.Id == req.ProcessoId))
            return BadRequest(new { mensagem = "Processo não encontrado." });

        if (!await _context.TiposAudiencia.AnyAsync(t => t.Id == req.TipoAudienciaId))
            return BadRequest(new { mensagem = "TipoAudiencia inválido." });

        var audiencia = new Audiencia
        {
            ProcessoId      = req.ProcessoId,
            TipoAudienciaId = req.TipoAudienciaId,
            DataHora        = req.DataHora,
            Local           = req.Local,
            LinkVirtual     = req.LinkVirtual,
            Observacoes     = req.Observacoes
        };

        _context.Audiencias.Add(audiencia);

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        _context.Movimentacoes.Add(new Movimentacao
        {
            ProcessoId = req.ProcessoId,
            UsuarioId  = userId,
            Descricao  = $"Audiência agendada para {req.DataHora:dd/MM/yyyy HH:mm}.",
            Tipo       = TipoMovimentacao.Audiencia
        });

        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = audiencia.Id }, new { audiencia.Id });
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Administrador,Advogado")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateAudienciaRequest req)
    {
        var a = await _context.Audiencias.FindAsync(id);
        if (a == null) return NotFound();

        if (req.DataHora.HasValue)   a.DataHora = req.DataHora.Value;
        if (req.Local != null)       a.Local = req.Local;
        if (req.LinkVirtual != null) a.LinkVirtual = req.LinkVirtual;
        if (req.Observacoes != null) a.Observacoes = req.Observacoes;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpPut("{id}/status")]
    [Authorize(Roles = "Administrador,Advogado")]
    public async Task<IActionResult> AlterarStatus(int id, [FromBody] AlterarStatusAudienciaRequest req)
    {
        var a = await _context.Audiencias.FindAsync(id);
        if (a == null) return NotFound();

        a.Status = req.Status;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Delete(int id)
    {
        var a = await _context.Audiencias.FindAsync(id);
        if (a == null) return NotFound();
        _context.Audiencias.Remove(a);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    private static AudienciaResponse MapResponse(Audiencia a) => new()
    {
        Id                = a.Id,
        ProcessoId        = a.ProcessoId,
        NumeroProcesso    = a.Processo?.NumeroProcesso ?? string.Empty,
        ProcessoTitulo    = a.Processo?.Titulo ?? string.Empty,
        TipoAudienciaId   = a.TipoAudienciaId,
        TipoAudienciaNome = a.TipoAudiencia?.Nome ?? string.Empty,
        DataHora          = a.DataHora,
        Local             = a.Local,
        LinkVirtual       = a.LinkVirtual,
        Status            = a.Status.ToString(),
        Observacoes       = a.Observacoes
    };
}
