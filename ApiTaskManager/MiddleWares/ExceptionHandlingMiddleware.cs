using Azure;
using System.Net;
using System.Text.Json;
using YamlDotNet.Core.Tokens;

namespace ApiTaskManager.Middlewares;

public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    private readonly RequestDelegate _next = next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger = logger;

    public async Task Invoke(HttpContext context)
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
        catch (BadHttpRequestException badHttpRequestEx)
        {
            _logger.LogError(badHttpRequestEx, "Erro de Conversão");

            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            context.Response.ContentType = "application/json";

            var msgException = $"{badHttpRequestEx.Message}";

            if (badHttpRequestEx.InnerException != null)
            {
                msgException += $" - InnerException: {badHttpRequestEx.InnerException.Message}";
            }
            var response = new
            {
                error = msgException
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
