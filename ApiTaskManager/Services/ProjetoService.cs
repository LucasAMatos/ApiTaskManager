using ApiTaskManager.Data;
using ApiTaskManager.Enums;
using ApiTaskManager.Extensions;
using ApiTaskManager.Interfaces;
using ApiTaskManager.Models;
using ApiTaskManager.Models.Request;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using System.Threading.Tasks;

namespace ApiTaskManager.Services
{
    public class ProjetoService(IDAL _DAL) : IProjetoService
    {
        #region Projetos
        public List<string> GetAllProjects() => [.. _DAL.GetAll<Projeto>().Select(p => p.Nome)];

        public List<string> GetAllProjectsByStatus(Status status) => [.. _DAL.GetAll<Projeto>().Where(p => p.Status == status).Select(p => p.Nome)];

        public Projeto? GetProjectById(int id) => _DAL.GetById<Projeto>(id);

        public int CreateProject(ProjetoRequest projetoRequest)
        {
            var _usuario = GetUsuarioByName(projetoRequest.Usuario);

            var response = new Projeto
            {
                Nome = projetoRequest.Nome,
                Descricao = projetoRequest.Descricao ?? string.Empty,
                DataDeCriacao = DateTime.UtcNow,
                AlteradoPor = _usuario,
            };

            return _DAL.Create<Projeto>(response).Id;
        }

        public void UpdateProject(int idProjeto, ProjetoRequest projetoAtualizado)
        {
            var _usuario = GetUsuarioByName(projetoAtualizado.Usuario);
            var _projeto = _DAL.GetById<Projeto>(idProjeto) ?? throw new ApplicationException("Projeto não encontrado");

            if (projetoAtualizado.Nome != null) _projeto.Nome = projetoAtualizado.Nome;
            if (projetoAtualizado.Descricao != null) _projeto.Descricao = projetoAtualizado.Descricao;
            _projeto.AlteradoPor = _usuario;

            _DAL.Update<Projeto>(_projeto);
        }

        public void DeleteProject(int idProjeto)
        {
            var _projeto = _DAL.GetById<Projeto>(idProjeto) ?? throw new ApplicationException("Projeto não encontrado");

            _DAL.Delete<Projeto>(_projeto);
        }

        public List<Tarefa> GetprojectTasksByStatus(int idProjeto, Status status)
        {
            var _projeto = _DAL.GetById<Projeto>(idProjeto, p => p.Tarefas) ?? throw new ApplicationException("Projeto não encontrado");

            return [.. _projeto.Tarefas.Where(t => t.Status == status)];
        }

        public List<string> GetAllTasksByProject(int idProjeto)
        {
            var _projeto = _DAL.GetById<Projeto>(idProjeto, p => p.Tarefas) ?? throw new ApplicationException("Projeto não encontrado");

            return [.. _projeto.Tarefas.Select(t => t.Titulo)];
        }

        #endregion Projetos

        #region Tarefas
        public Tarefa CreateTask(int idProjeto, TarefaRequest task)
        {
            var _projeto = _DAL.GetById<Projeto>(idProjeto, p => p.Tarefas) ?? throw new ApplicationException("Projeto não encontrado");

            if (_projeto.Tarefas.Count == 20)
            {
                throw new ApplicationException($"Projeto {idProjeto}-{_projeto.Nome} Possui quantidade máxima de tarefas atribuidas.");
            }

            var _usuario = GetUsuarioByName(task.UsuarioResponsavel);
            var _criadoPor = GetUsuarioByName(task.CriadoPor);

            Tarefa novaTarefa = new()
            {
                Titulo = task.Titulo,
                Descricao = task.Descricao,
                Usuario = _usuario,
                DataDeVencimento = task.DataDeVencimento,
                Prioridade = task.Prioridade,
                Status = Enums.Status.EmAndamento,                
            };

            _projeto.Tarefas.Add(novaTarefa);

            _DAL.Update<Projeto>(_projeto);
            _DAL.Create<TarefaHistorico>(novaTarefa.ToHistorico(_criadoPor, "Criação Tarefa"));

            return novaTarefa;
        }

        public Tarefa GetTaskByID(int idTarefa)
        {
            return _DAL.GetById<Tarefa>(idTarefa) ?? throw new ApplicationException("Tarefa não encontrada");
        }

        public void UpdateTask(int idTarefa, TarefaUpdateRequest request)
        {
            var tarefa = _DAL.GetById<Tarefa>(idTarefa) ?? throw new ApplicationException("Tarefa não encontrada");

            var _AlteradoPor = GetUsuarioByName(request.AlteradoPor);

            if (request.Titulo != null) tarefa.Titulo = request.Titulo;
            if (request.Descricao != null) tarefa.Descricao = request.Descricao;
            if (request.DataDeVencimento.HasValue) tarefa.DataDeVencimento = request.DataDeVencimento.Value;
            if (request.Status.HasValue) tarefa.Status = request.Status.Value;
            if (request.UsuarioResponsavel != null) tarefa.Usuario = GetUsuarioByName(request.UsuarioResponsavel);

            _DAL.Update<Tarefa>(tarefa);
            _DAL.Create<TarefaHistorico>(tarefa.ToHistorico(_AlteradoPor, "Alteração Tarefa"));
        }

        public void AddComment(int idTask, ComentarioRequest request)
        {
            var _usuarioComentario = GetUsuarioByName(request.Usuario);
            var tarefa = _DAL.GetById<Tarefa>(request.IdTarefa) ?? throw new ApplicationException("Tarefa não encontrada");

            tarefa.Comentarios.Add(request.ToComentario(_usuarioComentario));

            _DAL.Update<Tarefa>(tarefa);
            _DAL.Create<TarefaHistorico>(tarefa.ToHistorico(_usuarioComentario, "Novo Comentário na Tarefa"));
        }

        public void CloseTask(int idTarefa)
        {
            var _tarefa = _DAL.GetById<Tarefa>(idTarefa);

            if (_tarefa == null) return;

            _DAL.Delete<Tarefa>(_tarefa);
        }
        #endregion Tarefas

        #region Usuarios
        public Usuario? GetUsuarioByName(string nome)
        {
            return _DAL.GetAll<Usuario>().First(u => u.Nome == nome) ?? throw new ApplicationException("Usuário não encontrado");
        }
        #endregion
    }
}
