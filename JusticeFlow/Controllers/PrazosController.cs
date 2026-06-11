using JusticeFlow.Data;
using JusticeFlow.DTOs.Prazos;
using JusticeFlow.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace JusticeFlow.Controllers;

[ApiController]
[Route("api/prazos")]
[Authorize]
public class PrazosController : ControllerBase
{
    private readonly AppDbContext _context;

    public PrazosController(AppDbContext context) => _context = context;

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var isAdmin = User.IsInRole("Administrador");

        IQueryable<Prazo> query = _context.Prazos
            .Include(p => p.Processo)
            .Include(p => p.TipoPrazo)
            .Include(p => p.Advogado).ThenInclude(a => a!.Usuario)
            .AsNoTracking();

        if (!isAdmin)
        {
            if (User.IsInRole("Advogado"))
            {
                var adv = await _context.Advogados.FirstOrDefaultAsync(a => a.UsuarioId == userId);
                if (adv != null)
                    query = query.Where(p => p.Processo.ProcessosAdvogado.Any(pa => pa.AdvogadoId == adv.Id && pa.Ativo));
            }
            else
            {
                var cli = await _context.Clientes.FirstOrDefaultAsync(c => c.UsuarioId == userId);
                if (cli != null)
                    query = query.Where(p => p.Processo.ProcessosCliente.Any(pc => pc.ClienteId == cli.Id));
            }
        }

        var list = await query.ToListAsync();
        return Ok(list.Select(p => MapResponse(p)));
    }

    [HttpGet("vencendo")]
    [Authorize(Roles = "Administrador,Advogado")]
    public async Task<IActionResult> GetVencendo()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var isAdmin = User.IsInRole("Administrador");
        var limite = DateTime.UtcNow.AddDays(7);

        IQueryable<Prazo> query = _context.Prazos
            .Include(p => p.Processo).ThenInclude(p => p.ProcessosAdvogado)
            .Include(p => p.TipoPrazo)
            .Include(p => p.Advogado).ThenInclude(a => a!.Usuario)
            .Where(p => p.Status == StatusPrazo.Pendente && p.DataVencimento <= limite)
            .AsNoTracking();

        if (!isAdmin)
        {
            var adv = await _context.Advogados.AsNoTracking().FirstOrDefaultAsync(a => a.UsuarioId == userId);
            if (adv != null)
                query = query.Where(p => p.Processo.ProcessosAdvogado.Any(pa => pa.AdvogadoId == adv.Id && pa.Ativo));
        }

        var list = await query.ToListAsync();
        return Ok(list.Select(p => MapResponse(p)));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var p = await _context.Prazos
            .Include(p => p.Processo)
            .Include(p => p.TipoPrazo)
            .Include(p => p.Advogado).ThenInclude(a => a!.Usuario)
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id);

        if (p == null) return NotFound();
        return Ok(MapResponse(p));
    }

    [HttpPost]
    [Authorize(Roles = "Administrador,Advogado")]
    public async Task<IActionResult> Create([FromBody] CreatePrazoRequest req)
    {
        if (!await _context.Processos.AnyAsync(p => p.Id == req.ProcessoId))
            return BadRequest(new { mensagem = "Processo não encontrado." });

        if (!await _context.TiposPrazo.AnyAsync(t => t.Id == req.TipoPrazoId))
            return BadRequest(new { mensagem = "TipoPrazo inválido." });

        var prazo = new Prazo
        {
            ProcessoId     = req.ProcessoId,
            TipoPrazoId    = req.TipoPrazoId,
            AdvogadoId     = req.AdvogadoId,
            Descricao      = req.Descricao,
            DataVencimento = req.DataVencimento,
            Observacoes    = req.Observacoes
        };

        _context.Prazos.Add(prazo);

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        _context.Movimentacoes.Add(new Movimentacao
        {
            ProcessoId = req.ProcessoId,
            UsuarioId  = userId,
            Descricao  = $"Prazo cadastrado: {req.Descricao} — vence em {req.DataVencimento:dd/MM/yyyy}.",
            Tipo       = TipoMovimentacao.Prazo
        });

        _context.Notificacoes.Add(new Notificacao
        {
            UsuarioId  = userId,
            ProcessoId = req.ProcessoId,
            Titulo     = "Novo prazo cadastrado",
            Mensagem   = $"Prazo '{req.Descricao}' vence em {req.DataVencimento:dd/MM/yyyy}.",
            Tipo       = TipoNotificacao.Prazo
        });

        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = prazo.Id }, new { prazo.Id });
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Administrador,Advogado")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdatePrazoRequest req)
    {
        var prazo = await _context.Prazos.FindAsync(id);
        if (prazo == null) return NotFound();

        if (req.Descricao != null)        prazo.Descricao = req.Descricao;
        if (req.DataVencimento.HasValue)  prazo.DataVencimento = req.DataVencimento.Value;
        if (req.AdvogadoId.HasValue)      prazo.AdvogadoId = req.AdvogadoId;
        if (req.Observacoes != null)      prazo.Observacoes = req.Observacoes;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpPut("{id}/cumprir")]
    [Authorize(Roles = "Administrador,Advogado")]
    public async Task<IActionResult> Cumprir(int id)
    {
        var prazo = await _context.Prazos.FindAsync(id);
        if (prazo == null) return NotFound();

        prazo.Status = StatusPrazo.Cumprido;
        prazo.DataCumprimento = DateTime.UtcNow;

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        _context.Movimentacoes.Add(new Movimentacao
        {
            ProcessoId = prazo.ProcessoId,
            UsuarioId  = userId,
            Descricao  = $"Prazo '{prazo.Descricao}' cumprido.",
            Tipo       = TipoMovimentacao.Prazo
        });

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Delete(int id)
    {
        var prazo = await _context.Prazos.FindAsync(id);
        if (prazo == null) return NotFound();
        _context.Prazos.Remove(prazo);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    private static PrazoResponse MapResponse(Prazo p) => new()
    {
        Id              = p.Id,
        ProcessoId      = p.ProcessoId,
        NumeroProcesso  = p.Processo?.NumeroProcesso ?? string.Empty,
        ProcessoTitulo  = p.Processo?.Titulo ?? string.Empty,
        TipoPrazoId     = p.TipoPrazoId,
        TipoPrazoNome   = p.TipoPrazo?.Nome ?? string.Empty,
        AdvogadoNome    = p.Advogado?.Usuario?.NomeCompleto,
        Descricao       = p.Descricao,
        DataVencimento  = p.DataVencimento,
        DataCumprimento = p.DataCumprimento,
        Status          = p.Status.ToString(),
        Observacoes     = p.Observacoes,
        Vencendo        = p.Status == StatusPrazo.Pendente && p.DataVencimento <= DateTime.UtcNow.AddDays(7)
    };
}
