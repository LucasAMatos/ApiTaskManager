using ApiTaskManager.Enums;
using ApiTaskManager.Models;
using ApiTaskManager.Models.Request;

namespace ApiTaskManager.Interfaces
{
    public interface IProjetoService
    {
        List<string> GetAllProjects();
        List<string> GetAllProjectsByStatus(Status status);
        Projeto? GetProjecById(int idProjeto);
        int CreateProject(ProjetoRequest projetoRequest);
        void UpdateProject(int idProjeto, ProjetoRequest projetoAtualizado);
        void DeleteProject(int idProjeto);
        Tarefa CreateTask(int idProjeto, TarefaRequest task);
        List<string> GetAllTasksByProject(int idProjeto);
        List<Tarefa> GetprojectTasksByStatus(int idProjeto, Status status);
        Tarefa GetTaskByID(int idTarefa);
        void UpdateTask(int idTarefa, TarefaUpdateRequest request);
        void AddComment(int idTarefa, ComentarioRequest request);
        void CloseTask(int idTarefa);
    }
    
}
