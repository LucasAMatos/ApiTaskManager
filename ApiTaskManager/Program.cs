using ApiTaskManager.Endpoints;
using ApiTaskManager.Helpers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureApiServices();

var app = builder.Build();

app.ConfigureApiPipeline();

app.UseHttpsRedirection();

app.RegisterHealthChekEndpoints();

app.RegisterTaskManagerEndpoints();

app.Run();
