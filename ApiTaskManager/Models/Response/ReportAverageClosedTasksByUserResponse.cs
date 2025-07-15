namespace ApiTaskManager.Models.Response
{
    public class ReportAverageClosedTasksByUserResponse
    {
        public string Name { get; set; }
        public int TarefasNoProjeto { get; set; }
        public int TarefasEmAberto { get; set; }
        public int TarefasConcluidas { get; set; }
        public int MediaDeTarefasMes { get; set; }
    }
}
