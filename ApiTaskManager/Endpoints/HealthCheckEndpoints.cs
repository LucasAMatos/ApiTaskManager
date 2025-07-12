using ApiTaskManager.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.OpenApi.Models;

namespace ApiTaskManager.Endpoints
{
    public static class HealthCheckEndpoints
    {
        public static void RegisterHealthChekEndpoints(this WebApplication api)
        {
            api.MapGet("hc", async (HttpContext httpContext,[FromServices] HealthCheckService healthCheckService) =>
            {
                var report = await healthCheckService.CheckHealthAsync();

                var result = new
                {
                    status = report.Status.ToString(),
                    checks = report.Entries.Select(entry => new
                    {
                        key = entry.Key,
                        status = entry.Value.Status.ToString(),
                        description = entry.Value.Description,
                        data = entry.Value.Data
                    }),
                    warnings = report.Entries
                    .Where(entry => entry.Value.Status == HealthStatus.Degraded)
                    .Select(entry => new
                    {
                        key = entry.Key,
                        message = entry.Value.Description
                    })
                };

                int statusCode = StatusCodes.Status204NoContent;

                switch (report.Status)
                {
                    case HealthStatus.Unhealthy:
                        statusCode = StatusCodes.Status503ServiceUnavailable;
                        break;
                    case HealthStatus.Degraded:
                        statusCode = StatusCodes.Status500InternalServerError;
                        break;
                    case HealthStatus.Healthy:
                        statusCode = StatusCodes.Status200OK;
                        break;
                    default:
                        break;
                }
            })
            .WithOpenApi(operation => new(operation)
            {
                OperationId = "HealthCheck",
                Summary = "Status da API",
                Tags = new List<OpenApiTag> { new OpenApiTag { Name = "HealthCheck" } }
            })
            .Produces<OutputHealthCheck>(StatusCodes.Status200OK)
            .Produces<OutputHealthCheck>(StatusCodes.Status500InternalServerError)
            .Produces<OutputHealthCheck>(StatusCodes.Status503ServiceUnavailable);
        }
    }
}
