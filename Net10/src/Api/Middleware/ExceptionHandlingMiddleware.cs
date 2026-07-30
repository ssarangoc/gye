using System.Net;
using System.Text.Json;

namespace Inventory.Api.Middleware;

/// <summary>
/// Captura cualquier excepción no controlada, la registra con Serilog (vía ILogger)
/// y devuelve una respuesta JSON uniforme en lugar de la página de error por defecto de ASP.NET.
/// </summary>
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    private readonly IHostEnvironment _environment;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger, IHostEnvironment environment)
    {
        _next = next;
        _logger = logger;
        _environment = environment;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Error no controlado en {Method} {Path}",
                context.Request.Method,
                context.Request.Path);

            await WriteErrorResponseAsync(context, ex);
        }
    }

    private async Task WriteErrorResponseAsync(HttpContext context, Exception ex)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

        var payload = new
        {
            message = "Ocurrió un error inesperado. Intenta nuevamente más tarde.",
            detail = _environment.IsDevelopment() ? ex.Message : null,
            statusCode = context.Response.StatusCode
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(payload));
    }
}
