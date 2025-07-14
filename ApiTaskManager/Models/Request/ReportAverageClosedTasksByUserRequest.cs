namespace ApiTaskManager.Models.Response
{
    public class ReportAverageClosedTasksByUserRequest
    {
        public required string UsuarioSolicitante { get; set; } 
        public int IdProjeto { get; set; }
        public required string UsuarioConsultado { get; set; }
    }
}
