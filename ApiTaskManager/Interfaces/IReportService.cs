using ApiTaskManager.Models;

namespace ApiTaskManager.Interfaces
{
    public interface IReportService
    {
        List<Tarefa> AverageClosedTasksByUser(int idProjeto, int dias, string usuario);
    }
}
