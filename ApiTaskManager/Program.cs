using ApiTaskManager.Data;
using ApiTaskManager.Endpoints;
using ApiTaskManager.Helpers;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var connection = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.ConfigureApiServices(connection);

var app = builder.Build();

// Executa as migrations automaticamente no startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ProjetoDbContext>();
    db.Database.Migrate(); // <--- aplica as migra��es pendentes e cria o banco se necess�rio
}

// Executa as migrations automaticamente no startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<UsuarioDbContext>();
    db.Database.Migrate(); // <--- aplica as migra��es pendentes e cria o banco se necess�rio
}

app.ConfigureApiPipeline();

app.UseHttpsRedirection();

app.RegisterHealthChekEndpoints();

app.RegisterTaskManagerEndpoints();

app.Run();
