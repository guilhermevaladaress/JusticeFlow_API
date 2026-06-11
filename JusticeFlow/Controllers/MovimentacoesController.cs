using JusticeFlow.Data;
using JusticeFlow.DTOs.Shared;
using JusticeFlow.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace JusticeFlow.Controllers;

[ApiController]
[Route("api/movimentacoes")]
[Authorize]
public class MovimentacoesController : ControllerBase
{
    private readonly AppDbContext _context;

    public MovimentacoesController(AppDbContext context) => _context = context;

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var isAdmin = User.IsInRole("Administrador");

        IQueryable<Movimentacao> query = _context.Movimentacoes
            .Include(m => m.Usuario)
            .Include(m => m.Processo)
            .AsNoTracking();

        if (!isAdmin)
        {
            if (User.IsInRole("Advogado"))
            {
                var adv = await _context.Advogados.AsNoTracking().FirstOrDefaultAsync(a => a.UsuarioId == userId);
                if (adv != null)
                    query = query.Where(m => m.Processo.ProcessosAdvogado.Any(pa => pa.AdvogadoId == adv.Id && pa.Ativo));
            }
            else
            {
                var cli = await _context.Clientes.AsNoTracking().FirstOrDefaultAsync(c => c.UsuarioId == userId);
                if (cli != null)
                    query = query.Where(m => m.Processo.ProcessosCliente.Any(pc => pc.ClienteId == cli.Id));
            }
        }

        var list = await query.OrderByDescending(m => m.DataHora).Take(200).ToListAsync();
        return Ok(list.Select(MapFull));
    }

    [HttpGet("processo/{processoId}")]
    public async Task<IActionResult> GetByProcesso(int processoId)
    {
        if (!await _context.Processos.AnyAsync(p => p.Id == processoId))
            return NotFound(new { mensagem = "Processo não encontrado." });

        var list = await _context.Movimentacoes
            .Include(m => m.Usuario)
            .Where(m => m.ProcessoId == processoId)
            .OrderByDescending(m => m.DataHora)
            .AsNoTracking()
            .ToListAsync();

        return Ok(list.Select(MapResponse));
    }

    [HttpPost]
    [Authorize(Roles = "Administrador,Advogado")]
    public async Task<IActionResult> Create([FromBody] CreateMovimentacaoRequest req)
    {
        if (!await _context.Processos.AnyAsync(p => p.Id == req.ProcessoId))
            return BadRequest(new { mensagem = "Processo não encontrado." });

        if (!Enum.TryParse<TipoMovimentacao>(req.Tipo, true, out var tipo))
            tipo = TipoMovimentacao.Atualizacao;

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var mov = new Movimentacao
        {
            ProcessoId = req.ProcessoId,
            UsuarioId  = userId,
            Descricao  = req.Descricao,
            Tipo       = tipo
        };

        _context.Movimentacoes.Add(mov);
        await _context.SaveChangesAsync();
        await _context.Entry(mov).Reference(m => m.Usuario).LoadAsync();
        return CreatedAtAction(nameof(GetByProcesso), new { processoId = mov.ProcessoId }, MapResponse(mov));
    }

    private static MovimentacaoResponse MapResponse(Movimentacao m) => new()
    {
        Id         = m.Id,
        ProcessoId = m.ProcessoId,
        Usuario    = m.Usuario?.NomeCompleto ?? string.Empty,
        DataHora   = m.DataHora,
        Descricao  = m.Descricao,
        Tipo       = m.Tipo.ToString()
    };

    private static MovimentacaoResponse MapFull(Movimentacao m) => new()
    {
        Id             = m.Id,
        ProcessoId     = m.ProcessoId,
        ProcessoTitulo = m.Processo?.Titulo,
        NumeroProcesso = m.Processo?.NumeroProcesso,
        Usuario        = m.Usuario?.NomeCompleto ?? string.Empty,
        DataHora       = m.DataHora,
        Descricao      = m.Descricao,
        Tipo           = m.Tipo.ToString()
    };
}
