using ApiTaskManager.Middlewares;
using ApiTaskManager.Data;
using Microsoft.EntityFrameworkCore;
using ApiTaskManager.Services;
using ApiTaskManager.Interfaces.DAL;
using ApiTaskManager.Interfaces.Services;

namespace ApiTaskManager.Helpers
{
    public static class ConfigurationHelper
    {
        public static IServiceCollection ConfigureApiServices(this IServiceCollection services, string connection)
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

            services.AddDbContext<UsuarioDbContext>(options =>
                options.UseSqlServer(connection));

            services.AddDbContext<ProjetoDbContext>(options =>
                options.UseSqlServer(connection));
            //Acesso A Dados
            services.AddScoped<IProjetoDAL, ProjetosDAL>();
            services.AddScoped<IUsuarioDAL, UsuariosDAL>();

            //Serviços
            services.AddScoped<IProjetoService, ProjetoService>();
            services.AddScoped<IUsuarioService, UsuarioService>();
            services.AddScoped<IReportService, ReportService>();

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
