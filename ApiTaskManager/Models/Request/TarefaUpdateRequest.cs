using ApiTaskManager.Enums;

namespace ApiTaskManager.Models.Request
{
    public class TarefaUpdateRequest
    {
        public string? Titulo { get; set; }
        public string? Descricao { get; set; }
        public DateOnly? DataDeVencimento { get; set; }
        public string? UsuarioResponsavel { get; set; }
        public Status? Status { get; set; }
        public required string AlteradoPor { get; set; }
    }
}
