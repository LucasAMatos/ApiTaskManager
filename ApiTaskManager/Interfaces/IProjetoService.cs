using ApiTaskManager.Enums;
using ApiTaskManager.Models;
using ApiTaskManager.Models.Request;

namespace ApiTaskManager.Interfaces
{
    public interface IProjetoService
    {
        Task<List<Projeto>> GetAllProjectsAsync();
        Task<Projeto?> GetByIdAsync(int id);
        Task<Projeto> CreateProjectAsync(ProjetoRequest projetoRequest);
        Task<Projeto?> UpdateProjectAsync(int id, ProjetoRequest projetoAtualizado);
        Task<bool> CancelProjectAsync(int id);
        Task<Tarefa> CreateTaskAsync(int idProjeto, TarefaRequest task);
        Task<List<Tarefa>> GetAllTasksByProjectAsync(int idProjeto);
        Task<List<Tarefa>> GetprojectTasksByStatusAsync(int idProjeto, Status status);
        Task<Tarefa> GetTaskByIDAsync(int idTask);
        Task<Tarefa> UpdateTaskAsync(TarefaUpdateRequest request);
        Task<bool> AddCommentAsync(int idTask, string request);
        Task<bool> CloseTaskAsync(int idTask);
    }
    
}
