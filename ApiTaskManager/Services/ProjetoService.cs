using ApiTaskManager.Data;
using ApiTaskManager.Enums;
using ApiTaskManager.Interfaces;
using ApiTaskManager.Models;
using ApiTaskManager.Models.Request;
using Microsoft.EntityFrameworkCore;

namespace ApiTaskManager.Services
{
    public class ProjetoService(ApiDbContext context) : IProjetoService
    {
        private readonly ApiDbContext _context = context;

        public async Task<List<Projeto>> GetAllProjectsAsync()
        {
            return await _context.Projetos.ToListAsync();
        }

        public async Task<Projeto?> GetByIdAsync(int id)
        {
            return await _context.Projetos.FindAsync(id);
        }

        public async Task<Projeto> CreateProjectAsync(ProjetoRequest projetoRequest)
        {
            var response = new Projeto
            {
                Nome = projetoRequest.Nome,
                Descricao = projetoRequest.Descricao ?? string.Empty,
                DataDeCriacao = DateTime.UtcNow,
                AlteradoPor = projetoRequest.Usuario ?? string.Empty,
            };
            _context.Projetos.Add(response);
            await _context.SaveChangesAsync();
            return response;
        }

        public async Task<Projeto?> UpdateProjectAsync(int id, ProjetoRequest projetoAtualizado)
        {
            var projeto = await _context.Projetos.FindAsync(id);
            if (projeto == null) return null;

            projeto.Nome = projetoAtualizado.Nome;
            projeto.Descricao = projetoAtualizado.Descricao ?? string.Empty;
            projeto.AlteradoPor = projetoAtualizado.Usuario ?? string.Empty;

            await _context.SaveChangesAsync();
            return projeto;
        }

        public async Task<bool> CancelProjectAsync(int id)
        {
            var projeto = await _context.Projetos.FindAsync(id);
            if (projeto == null) return false;

            projeto.Status = Enums.Status.Cancelado;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Tarefa> CreateTaskAsync(int idProjeto, TarefaRequest task)
        {
            Tarefa novaTarefa = new()
            {
                Titulo = task.Titulo,
                Descricao = task.Descricao,
                usuario = task.UsuarioResponsavel,
                DataDeVencimento = task.DataDeVencimento,
                Prioridade = task.Prioridade,
                Status = Enums.Status.EmAndamento,
            };

            // Busca o projeto existente
            var projeto = await _context.Projetos.FirstOrDefaultAsync(p => p.Id == idProjeto) ?? throw new Exception("Projeto não encontrado");
            projeto.Status = Enums.Status.EmAndamento;

            // Adiciona a tarefa
            projeto.Tarefas.Add(novaTarefa);

            // Salva alterações
            await _context.SaveChangesAsync();

            return novaTarefa;
        }

        public Task<List<Tarefa>> GetAllTasksByProjectAsync(int idProjeto)
        {
            throw new NotImplementedException();
        }

        public Task<List<Tarefa>> GetprojectTasksByStatusAsync(int idProjeto, Status status)
        {
            throw new NotImplementedException();
        }

        public Task<Tarefa> GetTaskByIDAsync(int idTask)
        {
            throw new NotImplementedException();
        }

        public Task<Tarefa> UpdateTaskAsync(TarefaUpdateRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<bool> AddCommentAsync(int idTask, string request)
        {
            throw new NotImplementedException();
        }

        public Task<bool> CloseTaskAsync(int idTask)
        {
            throw new NotImplementedException();
        }
    }
}
