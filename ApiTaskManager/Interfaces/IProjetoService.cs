using ApiTaskManager.Enums;
using ApiTaskManager.Models;
using ApiTaskManager.Models.Request;

namespace ApiTaskManager.Interfaces
{
    public interface IProjetoService
    {
        Task<List<string>> GetAllProjectsAsync();
        Task<List<string>> GetAllProjectsByStatusAsync(Status status);
        Task<Projeto?> GetByIdAsync(int id);
        Task<Projeto> CreateProjectAsync(ProjetoRequest projetoRequest);
        Task<Projeto?> UpdateProjectAsync(int id, ProjetoRequest projetoAtualizado);
        Task<bool> CloseProjectAsync(int id);
        Task<Tarefa> CreateTaskAsync(int idProjeto, TarefaRequest task);
        Task<List<string>> GetAllTasksByProjectAsync(int idProjeto);
        Task<List<Tarefa>> GetprojectTasksByStatusAsync(int idProjeto, Status status);
        Task<Tarefa> GetTaskByIDAsync(int idTarefa);
        Task<Tarefa> UpdateTaskAsync(int idTarefa, TarefaUpdateRequest request);
        Task<bool> AddCommentAsync(int idTarefa, string request);
        Task<bool> CloseTaskAsync(int idTarefa);
    }
    
}
