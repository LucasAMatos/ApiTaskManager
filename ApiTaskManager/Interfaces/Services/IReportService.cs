using ApiTaskManager.Models.Entity;
using ApiTaskManager.Models.Response;

namespace ApiTaskManager.Interfaces.Services
{
    public interface IReportService
    {
        ReportAverageClosedTasksByUserResponse AverageClosedTasksByUser(ReportAverageClosedTasksByUserRequest request);
    }
}
