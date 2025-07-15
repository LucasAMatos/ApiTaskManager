using ApiTaskManager.Enums;

namespace ApiTaskManager.Models.Entity;

public class TarefaHistorico
{
    public int Id { get; set; }
    public int IdTarefa { get; set; }
    public required string Titulo { get; set; }
    public required string Descricao { get; set; }
    public DateTime DataDeVencimento { get; set; }
    public Status Status { get; set; }
    public required Usuario Usuario { get; set; }
    public Prioridade Prioridade { get; set; }
    public int IdProjeto { get; set; }
    public required Usuario AlteradoPor { get; set; }
    public string DescricaoDaAlteracao { get; set; }
    public DateTime DataAlteracao { get; set; }
}
