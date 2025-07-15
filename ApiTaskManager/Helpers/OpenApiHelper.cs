using Microsoft.OpenApi;
using Microsoft.AspNetCore.OpenApi;

namespace ApiTaskManager.Helpers;

public static class OpenApiExtension
{
    public static IEndpointConventionBuilder WithOpenApiTaskManager(this IEndpointConventionBuilder operation, string operationId, string summary)
    {
        return operation.WithOpenApi(operation => new(operation)
        {
            OperationId = operationId,
            Summary = summary
        });
    }
}
