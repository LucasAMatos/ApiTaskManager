using ApiTaskManager.Interfaces;
using ApiTaskManager.Models;
using FluentValidation.Validators;

namespace ApiTaskManager.Services
{
    public class ReportService(
        IProjetoService projetoService,
        IUsuarioService usuarioService) : IReportService
    {
        public List<Tarefa> AverageClosedTasksByUser(int idProjeto, int dias, string usuario)
        {
            var _usuario = usuarioService.GetUsuarioByName(usuario);

            ValidAccessLevel(_usuario);

            var tasksByProject = projetoService.GetprojectTasksByStatus(idProjeto, Enums.Status.Concluida);

            if (tasksByProject == null || tasksByProject.Count == 0)
                return [];

            return [.. tasksByProject.Where(t => t.UltimaAlteracao >= DateTime.Today.AddDays(-30))];
        }

        private static void ValidAccessLevel(Usuario usuario)
        {
            if (usuario.Cargo != "Gerente")
            {
                throw new ApplicationException("Apenas usuários gerentes podem acessar essa rotina");
            }
        }
    }
}
