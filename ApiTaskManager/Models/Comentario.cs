namespace ApiTaskManager.Models
{
    public class Comentario
    {
        public int Id { get; set; }
        public string conteudo { get; set; }
        public string Usuario { get; set; }
        public int IdTarefa { get; set; }
        public Tarefa Tarefa { get; set; } = null!;
    }
}
