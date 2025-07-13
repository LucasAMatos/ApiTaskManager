namespace ApiTaskManager.Models.Request;

public class ProjetoRequest
{
    public string? Nome { get; set; }

    public string? Descricao { get; set; }

    public required string Usuario { get; set; }
}
