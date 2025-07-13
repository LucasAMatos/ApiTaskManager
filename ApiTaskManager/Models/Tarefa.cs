using ApiTaskManager.Enums;

namespace ApiTaskManager.Models;
public class Tarefa
{
    public int Id { get; set; }
    public required string Titulo { get; set; }
    public required string Descricao { get; set; }
    public DateTime DataDeVencimento { get; set; }
    public Status Status { get; set; }
    public required string Usuario { get; set; }
    public Prioridade Prioridade { get; set; }
    public List<Comentario> Comentarios { get; set; } = [];
    public int ProjetoId { get; set; }
    public Projeto Projeto { get; set; } = null!;
}
