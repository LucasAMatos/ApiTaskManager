using ApiTaskManager.Middlewares;
using ApiTaskManager.Data;
using Microsoft.EntityFrameworkCore;
using ApiTaskManager.Interfaces;
using ApiTaskManager.Services;

namespace ApiTaskManager.Helpers
{
    public static class ConfigurationHelper
    {
        public static IServiceCollection ConfigureApiServices(this IServiceCollection services)
        {
            services.AddEndpointsApiExplorer();
            services.AddOpenApiDocument(config =>
            {
                config.Title = "APITaskManager";
                config.Version = "V1";
                config.Description = "API para Gerenciar Projetos e Tarefas do TIme";
            });
            // Add services to the container.
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            services.AddOpenApi();
            services.AddHealthChecks();

            //Serviços
            services.AddScoped<IProjetoService, ProjetoService>();
            services.AddScoped<IUsuarioService, UsuarioService>();
            services.AddScoped<IReportService, ReportService>();

            //Acesso A Dados
            services.AddScoped<IDAL, DAL>();

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
