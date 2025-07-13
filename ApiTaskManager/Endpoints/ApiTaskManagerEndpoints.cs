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

        projectEndpoints.MapGet("/listAll",  ([FromServices] IProjetoService projetoService) =>
        {
            return projetoService.GetAllProjects();
        })
        .Produces<List<Projeto>>()
        .WithOpenApiTaskManager("Consultartodososprojetos", "Consulta todos os projetos");

        projectEndpoints.MapGet("/listByStatus/{status}",  (Status status, [FromServices] IProjetoService projetoService) =>
        {
            return projetoService.GetAllProjectsByStatus(status);
        })
        .Produces<List<Projeto>>()
        .WithOpenApiTaskManager("Consultartodososprojetos", "Consulta todos os projetos ativos na equipe");

        projectEndpoints.MapGet("/{idProject}",  (int idProject, [FromServices] IProjetoService projetoService) =>
        {
            return projetoService.GetProjecById(idProject);
        })
        .WithOpenApiTaskManager("ConsultarProjeto", "Consultar detalhes do projeto");

        projectEndpoints.MapPost("/newProject",  (ProjetoRequest request, [FromServices] IProjetoService projetoService) =>
        {
            var id = projetoService.CreateProject(request);
            return Results.Created($"/{id}", request.Nome);
        })
        .WithOpenApiTaskManager("CriarProjeto", "CriaNovoProjeto");

        projectEndpoints.MapPost("/{idProject}/update", static  (int idProject, ProjetoRequest request, [FromServices] IProjetoService projetoService) =>
        {
            projetoService.UpdateProject(idProject, request);
            return Results.Ok();
        })
        .WithOpenApiTaskManager("AtualizarProjeto", "Atualiza os Dados do Projeto");

        projectEndpoints.MapPost("/{idProject}/close",  (int idProject, [FromServices] IProjetoService projetoService) =>
        {
            projetoService.DeleteProject(idProject);
        })
        .WithOpenApiTaskManager("FinalizarProjeto", "Finaliza o Projeto");

        projectEndpoints.MapPut("/{idProject}/createTask/",  (int idProject, TarefaRequest request, [FromServices] IProjetoService projetoService) =>
        {
            return projetoService.CreateTask(idProject, request);
        })
        .WithOpenApiTaskManager("IncluirTarefaNoProjeto", "Inclui nova tarefa no projeto");

        projectEndpoints.MapGet("/{idProject}/tasksByStatus/{status}",  (int idProject, Status status, [FromServices] IProjetoService projetoService) =>
        {
            return projetoService.GetprojectTasksByStatus(idProject, status);
        })
        .WithOpenApiTaskManager("ConsultarTarefasPorStatus", "Consulta as tarefas do projeto Por status");

        projectEndpoints.MapGet("/{idProject}/alltasks",  (int idProject, [FromServices] IProjetoService projetoService) =>
        {
            return projetoService.GetAllTasksByProject(idProject);
        })
        .WithOpenApiTaskManager("Consultartodasastarefas", "Consulta todas as tarefas de um projeto");
    }

    private static void RegisterTaskEndpoints(this WebApplication api)
    {
        var projectEndpoints = api.MapGroup("Tasks").WithTags("Tasks");

        projectEndpoints.MapGet("/{idTask}",  (int idTask, [FromServices] IProjetoService projetoService) =>
        {
            return projetoService.GetTaskByID(idTask);
        })
        .WithOpenApiTaskManager("ConsultarTarefa", "Consulta os detalhes de uma tarefa");

        projectEndpoints.MapPost("/{idTask}/update",  (int idTask, TarefaUpdateRequest request, [FromServices] IProjetoService projetoService) =>
        {
            projetoService.UpdateTask(idTask, request);
        })
        .WithOpenApiTaskManager("AtualizarTarefa", "Atualiza os dados da tarefa");

        projectEndpoints.MapPost("/{idTask}/addcomment",  (int idTask, ComentarioRequest request, [FromServices] IProjetoService projetoService) =>
        {
            projetoService.AddComment(idTask, request);
        })
        .WithOpenApiTaskManager("IncluirComentário", "Adiciona um novo comentário na tarefa");

        projectEndpoints.MapPost("/close/{idTask}",  (int idTask, [FromServices] IProjetoService projetoService) =>
        {
            projetoService.CloseTask(idTask);
        })
        .WithOpenApiTaskManager("FinalizarTarefa", "Finaliza a Tarefa");
    }
}
