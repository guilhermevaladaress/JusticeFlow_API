using JusticeFlow.Data;
using JusticeFlow.DTOs.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JusticeFlow.Controllers;

[ApiController]
[Route("api/movimentacoes")]
[Authorize]
public class MovimentacoesController : ControllerBase
{
    private readonly AppDbContext _context;

    public MovimentacoesController(AppDbContext context) => _context = context;

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

        return Ok(list.Select(m => new MovimentacaoResponse
        {
            Id         = m.Id,
            ProcessoId = m.ProcessoId,
            Usuario    = m.Usuario?.NomeCompleto ?? string.Empty,
            DataHora   = m.DataHora,
            Descricao  = m.Descricao,
            Tipo       = m.Tipo.ToString()
        }));
    }
}
