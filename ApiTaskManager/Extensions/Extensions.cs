using ApiTaskManager.Models;
using ApiTaskManager.Models.Request;

namespace ApiTaskManager.Extensions;

public static class Extensions
{
    public static TarefaHistorico ToHistorico(this Tarefa tarefa, Usuario alteradoPor, string descricaoDaAlteracao)
    {
        return new TarefaHistorico
        {
            IdTarefa = tarefa.Id,
            Titulo = tarefa.Titulo,
            Descricao = tarefa.Descricao,
            DataDeVencimento = tarefa.DataDeVencimento,
            Status = tarefa.Status,
            Usuario = tarefa.Usuario,
            Prioridade = tarefa.Prioridade,
            ProjetoId = tarefa.ProjetoId,
            AlteradoPor = alteradoPor,
            DescricaoDaAlteracao = descricaoDaAlteracao,
            DataAlteracao = DateTime.UtcNow
        };
    }

    public static Comentario ToComentario(this ComentarioRequest comentarioRequest, Usuario usuarioComentario)
    {
        return new Comentario
        {
            conteudo = comentarioRequest.Comentario,
            IdTarefa = comentarioRequest.IdTarefa,
            Usuario = usuarioComentario,
        };
    }
}