using ApiTaskManager.Enums;

namespace ApiTaskManager.Models;

public class Projeto
{
    public int Id { get; set; }
    public required string Nome { get; set; }
    public DateTime DataDeCriacao { get; set; }
    public required string Descricao { get; set; }
    public string AlteradoPor { get; set; }
    public Status Status { get; set; }
}
