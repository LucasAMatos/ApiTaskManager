using ApiTaskManager.OutPut;
using Microsoft.OpenApi.Models;

namespace ApiTaskManager.Helpers;

public static class OpenApiExtension
{
    public static IEndpointConventionBuilder WithOpenApiTaskManager(this IEndpointConventionBuilder operation, string operationId, string summary, params string[] tags)
    {
        return operation.WithOpenApi(operation => new(operation)
        {
            OperationId = operationId,
            Summary = summary
        });
    }
}
