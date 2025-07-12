using ApiTaskManager.Middlewares;

namespace ApiTaskManager.Helpers
{
    public static class ConfigurationHelper
    {
        public static IServiceCollection ConfigureApiServices(this IServiceCollection services)
        {
            services.AddEndpointsApiExplorer();
            services.AddOpenApiDocument(config =>
            {
                config.Title = "AlteredSearch API";
                config.Version = "v1";
                config.Description = "API para consultar dados públicos do jogo Altered.";
            });
            // Add services to the container.
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            services.AddOpenApi();
            services.AddHealthChecks();

            return services;
        }
        public static WebApplication ConfigureApiPipeline(this WebApplication app)
        {
            app.UseMiddleware<ExceptionHandlingMiddleware>();
            app.UseOpenApi();      // Gera o /swagger/v1/swagger.json
            app.UseSwaggerUi();   // Interface Swagger compatível com WithDescription

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi(); 
            }

            return app;
        }
    }
}
