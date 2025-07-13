using System.Net;
using System.Text.Json;

namespace ApiTaskManager.Middlewares;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context); // Chama o próximo middleware
        }
        catch (ApplicationException appEx)
        {
            _logger.LogError(appEx, "Erro de Negócio");

            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            context.Response.ContentType = "application/json";

            var response = new
            {
                error = $"{appEx.Message}"
            };

            var json = JsonSerializer.Serialize(response);
            await context.Response.WriteAsync(json);

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado ocorreu.");

            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.ContentType = "application/json";

            var response = new
            {
                error = $"Ocorreu um erro inesperado: {ex.Message}"
            };

            var json = JsonSerializer.Serialize(response);
            await context.Response.WriteAsync(json);
        }
    }
}
