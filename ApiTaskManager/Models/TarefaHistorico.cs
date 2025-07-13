using ApiTaskManager.Enums;

namespace ApiTaskManager.Models;
public class TarefaHistorico
{
    public int Id { get; set; }
    public int IdTarefa { get; set; }
    public required string Titulo { get; set; }
    public required string Descricao { get; set; }
    public DateTime DataDeVencimento { get; set; }
    public Status Status { get; set; }
    public required string Usuario { get; set; }
    public Prioridade Prioridade { get; set; }
    public int ProjetoId { get; set; }
    public required string AlteradoPor { get; set; }
    public string DescricaoDaAlteracao { get; set; }
    public DateTime DataAlteracao { get; set; }
}
