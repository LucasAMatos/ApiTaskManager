namespace ApiTaskManager.Models.Request
{
    public class ComentarioRequest
    {
        public int IdTarefa { get; set; }
        public required string Comentario { get; set; }
        public required string Usuario { get; set; }
    }
}
