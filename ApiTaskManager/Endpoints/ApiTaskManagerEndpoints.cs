using ApiTaskManager.Helpers;
using ApiTaskManager.Interfaces;
using Microsoft.AspNetCore.Mvc;

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

        projectEndpoints.MapGet("/listall", async ([FromServices] ITaskManagerService taskManagerService) =>
        {
            throw new NotImplementedException("Serviço Não Implementado");
        })
        .WithOpenApiTaskManager("Consultartodososprojetos", "Consulta todos os projetos ativos na equipe");

        projectEndpoints.MapGet("/{project}", static async ([FromServices] ITaskManagerService taskManagerService) =>
        {
            throw new NotImplementedException("Serviço Não Implementado");
        })
        .WithOpenApiTaskManager("ConsultarProjeto", "Consultar detalhes do projeto");

        projectEndpoints.MapPost("/newproject", static async ([FromServices] ITaskManagerService taskManagerService) =>
        {
            throw new NotImplementedException("Serviço Não Implementado");
        })
        .WithOpenApiTaskManager("CriarProjeto", "CriaNovoProjeto");

        projectEndpoints.MapPost("/{project}/update", static async ([FromServices] ITaskManagerService taskManagerService) =>
        {
            throw new NotImplementedException("Serviço Não Implementado");
        })
        .WithOpenApiTaskManager("AtualizarProjeto", "Atualiza os Dados do Projeto");

        projectEndpoints.MapPost("/{project}/close", async ([FromServices] ITaskManagerService taskManagerService) =>
        {
            throw new NotImplementedException("Serviço Não Implementado");
        })
        .WithOpenApiTaskManager("FinalizarTarefa", "Finaliza o Projeto");

        projectEndpoints.MapPut("/{project}/newtask/", async ([FromServices] ITaskManagerService taskManagerService) =>
        {
            throw new NotImplementedException("Serviço Não Implementado");
        })
        .WithOpenApiTaskManager("IncluirTarefaNoProjeto", "Inclui nova tarefa no projeto");

        projectEndpoints.MapGet("/{project}/alltasks", async ([FromServices] ITaskManagerService taskManagerService) =>
        {
            throw new NotImplementedException("Serviço Não Implementado");
        })
        .WithOpenApiTaskManager("Consultartodasastarefas", "Consulta todas as tarefas de um projeto");

        projectEndpoints.MapGet("/{project}/tasksbystatus/{status}", async ([FromServices] ITaskManagerService taskManagerService) =>
        {
            throw new NotImplementedException("Serviço Não Implementado");
        })
        .WithOpenApiTaskManager("ConsultarTarefasPorStatus", "Consulta as tarefas do projeto Por status");
    }

    private static void RegisterTaskEndpoints(this WebApplication api)
    {
        var projectEndpoints = api.MapGroup("Tasks").WithTags("Tasks");

        projectEndpoints.MapGet("/{task}", async ([FromServices] ITaskManagerService taskManagerService) =>
        {
            throw new NotImplementedException("Serviço Não Implementado");
        })
        .WithOpenApiTaskManager("ConsultarTarefa", "Consulta os detalhes de uma tarefa");

        projectEndpoints.MapPost("/{task}/update", async ([FromServices] ITaskManagerService taskManagerService) =>
        {
            throw new NotImplementedException("Serviço Não Implementado");
        })
        .WithOpenApiTaskManager("AtualizarTarefa", "Atualiza os dados da tarefa");

        projectEndpoints.MapPost("/{task}/addcomment", async ([FromServices] ITaskManagerService taskManagerService) =>
        {
            throw new NotImplementedException("Serviço Não Implementado");
        })
        .WithOpenApiTaskManager("IncluirComentário", "Adiciona um novo comentário na tarefa");

        projectEndpoints.MapPost("/{task}/close", async ([FromServices] ITaskManagerService taskManagerService) =>
        {
            throw new NotImplementedException("Serviço Não Implementado");
        })
        .WithOpenApiTaskManager("FinalizarTarefa", "Finaliza a Tarefa");
    }
}
