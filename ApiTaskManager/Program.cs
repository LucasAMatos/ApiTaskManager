using ApiTaskManager.Data;
using ApiTaskManager.Endpoints;
using ApiTaskManager.Helpers;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var connection = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.ConfigureApiServices(connection);

var app = builder.Build();

app.ConfigureApiPipeline();

app.UseHttpsRedirection();

app.RegisterHealthChekEndpoints();

app.RegisterTaskManagerEndpoints();

app.Run();
