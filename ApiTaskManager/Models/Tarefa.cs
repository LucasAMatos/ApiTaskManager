namespace ApiTaskManager.Models;
public class Tarefa
{
    public int Id { get; set; }

    public required string Titulo { get; set; }

    public required string Descricao { get; set; }

    public DateOnly DataDeVencimento { get; set; }

    public int Status { get; set; }

    public Usuario usuario { get; set; }
}
