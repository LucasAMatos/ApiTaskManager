using ApiTaskManager.Data;
using ApiTaskManager.Enums;
using ApiTaskManager.Extensions;
using ApiTaskManager.Interfaces;
using ApiTaskManager.Models;
using ApiTaskManager.Models.Request;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

namespace ApiTaskManager.Services
{
    public class ProjetoService(DAL _DAL) : IProjetoService
    {
        #region Projetos
        public List<string> GetAllProjects() => [.. _DAL.GetAll<Projeto>().Select(p => p.Nome)];

        public  List<string> GetAllProjectsByStatus(Status status) => [.. _DAL.GetAll<Projeto>().Where(p => p.Status == status).Select(p => p.Nome)];

        public Projeto? GetProjecById(int id) => _DAL.GetById<Projeto>(id);

        public int CreateProject(ProjetoRequest projetoRequest)
        {
            var response = new Projeto
            {
                Nome = projetoRequest.Nome,
                Descricao = projetoRequest.Descricao ?? string.Empty,
                DataDeCriacao = DateTime.UtcNow,
                AlteradoPor = projetoRequest.Usuario ?? string.Empty,
            };
            
            return _DAL.Create<Projeto>(response).Id;
        }

        public void UpdateProject(int idProjeto, ProjetoRequest projetoAtualizado)
        {
            var _projeto = _DAL.GetById<Projeto>(idProjeto) ?? throw new ApplicationException("Projeto não encontrado");

            _projeto.Nome = projetoAtualizado.Nome;
            _projeto.Descricao = projetoAtualizado.Descricao ?? string.Empty;
            _projeto.AlteradoPor = projetoAtualizado.Usuario ?? string.Empty;

            _DAL.Update<Projeto>(_projeto);
        }

        public void DeleteProject(int idProjeto)
        {
            var _projeto = _DAL.GetById<Projeto>(idProjeto) ?? throw new ApplicationException("Projeto não encontrado");

            if (_projeto == null) return;

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

            Tarefa novaTarefa = new()
            {
                Titulo = task.Titulo,
                Descricao = task.Descricao,
                Usuario = task.UsuarioResponsavel,
                DataDeVencimento = task.DataDeVencimento,
                Prioridade = task.Prioridade,
                Status = Enums.Status.EmAndamento,                
            };

            _projeto.Tarefas.Add(novaTarefa);

            _DAL.Update<Projeto>(_projeto);
            _DAL.Create<TarefaHistorico>(novaTarefa.ToHistorico(task.CriadoPor, "Criação Tarefa"));

            return novaTarefa;
        }

        public Tarefa GetTaskByID(int idTarefa)
        {
            return _DAL.GetById<Tarefa>(idTarefa) ?? throw new ApplicationException("Tarefa não encontrada");
        }

        public void UpdateTask(int idTarefa, TarefaUpdateRequest request)
        {
            var tarefa = _DAL.GetById<Tarefa>(idTarefa) ?? throw new ApplicationException("Tarefa não encontrada");

            tarefa.Titulo = request.Titulo;
            tarefa.Descricao = request.Descricao;
            tarefa.DataDeVencimento = request.DataDeVencimento;
            tarefa.Status = request.Status;

            _DAL.Update<Tarefa>(tarefa);
            _DAL.Create<TarefaHistorico>(tarefa.ToHistorico(request.AlteradoPor, "Alteração Tarefa"));
        }

        public void AddComment(int idTask, ComentarioRequest request)
        {
            var tarefa = _DAL.GetById<Tarefa>(request.IdTarefa) ?? throw new ApplicationException("Tarefa não encontrada");

            tarefa.Comentarios.Add(request.ToComentario());

            _DAL.Update<Tarefa>(tarefa);
            _DAL.Create<TarefaHistorico>(tarefa.ToHistorico(request.Usuario, "Novo Comentário na Tarefa"));
        }

        public void CloseTask(int idTarefa)
        {
            var _tarefa = _DAL.GetById<Tarefa>(idTarefa);

            if (_tarefa == null) return;

            _DAL.Delete<Tarefa>(_tarefa);
        }
        #endregion Tarefas
    }
}
