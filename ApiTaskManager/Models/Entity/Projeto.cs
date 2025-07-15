using ApiTaskManager.Enums;

namespace ApiTaskManager.Models.Entity;

public class Projeto
{
    public int Id { get; set; }
    public required string Nome { get; set; }
    public DateTime DataDeCriacao { get; set; }
    public required string Descricao { get; set; }
    public required Usuario AlteradoPor { get; set; }
    public Status Status { get; set; }
    public List<Tarefa> Tarefas { get; set; } = [];
    public DateTime UltimaAlteracao { get; set; }
}
