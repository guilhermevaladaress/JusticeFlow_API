using JusticeFlow.Data;
using JusticeFlow.DTOs.Contratos;
using JusticeFlow.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JusticeFlow.Controllers;

[ApiController]
[Route("api/contratos")]
[Authorize(Roles = "Administrador,Advogado")]
public class ContratosController : ControllerBase
{
    private readonly AppDbContext _context;

    public ContratosController(AppDbContext context) => _context = context;

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var list = await _context.Contratos
            .Include(c => c.Advogado).ThenInclude(a => a.Usuario)
            .Include(c => c.Cliente).ThenInclude(cl => cl.Usuario)
            .AsNoTracking()
            .ToListAsync();

        return Ok(list.Select(MapResponse));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var c = await _context.Contratos
            .Include(c => c.Advogado).ThenInclude(a => a.Usuario)
            .Include(c => c.Cliente).ThenInclude(cl => cl.Usuario)
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id);

        if (c == null) return NotFound();
        return Ok(MapResponse(c));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateContratoRequest req)
    {
        if (!await _context.Advogados.AnyAsync(a => a.Id == req.AdvogadoId))
            return BadRequest(new { mensagem = "Advogado não encontrado." });

        if (!await _context.Clientes.AnyAsync(c => c.Id == req.ClienteId))
            return BadRequest(new { mensagem = "Cliente não encontrado." });

        var contrato = new Contrato
        {
            AdvogadoId      = req.AdvogadoId,
            ClienteId       = req.ClienteId,
            DataAssinatura  = req.DataAssinatura,
            DataValidade    = req.DataValidade,
            ValorHonorarios = req.ValorHonorarios,
            FormaPagamento  = req.FormaPagamento,
            ConteudoBase64  = req.ConteudoBase64,
            MimeType        = req.MimeType
        };

        _context.Contratos.Add(contrato);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = contrato.Id }, new { contrato.Id });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateContratoRequest req)
    {
        var c = await _context.Contratos.FindAsync(id);
        if (c == null) return NotFound();

        if (req.DataValidade.HasValue)    c.DataValidade = req.DataValidade;
        if (req.ValorHonorarios.HasValue) c.ValorHonorarios = req.ValorHonorarios.Value;
        if (req.Status.HasValue)          c.Status = req.Status.Value;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Delete(int id)
    {
        var c = await _context.Contratos.FindAsync(id);
        if (c == null) return NotFound();

        c.Status = StatusContrato.Cancelado;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    private static ContratoResponse MapResponse(Contrato c) => new()
    {
        Id              = c.Id,
        Advogado        = c.Advogado?.Usuario?.NomeCompleto ?? string.Empty,
        Cliente         = c.Cliente?.Usuario?.NomeCompleto ?? string.Empty,
        DataAssinatura  = c.DataAssinatura,
        DataValidade    = c.DataValidade,
        ValorHonorarios = c.ValorHonorarios,
        FormaPagamento  = c.FormaPagamento.ToString(),
        Status          = c.Status.ToString()
    };
}
