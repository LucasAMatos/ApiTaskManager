namespace ApiTaskManager.Models;

public class Projeto
{
    public int Id { get; set; }
    public string Nome { get; set; } = null!;
    public DateTime DataDeCriacao { get; set; }
    public string Descricao { get; set; } = null!;
}
