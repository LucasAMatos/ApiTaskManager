using ApiTaskManager.Interfaces.Services;
using ApiTaskManager.Models.Entity;
using ApiTaskManager.Models.Response;
using FluentValidation.Validators;

namespace ApiTaskManager.Services
{
    public class ReportService : IReportService
    {
        private readonly IUsuarioService _usuarioService;
        private readonly IProjetoService _projetoService;

        public ReportService(
        IProjetoService projetoService,
        IUsuarioService usuarioService)
        {
            _projetoService = projetoService;
            _usuarioService = usuarioService;
        }
        public ReportAverageClosedTasksByUserResponse AverageClosedTasksByUser(ReportAverageClosedTasksByUserRequest request)
        {
            var _usuarioSolicitante = _usuarioService.GetUsuarioByName(request.UsuarioSolicitante);
            
            ValidAccessLevel(_usuarioSolicitante);

            _usuarioService.GetUsuarioByName(request.UsuarioConsultado);

            var tasksByProjectFromUser = _projetoService.GetHistoryTasksByProject(request.IdProjeto).Where(t => t.Usuario.Nome == request.UsuarioConsultado);

            if (tasksByProjectFromUser == null || !tasksByProjectFromUser.Any())
            {
                throw new ApplicationException($"Usuário não tem histórico de tarefas no projeto {request.IdProjeto}");
            }

            return new ReportAverageClosedTasksByUserResponse
            {
                Name = request.UsuarioConsultado,
                TarefasNoProjeto = tasksByProjectFromUser.Select(t => t.IdTarefa).Distinct().Count(),
                TarefasConcluidas = tasksByProjectFromUser.Where(t => t.Status == Enums.Status.Concluida).Select(t => t.IdTarefa).Distinct().Count(),
                TarefasEmAberto = tasksByProjectFromUser.Where(t => t.Status != Enums.Status.Concluida).Select(t => t.IdTarefa).Distinct().Count(),
                MediaDeTarefasMes = tasksByProjectFromUser.Count(t => t.Status == Enums.Status.Concluida && t.DataAlteracao >= DateTime.Now.AddDays(-30))
            };
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
