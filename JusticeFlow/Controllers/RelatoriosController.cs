using JusticeFlow.Data;
using JusticeFlow.DTOs.Shared;
using JusticeFlow.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JusticeFlow.Controllers;

[ApiController]
[Route("api/relatorios")]
[Authorize(Roles = "Administrador,Advogado")]
public class RelatoriosController : ControllerBase
{
    private readonly AppDbContext _context;

    public RelatoriosController(AppDbContext context) => _context = context;

    [HttpGet("dashboard")]
    public async Task<IActionResult> Dashboard()
    {
        var hoje = DateTime.UtcNow.Date;
        var agora = DateTime.UtcNow;
        var limite7dias = agora.AddDays(7);

        var prazosProximos = await _context.Prazos
            .Include(p => p.Processo)
            .Where(p => p.Status == StatusPrazo.Pendente && p.DataVencimento <= limite7dias)
            .OrderBy(p => p.DataVencimento)
            .Take(5)
            .Select(p => new PrazoProximoItem
            {
                Id              = p.Id,
                Descricao       = p.Descricao,
                ProcessoTitulo  = p.Processo.Titulo,
                DataVencimento  = p.DataVencimento,
                Status          = p.Status.ToString()
            })
            .ToListAsync();

        var audienciasProximas = await _context.Audiencias
            .Include(a => a.Processo)
            .Include(a => a.TipoAudiencia)
            .Where(a => a.DataHora >= agora && a.Status == StatusAudiencia.Agendada)
            .OrderBy(a => a.DataHora)
            .Take(5)
            .Select(a => new AudienciaProximaItem
            {
                Id                = a.Id,
                ProcessoTitulo    = a.Processo.Titulo,
                TipoAudienciaNome = a.TipoAudiencia.Nome,
                DataHora          = a.DataHora,
                Status            = a.Status.ToString()
            })
            .ToListAsync();

        var inicioMes = new DateTime(agora.Year, agora.Month, 1, 0, 0, 0, DateTimeKind.Utc);

        var processosRecentes = await _context.Processos
            .Include(p => p.TipoProcesso)
            .OrderByDescending(p => p.Id)
            .Take(5)
            .Select(p => new ProcessoRecenteItem
            {
                Id          = p.Id,
                Numero      = p.NumeroProcesso,
                Titulo      = p.Titulo,
                AreaDireito = p.TipoProcesso.Nome,
                Status      = p.Status.ToString()
            })
            .ToListAsync();

        var dashboard = new DashboardResponse
        {
            ProcessosAtivos    = await _context.Processos.CountAsync(p => p.Status == StatusProcesso.Ativo),
            AudienciasHoje     = await _context.Audiencias.CountAsync(a => a.DataHora.Date == hoje && a.Status == StatusAudiencia.Agendada),
            PrazosVencendo     = await _context.Prazos.CountAsync(p => p.Status == StatusPrazo.Pendente && p.DataVencimento <= limite7dias),
            TotalDocumentos    = await _context.Documentos.CountAsync(d => d.Status == StatusDocumento.Ativo),
            TotalClientes      = await _context.Clientes.CountAsync(),
            TotalAdvogados     = await _context.Advogados.CountAsync(a => a.Status == StatusAdvogado.Ativo),
            HonariosAtrasados  = await _context.Honorarios.CountAsync(h => h.Status == StatusHonorario.Atrasado),
            ProcessosEncerrados = await _context.Processos.CountAsync(p => p.Status == StatusProcesso.Encerrado),
            AudienciasMes      = await _context.Audiencias.CountAsync(a => a.DataHora >= inicioMes),
            PrazosCumpridos    = await _context.Prazos.CountAsync(p => p.Status == StatusPrazo.Cumprido),
            NovosClientesMes   = await _context.Clientes.CountAsync(c => c.DataCadastro >= inicioMes),
            PrazosProximos     = prazosProximos,
            AudienciasProximas = audienciasProximas,
            ProcessosRecentes  = processosRecentes
        };

        return Ok(dashboard);
    }

    [HttpGet("processos")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> RelatorioProcessos([FromQuery] string? status, [FromQuery] int? tipoId)
    {
        var query = _context.Processos
            .Include(p => p.TipoProcesso)
            .Include(p => p.Tribunal)
            .AsNoTracking();

        if (!string.IsNullOrEmpty(status) && Enum.TryParse<StatusProcesso>(status, out var statusEnum))
            query = query.Where(p => p.Status == statusEnum);

        if (tipoId.HasValue)
            query = query.Where(p => p.TipoProcessoId == tipoId.Value);

        var processos = await query.ToListAsync();

        return Ok(new
        {
            Total = processos.Count,
            PorStatus = processos.GroupBy(p => p.Status.ToString())
                .Select(g => new { Status = g.Key, Total = g.Count() }),
            PorTipo = processos.GroupBy(p => p.TipoProcesso.Nome)
                .Select(g => new { Tipo = g.Key, Total = g.Count() })
        });
    }

    [HttpGet("prazos")]
    public async Task<IActionResult> RelatorioPrazos([FromQuery] string? status)
    {
        var query = _context.Prazos
            .Include(p => p.TipoPrazo)
            .AsNoTracking();

        if (!string.IsNullOrEmpty(status) && Enum.TryParse<StatusPrazo>(status, out var statusEnum))
            query = query.Where(p => p.Status == statusEnum);

        var prazos = await query.ToListAsync();

        return Ok(new
        {
            Total = prazos.Count,
            PorStatus = prazos.GroupBy(p => p.Status.ToString())
                .Select(g => new { Status = g.Key, Total = g.Count() }),
            Vencidos = prazos.Count(p => p.Status == StatusPrazo.Vencido),
            VencendoEm7Dias = prazos.Count(p => p.Status == StatusPrazo.Pendente && p.DataVencimento <= DateTime.UtcNow.AddDays(7))
        });
    }

    [HttpGet("audiencias")]
    public async Task<IActionResult> RelatorioAudiencias([FromQuery] DateTime? inicio, [FromQuery] DateTime? fim)
    {
        var query = _context.Audiencias
            .Include(a => a.TipoAudiencia)
            .AsNoTracking();

        if (inicio.HasValue) query = query.Where(a => a.DataHora >= inicio.Value);
        if (fim.HasValue)    query = query.Where(a => a.DataHora <= fim.Value);

        var audiencias = await query.ToListAsync();

        return Ok(new
        {
            Total = audiencias.Count,
            PorStatus = audiencias.GroupBy(a => a.Status.ToString())
                .Select(g => new { Status = g.Key, Total = g.Count() }),
            PorTipo = audiencias.GroupBy(a => a.TipoAudiencia.Nome)
                .Select(g => new { Tipo = g.Key, Total = g.Count() })
        });
    }

    [HttpGet("honorarios")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> RelatorioHonorarios([FromQuery] DateOnly? inicio, [FromQuery] DateOnly? fim)
    {
        var query = _context.Honorarios.AsNoTracking();

        if (inicio.HasValue) query = query.Where(h => h.DataVencimento >= inicio.Value);
        if (fim.HasValue)    query = query.Where(h => h.DataVencimento <= fim.Value);

        var honorarios = await query.ToListAsync();

        return Ok(new
        {
            Total = honorarios.Count,
            ValorTotal = honorarios.Sum(h => h.Valor),
            ValorRecebido = honorarios.Where(h => h.Status == StatusHonorario.Pago).Sum(h => h.Valor),
            ValorPendente = honorarios.Where(h => h.Status == StatusHonorario.Pendente).Sum(h => h.Valor),
            ValorAtrasado = honorarios.Where(h => h.Status == StatusHonorario.Atrasado).Sum(h => h.Valor),
            PorStatus = honorarios.GroupBy(h => h.Status.ToString())
                .Select(g => new { Status = g.Key, Total = g.Count(), Valor = g.Sum(h => h.Valor) })
        });
    }
}
