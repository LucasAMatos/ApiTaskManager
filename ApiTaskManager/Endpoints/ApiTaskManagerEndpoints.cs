using ApiTaskManager.Enums;
using ApiTaskManager.Helpers;
using ApiTaskManager.Interfaces;
using ApiTaskManager.Models;
using ApiTaskManager.Models.Request;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ApiTaskManager.Endpoints;

public static class ApiTaskManagerEndpoints
{
    public static void RegisterTaskManagerEndpoints(this WebApplication api)
    {
        api.RegisterProjectEndpoints();
        api.RegisterTaskEndpoints();
    }

    private static void RegisterProjectEndpoints(this WebApplication api)
    {
        var projectEndpoints = api.MapGroup("Project").WithTags("Project");

        projectEndpoints.MapGet("/listall", async ([FromServices] IProjetoService projetoService) =>
        {
            return await projetoService.GetAllProjectsAsync();
        })
        .Produces<List<Projeto>>()
        .WithOpenApiTaskManager("Consultartodososprojetos", "Consulta todos os projetos ativos na equipe");

        projectEndpoints.MapGet("/{idProject}", async (int idProject, [FromServices] IProjetoService projetoService) =>
        {
            return await projetoService.GetByIdAsync(idProject);
        })
        .WithOpenApiTaskManager("ConsultarProjeto", "Consultar detalhes do projeto");

        projectEndpoints.MapPost("/newproject", async (ProjetoRequest request, [FromServices] IProjetoService projetoService) =>
        {
            var projeto = await projetoService.CreateProjectAsync(request);
            return Results.Created($"/projetos/{projeto.Id}", projeto);
        })
        .WithOpenApiTaskManager("CriarProjeto", "CriaNovoProjeto");

        projectEndpoints.MapPost("/{idProject}/update", static async (int idProject, ProjetoRequest request, [FromServices] IProjetoService projetoService) =>
        {
            return await projetoService.UpdateProjectAsync(idProject, request);
        })
        .WithOpenApiTaskManager("AtualizarProjeto", "Atualiza os Dados do Projeto");

        projectEndpoints.MapPost("/{idProject}/close", async (int idProject, [FromServices] IProjetoService projetoService) =>
        {
            return await projetoService.CancelProjectAsync(idProject);
        })
        .WithOpenApiTaskManager("FinalizarProjeto", "Finaliza o Projeto");

        projectEndpoints.MapPut("/{idProject}/newtask/", async (int idProject, TarefaRequest request, [FromServices] IProjetoService projetoService) =>
        {
            return await projetoService.CreateTaskAsync(idProject, request);
        })
        .WithOpenApiTaskManager("IncluirTarefaNoProjeto", "Inclui nova tarefa no projeto");

        projectEndpoints.MapGet("/{idProject}/tasksbystatus/{status}", async (int idProject, Status status, [FromServices] IProjetoService projetoService) =>
        {
            return await projetoService.GetprojectTasksByStatusAsync(idProject, status);
        })
        .WithOpenApiTaskManager("ConsultarTarefasPorStatus", "Consulta as tarefas do projeto Por status");
    }

    private static void RegisterTaskEndpoints(this WebApplication api)
    {
        var projectEndpoints = api.MapGroup("Tasks").WithTags("Tasks");

        projectEndpoints.MapGet("/{idTask}", async (int idTask, [FromServices] IProjetoService projetoService) =>
        {
            return await projetoService.GetTaskByIDAsync(idTask);
        })
        .WithOpenApiTaskManager("ConsultarTarefa", "Consulta os detalhes de uma tarefa");

        projectEndpoints.MapPost("/{idTask}/update", async (int idTask, TarefaUpdateRequest request, [FromServices] IProjetoService projetoService) =>
        {
            return await projetoService.UpdateTaskAsync(request);
        })
        .WithOpenApiTaskManager("AtualizarTarefa", "Atualiza os dados da tarefa");

        projectEndpoints.MapPost("/{idTask}/addcomment", async (int idTask, [FromBody] string request, [FromServices] IProjetoService projetoService) =>
        {
            return await projetoService.AddCommentAsync(idTask, request);
        })
        .WithOpenApiTaskManager("IncluirComentário", "Adiciona um novo comentário na tarefa");

        projectEndpoints.MapPost("/close/{idTask}", async (int idTask, [FromServices] IProjetoService projetoService) =>
        {
            return await projetoService.CloseTaskAsync(idTask);
        })
        .WithOpenApiTaskManager("FinalizarTarefa", "Finaliza a Tarefa");

        projectEndpoints.MapGet("/{idProject}/alltasks", async (int idProject, [FromServices] IProjetoService projetoService) =>
        {
            return await projetoService.GetAllTasksByProjectAsync(idProject);
        })
        .WithOpenApiTaskManager("Consultartodasastarefas", "Consulta todas as tarefas de um projeto");
    }
}
