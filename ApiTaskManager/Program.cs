using ApiTaskManager.Data;
using ApiTaskManager.Endpoints;
using ApiTaskManager.Helpers;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureApiServices();

builder.Services.AddDbContext<DbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
        sql => sql.MigrationsAssembly("ApiTaskManager")));

var app = builder.Build();

app.ConfigureApiPipeline();

app.UseHttpsRedirection();

app.RegisterHealthChekEndpoints();

app.RegisterTaskManagerEndpoints();

app.Run();
