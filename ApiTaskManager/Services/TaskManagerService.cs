using ApiTaskManager.Database;
using ApiTaskManager.Interfaces;
using ApiTaskManager.Models;
using ApiTaskManager.Request;
using Microsoft.EntityFrameworkCore;

namespace ApiTaskManager.Services
{
    public class TaskManagerService : ITaskManagerService
    {
        private readonly ApiDbContext _context;

        public TaskManagerService(ApiDbContext context)
        {
            _context = context;
        }

        public async Task<List<Projeto>> GetAllAsync()
        {
            try
            {
                return await _context.Projetos.ToListAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<Projeto?> GetByIdAsync(int id)
        {
            return await _context.Projetos.FindAsync(id);
        }

        public async Task<Projeto> CreateAsync(ProjectRequest request)
        {
            try
            {
                var response = new Projeto
                {
                    Nome = request.Nome,
                    Descricao = request.Descricao,
                    DataDeCriacao = DateTime.UtcNow,
                    AlteradoPor = request.Usuario,
                };
                _context.Projetos.Add(response);
                await _context.SaveChangesAsync();
                return response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        
        public async Task<Projeto?> UpdateAsync(int id, ProjectRequest projetoAtualizado)
        {
            var projeto = await _context.Projetos.FindAsync(id);
            if (projeto == null) return null;

            projeto.Nome = projetoAtualizado.Nome;
            projeto.Descricao = projetoAtualizado.Descricao;
            projeto.AlteradoPor = projetoAtualizado.Usuario;

            await _context.SaveChangesAsync();
            return projeto;
        }

        public async Task<bool> CancelAsync(int id)
        {
            var projeto = await _context.Projetos.FindAsync(id);
            if (projeto == null) return false;

            projeto.Status = Enums.Status.Cancelado;

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
