using ApiTaskManager.Enums;
using ApiTaskManager.Extensions;
using ApiTaskManager.Interfaces.DAL;
using ApiTaskManager.Interfaces.Services;
using ApiTaskManager.Models.Entity;
using ApiTaskManager.Models.Request;

namespace ApiTaskManager.Services;

public class ProjetoService : IProjetoService
{
    private readonly IProjetoDAL _ProjetoDAL;
    private readonly IUsuarioService _usuarioService;

    public ProjetoService(IProjetoDAL projetoDAL, IUsuarioService usuarioService)
    {
        _ProjetoDAL = projetoDAL;
        _usuarioService = usuarioService;
    }

    #region Projetos
    public List<string> GetAllProjects() => [.. _ProjetoDAL.GetAll<Projeto>().Select(p => p.Nome)];

    public List<string> GetAllProjectsByStatus(Status status) => [.. _ProjetoDAL.GetAll<Projeto>().Where(p => p.Status == status).Select(p => p.Nome)];

    public Projeto? GetProjectById(int idProjeto) => _ProjetoDAL.GetById<Projeto>(idProjeto) ?? throw new ApplicationException("Projeto não encontrado");

    public Projeto GetProjectByIdWithTasks(int idProjeto) => _ProjetoDAL.GetById<Projeto>(idProjeto, p => p.Tarefas) ?? throw new ApplicationException("Projeto não encontrado");

    public List<Tarefa> GetprojectTasksByStatus(int idProjeto, Status status) => [.. GetProjectByIdWithTasks(idProjeto).Tarefas.Where(t => t.Status == status)];

    public List<string> GetAllTasksByProject(int idProjeto) => [.. GetProjectByIdWithTasks(idProjeto).Tarefas.Select(t => t.Titulo)];

    public int CreateProject(ProjetoRequest projetoRequest)
    {
        var _usuario = _usuarioService.GetUsuarioByName(projetoRequest.Usuario);

        var response = new Projeto
        {
            Nome = projetoRequest.Nome,
            Descricao = projetoRequest.Descricao ?? string.Empty,
            DataDeCriacao = DateTime.Now,
            AlteradoPor = _usuario,
            UltimaAlteracao = DateTime.Now
        };

        return _ProjetoDAL.Create<Projeto>(response).Id;
    }

    public void UpdateProject(int idProjeto, ProjetoRequest projetoAtualizado)
    {
        var _usuario = _usuarioService.GetUsuarioByName(projetoAtualizado.Usuario);
        var _projeto = _ProjetoDAL.GetById<Projeto>(idProjeto) ?? throw new ApplicationException("Projeto não encontrado");

        if (projetoAtualizado.Nome != null) _projeto.Nome = projetoAtualizado.Nome;
        if (projetoAtualizado.Descricao != null) _projeto.Descricao = projetoAtualizado.Descricao;
        _projeto.AlteradoPor = _usuario;
        _projeto.UltimaAlteracao = DateTime.Now;

        _ProjetoDAL.Update<Projeto>(_projeto);
    }

    public void DeleteProject(int idProjeto, string usuario)
    {
        _usuarioService.GetUsuarioByName(usuario);

        var _projeto = GetProjectByIdWithTasks(idProjeto);

        if (_projeto.Tarefas != null && _projeto.Tarefas.Count > 0)
        {
            throw new ApplicationException($"Projeto Possui {_projeto.Tarefas.Count} Tarefas em Aberto que devem ser finalizadas ou excluídas");
        }

        _ProjetoDAL.Delete<Projeto>(_projeto);
    }


    #endregion Projetos

    #region Tarefas
    public Tarefa CreateTask(int idProjeto, TarefaRequest task)
    {
        var _projeto = GetProjectByIdWithTasks(idProjeto);

        if (_projeto.Tarefas.Count == 20)
        {
            throw new ApplicationException($"Projeto {idProjeto}-{_projeto.Nome} Possui quantidade máxima de tarefas atribuidas.");
        }

        var _usuario = _usuarioService.GetUsuarioByName(task.UsuarioResponsavel);
        var _criadoPor = _usuarioService.GetUsuarioByName(task.CriadoPor);

        Tarefa novaTarefa = new()
        {
            Titulo = task.Titulo,
            Descricao = task.Descricao,
            Usuario = _usuario,
            DataDeVencimento = task.DataDeVencimento,
            Prioridade = task.Prioridade,
            Status = Enums.Status.EmAndamento,
            UltimaAlteracao = DateTime.Now,
        };

        _projeto.Tarefas.Add(novaTarefa);

        _ProjetoDAL.Update<Projeto>(_projeto);
        _ProjetoDAL.Create<TarefaHistorico>(novaTarefa.ToHistorico(_criadoPor, "Criação Tarefa"));

        return novaTarefa;
    }

    public Tarefa GetTaskByID(int idTarefa) => _ProjetoDAL.GetById<Tarefa>(idTarefa) ?? throw new ApplicationException("Tarefa não encontrada");

    public void UpdateTask(int idTarefa, TarefaUpdateRequest request)
    {
        var tarefa = _ProjetoDAL.GetById<Tarefa>(idTarefa) ?? throw new ApplicationException("Tarefa não encontrada");

        var _AlteradoPor = _usuarioService.GetUsuarioByName(request.AlteradoPor);

        if (request.Titulo != null) tarefa.Titulo = request.Titulo;
        if (request.Descricao != null) tarefa.Descricao = request.Descricao;
        if (request.DataDeVencimento.HasValue) tarefa.DataDeVencimento = request.DataDeVencimento.Value;
        if (request.Status.HasValue) tarefa.Status = request.Status.Value;
        if (request.UsuarioResponsavel != null) tarefa.Usuario = _usuarioService.GetUsuarioByName(request.UsuarioResponsavel);

        _ProjetoDAL.Update<Tarefa>(tarefa);
        _ProjetoDAL.Create<TarefaHistorico>(tarefa.ToHistorico(_AlteradoPor, "Alteração Tarefa"));
    }

    public void AddComment(int idTarefa, ComentarioRequest request)
    {
        var _usuarioComentario = _usuarioService.GetUsuarioByName(request.Usuario);
        var tarefa = GetTaskByID(idTarefa);

        tarefa.Comentarios.Add(request.ToComentario(_usuarioComentario));
        tarefa.UltimaAlteracao = DateTime.Now;

        _ProjetoDAL.Update<Tarefa>(tarefa);
        _ProjetoDAL.Create<TarefaHistorico>(tarefa.ToHistorico(_usuarioComentario, "Novo Comentário na Tarefa"));
    }

    public void CloseTask(int idTarefa, string usuario)
    {
        var _usuario = _usuarioService.GetUsuarioByName(usuario);

        var _tarefa = GetTaskByID(idTarefa);

        _tarefa.Status = Status.Concluida;
        _tarefa.UltimaAlteracao = DateTime.Now;

        _ProjetoDAL.Create<TarefaHistorico>(_tarefa.ToHistorico(_usuario, "Finalizando Tarefa"));
        _ProjetoDAL.Delete<Tarefa>(_tarefa);
    }
    #endregion Tarefas

    #region TarefaHistorico

    public List<TarefaHistorico> GetHistoryTasksByProject(int idProjeto)
    {
        GetProjectById(idProjeto);

        return [.. _ProjetoDAL.GetAll<TarefaHistorico>().Where(th => th.IdProjeto == idProjeto)];
    }
    #endregion
}
