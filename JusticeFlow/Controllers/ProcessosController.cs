using JusticeFlow.Data;
using JusticeFlow.DTOs.Processos;
using JusticeFlow.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace JusticeFlow.Controllers;

[ApiController]
[Route("api/processos")]
[Authorize]
public class ProcessosController : ControllerBase
{
    private readonly AppDbContext _context;

    public ProcessosController(AppDbContext context) => _context = context;

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var isAdmin = User.IsInRole("Administrador");
        var isAdvogado = User.IsInRole("Advogado");

        IQueryable<Processo> query = _context.Processos
            .Include(p => p.TipoProcesso)
            .Include(p => p.Tribunal)
            .Include(p => p.Vara)
            .Include(p => p.ProcessosAdvogado).ThenInclude(pa => pa.Advogado).ThenInclude(a => a.Usuario)
            .Include(p => p.ProcessosCliente).ThenInclude(pc => pc.Cliente).ThenInclude(c => c.Usuario)
            .AsNoTracking();

        if (!isAdmin)
        {
            if (isAdvogado)
            {
                var advogado = await _context.Advogados.FirstOrDefaultAsync(a => a.UsuarioId == userId);
                if (advogado != null)
                    query = query.Where(p => p.ProcessosAdvogado.Any(pa => pa.AdvogadoId == advogado.Id && pa.Ativo));
            }
            else
            {
                var cliente = await _context.Clientes.FirstOrDefaultAsync(c => c.UsuarioId == userId);
                if (cliente != null)
                    query = query.Where(p => p.ProcessosCliente.Any(pc => pc.ClienteId == cliente.Id));
                else
                    return Ok(Array.Empty<object>());
            }
        }

        var processos = await query.ToListAsync();
        return Ok(processos.Select(MapResponse));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var p = await _context.Processos
            .Include(p => p.TipoProcesso)
            .Include(p => p.Tribunal)
            .Include(p => p.Vara)
            .Include(p => p.ProcessosAdvogado).ThenInclude(pa => pa.Advogado).ThenInclude(a => a.Usuario)
            .Include(p => p.ProcessosCliente).ThenInclude(pc => pc.Cliente).ThenInclude(c => c.Usuario)
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id);

        if (p == null) return NotFound();
        return Ok(MapResponse(p));
    }

    [HttpPost]
    [Authorize(Roles = "Administrador,Advogado")]
    public async Task<IActionResult> Create([FromBody] CreateProcessoRequest req)
    {
        if (await _context.Processos.AnyAsync(p => p.NumeroProcesso == req.NumeroProcesso))
            return Conflict(new { mensagem = "Número de processo já cadastrado." });

        if (!await _context.TiposProcesso.AnyAsync(t => t.Id == req.TipoProcessoId))
            return BadRequest(new { mensagem = "TipoProcesso inválido." });

        if (!await _context.Tribunais.AnyAsync(t => t.Id == req.TribunalId))
            return BadRequest(new { mensagem = "Tribunal inválido." });

        var processo = new Processo
        {
            NumeroProcesso = req.NumeroProcesso,
            Titulo         = req.Titulo,
            Descricao      = req.Descricao,
            DataAbertura   = req.DataAbertura,
            TipoProcessoId = req.TipoProcessoId,
            TribunalId     = req.TribunalId,
            VaraId         = req.VaraId
        };

        _context.Processos.Add(processo);
        await _context.SaveChangesAsync();

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        _context.Movimentacoes.Add(new Movimentacao
        {
            ProcessoId = processo.Id,
            UsuarioId  = userId,
            Descricao  = $"Processo {processo.NumeroProcesso} cadastrado.",
            Tipo       = TipoMovimentacao.Criacao
        });

        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = processo.Id }, new { processo.Id });
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Administrador,Advogado")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateProcessoRequest req)
    {
        var p = await _context.Processos.FindAsync(id);
        if (p == null) return NotFound();

        if (req.Titulo != null)    p.Titulo = req.Titulo;
        if (req.Descricao != null) p.Descricao = req.Descricao;
        if (req.VaraId.HasValue)   p.VaraId = req.VaraId;

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        _context.Movimentacoes.Add(new Movimentacao
        {
            ProcessoId = p.Id,
            UsuarioId  = userId,
            Descricao  = "Dados do processo atualizados.",
            Tipo       = TipoMovimentacao.Atualizacao
        });

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpPut("{id}/status")]
    [Authorize(Roles = "Administrador,Advogado")]
    public async Task<IActionResult> AlterarStatus(int id, [FromBody] AlterarStatusProcessoRequest req)
    {
        var p = await _context.Processos.FindAsync(id);
        if (p == null) return NotFound();

        p.Status = req.Status;
        if (req.DataEncerramento.HasValue) p.DataEncerramento = req.DataEncerramento;

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        _context.Movimentacoes.Add(new Movimentacao
        {
            ProcessoId = p.Id,
            UsuarioId  = userId,
            Descricao  = $"Status alterado para {req.Status}.",
            Tipo       = TipoMovimentacao.StatusChange
        });

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Delete(int id)
    {
        var p = await _context.Processos.FindAsync(id);
        if (p == null) return NotFound();

        p.Status = StatusProcesso.Arquivado;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    // Vínculos

    [HttpPost("{id}/advogados")]
    [Authorize(Roles = "Administrador,Advogado")]
    public async Task<IActionResult> VincularAdvogado(int id, [FromBody] VincularAdvogadoRequest req)
    {
        if (!await _context.Processos.AnyAsync(p => p.Id == id))
            return NotFound(new { mensagem = "Processo não encontrado." });

        if (!await _context.Advogados.AnyAsync(a => a.Id == req.AdvogadoId))
            return NotFound(new { mensagem = "Advogado não encontrado." });

        var jaVinculado = await _context.ProcessosAdvogado
            .AnyAsync(pa => pa.ProcessoId == id && pa.AdvogadoId == req.AdvogadoId && pa.Ativo);

        if (jaVinculado)
            return Conflict(new { mensagem = "Advogado já vinculado ao processo." });

        _context.ProcessosAdvogado.Add(new ProcessoAdvogado
        {
            ProcessoId  = id,
            AdvogadoId  = req.AdvogadoId,
            TipoVinculo = req.TipoVinculo
        });

        await _context.SaveChangesAsync();
        return Ok(new { mensagem = "Advogado vinculado com sucesso." });
    }

    [HttpDelete("{id}/advogados/{advId}")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> DesvincularAdvogado(int id, int advId)
    {
        var vinculo = await _context.ProcessosAdvogado
            .FirstOrDefaultAsync(pa => pa.ProcessoId == id && pa.AdvogadoId == advId);

        if (vinculo == null) return NotFound();

        vinculo.Ativo = false;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpPost("{id}/clientes")]
    [Authorize(Roles = "Administrador,Advogado")]
    public async Task<IActionResult> VincularCliente(int id, [FromBody] VincularClienteRequest req)
    {
        if (!await _context.Processos.AnyAsync(p => p.Id == id))
            return NotFound(new { mensagem = "Processo não encontrado." });

        if (!await _context.Clientes.AnyAsync(c => c.Id == req.ClienteId))
            return NotFound(new { mensagem = "Cliente não encontrado." });

        if (await _context.ProcessosCliente.AnyAsync(pc => pc.ProcessoId == id && pc.ClienteId == req.ClienteId))
            return Conflict(new { mensagem = "Cliente já vinculado ao processo." });

        _context.ProcessosCliente.Add(new ProcessoCliente
        {
            ProcessoId = id,
            ClienteId  = req.ClienteId,
            TipoParte  = req.TipoParte
        });

        await _context.SaveChangesAsync();
        return Ok(new { mensagem = "Cliente vinculado com sucesso." });
    }

    [HttpDelete("{id}/clientes/{cliId}")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> DesvincularCliente(int id, int cliId)
    {
        var vinculo = await _context.ProcessosCliente
            .FirstOrDefaultAsync(pc => pc.ProcessoId == id && pc.ClienteId == cliId);

        if (vinculo == null) return NotFound();

        _context.ProcessosCliente.Remove(vinculo);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    private static ProcessoResponse MapResponse(Processo p) => new()
    {
        Id             = p.Id,
        NumeroProcesso = p.NumeroProcesso,
        Titulo         = p.Titulo,
        Descricao      = p.Descricao,
        Status         = p.Status.ToString(),
        DataAbertura   = p.DataAbertura,
        DataEncerramento = p.DataEncerramento,
        TipoProcesso   = p.TipoProcesso?.Nome ?? string.Empty,
        Tribunal       = p.Tribunal?.Nome ?? string.Empty,
        Vara           = p.Vara?.Nome,
        Advogados      = p.ProcessosAdvogado
                          .Where(pa => pa.Ativo)
                          .Select(pa => pa.Advogado.Usuario.NomeCompleto)
                          .ToList(),
        Clientes       = p.ProcessosCliente
                          .Select(pc => pc.Cliente.Usuario.NomeCompleto)
                          .ToList()
    };
}
