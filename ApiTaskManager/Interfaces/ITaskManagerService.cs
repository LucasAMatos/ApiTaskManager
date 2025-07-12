using ApiTaskManager.Models;
using ApiTaskManager.Request;

namespace ApiTaskManager.Interfaces
{
    public interface ITaskManagerService
    {
        Task<List<Projeto>> GetAllAsync();
        Task<Projeto?> GetByIdAsync(int id);
        Task<Projeto> CreateAsync(ProjectRequest projeto);
        Task<Projeto?> UpdateAsync(int id, ProjectRequest projetoAtualizado);
        Task<bool> CancelAsync(int id);
    }
}
