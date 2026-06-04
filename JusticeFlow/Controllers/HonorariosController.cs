using JusticeFlow.Data;
using JusticeFlow.DTOs.Contratos;
using JusticeFlow.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JusticeFlow.Controllers;

[ApiController]
[Route("api/honorarios")]
[Authorize(Roles = "Administrador,Advogado")]
public class HonorariosController : ControllerBase
{
    private readonly AppDbContext _context;

    public HonorariosController(AppDbContext context) => _context = context;

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int? contratoId)
    {
        var query = _context.Honorarios.Include(h => h.Contrato).AsNoTracking();

        if (contratoId.HasValue)
            query = query.Where(h => h.ContratoId == contratoId.Value);

        var list = await query.ToListAsync();
        return Ok(list.Select(MapResponse));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var h = await _context.Honorarios.Include(h => h.Contrato).AsNoTracking().FirstOrDefaultAsync(h => h.Id == id);
        if (h == null) return NotFound();
        return Ok(MapResponse(h));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateHonorarioRequest req)
    {
        if (!await _context.Contratos.AnyAsync(c => c.Id == req.ContratoId))
            return BadRequest(new { mensagem = "Contrato não encontrado." });

        var hon = new Honorario
        {
            ContratoId     = req.ContratoId,
            Descricao      = req.Descricao,
            Valor          = req.Valor,
            DataVencimento = req.DataVencimento
        };

        _context.Honorarios.Add(hon);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = hon.Id }, new { hon.Id });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateHonorarioRequest req)
    {
        var h = await _context.Honorarios.FindAsync(id);
        if (h == null) return NotFound();

        if (req.Descricao != null)       h.Descricao = req.Descricao;
        if (req.Valor.HasValue)          h.Valor = req.Valor.Value;
        if (req.DataVencimento.HasValue) h.DataVencimento = req.DataVencimento.Value;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpPut("{id}/pagar")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Pagar(int id)
    {
        var h = await _context.Honorarios.FindAsync(id);
        if (h == null) return NotFound();

        h.Status = StatusHonorario.Pago;
        h.DataPagamento = DateOnly.FromDateTime(DateTime.UtcNow);

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Delete(int id)
    {
        var h = await _context.Honorarios.FindAsync(id);
        if (h == null) return NotFound();
        _context.Honorarios.Remove(h);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    private static HonorarioResponse MapResponse(Honorario h) => new()
    {
        Id             = h.Id,
        ContratoId     = h.ContratoId,
        Descricao      = h.Descricao,
        Valor          = h.Valor,
        DataVencimento = h.DataVencimento,
        DataPagamento  = h.DataPagamento,
        Status         = h.Status.ToString()
    };
}
