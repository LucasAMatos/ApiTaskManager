using ApiTaskManager.Enums;
using ApiTaskManager.Models.Entity;
using ApiTaskManager.Models.Request;

namespace ApiTaskManager.Interfaces.Services
{
    public interface IProjetoService
    {
        List<string> GetAllProjects();
        List<string> GetAllProjectsByStatus(Status status);
        Projeto? GetProjectById(int idProjeto);
        int CreateProject(ProjetoRequest projetoRequest);
        void UpdateProject(int idProjeto, ProjetoRequest projetoAtualizado);
        void DeleteProject(int idProjeto, string usuario);
        Tarefa CreateTask(int idProjeto, TarefaRequest task);
        List<string> GetAllTasksByProject(int idProjeto);
        List<Tarefa> GetprojectTasksByStatus(int idProjeto, Status status);
        List<TarefaHistorico> GetHistoryTasksByProject(int idProjeto);
        Tarefa GetTaskByID(int idTarefa);
        void UpdateTask(int idTarefa, TarefaUpdateRequest request);
        void AddComment(int idTarefa, ComentarioRequest request);
        void CloseTask(int idTarefa, string usuario);
    }
    
}
