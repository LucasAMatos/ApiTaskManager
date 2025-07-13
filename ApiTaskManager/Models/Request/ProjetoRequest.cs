namespace ApiTaskManager.Models.Request;

public class ProjetoRequest
{
    public required string Nome { get; set; }

    public string? Descricao { get; set; }

    public string? Usuario { get; set; }
}
