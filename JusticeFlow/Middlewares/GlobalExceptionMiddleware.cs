using System.Net;
using System.Text.Json;

namespace JusticeFlow.Middlewares;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exceção não tratada: {Message}", ex.Message);
            await EscreverRespostaErroAsync(context, ex);
        }
    }

    private static async Task EscreverRespostaErroAsync(HttpContext context, Exception ex)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

        var resposta = new
        {
            status = 500,
            mensagem = "Ocorreu um erro interno. Tente novamente mais tarde.",
            detalhe = ex.Message
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(resposta));
    }
}
