using ApiTaskManager.Data;
using ApiTaskManager.Enums;
using ApiTaskManager.Interfaces;
using ApiTaskManager.Models;
using ApiTaskManager.Models.Request;
using Microsoft.EntityFrameworkCore;

namespace ApiTaskManager.Services
{
    public class ProjetoService(ProjetosDAL _projetosDAL) : IProjetoService
    {
        public async Task<List<string>> GetAllProjectsAsync()
        {
            return _projetosDAL.GetNameOfAllProjects();
        }

        public async Task<List<string>> GetAllProjectsByStatusAsync(Status status)
        {
            return _projetosDAL.GetProjectsByStatus(status);
        }

        public async Task<Projeto?> GetByIdAsync(int id)
        {
            return _projetosDAL.GetById(id);
        }

        public async Task<int> CreateProjectAsync(ProjetoRequest projetoRequest)
        {
            var response = new Projeto
            {
                Nome = projetoRequest.Nome,
                Descricao = projetoRequest.Descricao ?? string.Empty,
                DataDeCriacao = DateTime.UtcNow,
                AlteradoPor = projetoRequest.Usuario ?? string.Empty,
            };

            
            return _projetosDAL.CreateProject(response);
        }

        public async Task UpdateProjectAsync(int idProjeto, ProjetoRequest projetoAtualizado)
        {
            var projeto = _projetosDAL.GetById(idProjeto);

            projeto.Nome = projetoAtualizado.Nome;
            projeto.Descricao = projetoAtualizado.Descricao ?? string.Empty;
            projeto.AlteradoPor = projetoAtualizado.Usuario ?? string.Empty;

            _projetosDAL.UpdateProject(projeto);
        }

        public async Task CloseProjectAsync(int idProjeto)
        {
            var _projeto = _projetosDAL.GetByIdWithTasks(idProjeto);

            if (_projeto == null) return;

            if (_projeto.Tarefas.Any(t => t.Status != Status.Concluida))
            {
                throw new ApplicationException($"Projeto possui tarefas em aberto. Será necessário concluir ou cancelar as tarefas do projeto {_projeto.Nome}");
            }

            _projetosDAL.Delete<Projeto>(_projeto);
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
            var projeto = await _context.Projetos.FirstOrDefaultAsync(p => p.Id == idProjeto) ?? throw new ApplicationException("Projeto não encontrado");
            projeto.Status = Enums.Status.EmAndamento;

            // Adiciona a tarefa
            projeto.Tarefas.Add(novaTarefa);

            // Salva alterações
            await _context.SaveChangesAsync();

            return novaTarefa;
        }

        public async Task<List<string>> GetAllTasksByProjectAsync(int idProjeto)
        {
            var projeto = await _context.Projetos
                .Include(p => p.Tarefas)
                .FirstOrDefaultAsync(p => p.Id == idProjeto) ?? throw new ApplicationException("Projeto não encontrado");

            return [.. projeto.Tarefas.Select(t => t.Titulo)];
        }

        public async Task<List<Tarefa>> GetprojectTasksByStatusAsync(int idProjeto, Status status)
        {
            return _projetosDAL.GetByIdWithTasks(idProjeto).Tarefas.Where(t => t.Status == status).ToList();
        }

        public async Task<Tarefa> GetTaskByIDAsync(int idTarefa)
        {
            return await _context.Tarefas.FindAsync(idTarefa) ?? throw new ApplicationException("Tarefa não encontrada");
        }

        public async Task<Tarefa> UpdateTaskAsync(int idTarefa, TarefaUpdateRequest request)
        {
            var tarefa = await _context.Tarefas.FindAsync(idTarefa) ?? throw new ApplicationException("Tarefa não encontrada");

            tarefa.Titulo = request.Titulo;
            tarefa.Descricao = request.Descricao;
            tarefa.DataDeVencimento = request.DataDeVencimento;
            tarefa.Status = request.status;

            await _context.SaveChangesAsync();

            return tarefa;
        }

        public Task<bool> AddCommentAsync(int idTask, string request)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> CloseTaskAsync(int idTarefa)
        {
            var _tarefa = await _context.Tarefas.FindAsync(idTarefa) ?? throw new ApplicationException("Tarefa não encontrada");

            _context.Tarefas.Remove(_tarefa);

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
