using ApiTaskManager.Enums;

namespace ApiTaskManager.Models.Request
{
    public class TarefaUpdateRequest
    {
        public required string Titulo { get; set; }
        public required string Descricao { get; set; }
        public DateOnly DataDeVencimento { get; set; }
        public required string UsuarioResponsavel { get; set; }
        public required Status Status { get; set; }
        public required string AlteradoPor { get; set; }
    }
}
