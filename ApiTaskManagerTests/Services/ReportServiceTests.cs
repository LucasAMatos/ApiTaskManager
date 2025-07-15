using ApiTaskManager.Models.Request;
using ApiTaskManager.Models.Entity;
using ApiTaskManager.Enums;
using ApiTaskManager.Services;
using FluentAssertions;
using Moq;
using System.Linq.Expressions;
using ApiTaskManager.Interfaces.DAL;
using ApiTaskManager.Interfaces.Services;
using ApiTaskManager.Models.Response;

namespace ApiTaskManager.Tests.Services
{

    public class ReportServiceTests
    {
        private readonly Mock<IUsuarioService> _usuarioServiceMock;
        private readonly Mock<IProjetoService> _projetoServiceMock;
        private readonly ReportService _reportService;

        public ReportServiceTests()
        {
            _usuarioServiceMock = new Mock<IUsuarioService>();
            _projetoServiceMock = new Mock<IProjetoService>();
            _reportService = new ReportService(_projetoServiceMock.Object, _usuarioServiceMock.Object);
        }

        [Fact]
        public void Should_Throw_When_NonManager_User_Tries_To_Use_Report()
        {
            // Arrange
            _usuarioServiceMock.Setup(x => x.GetUsuarioByName("user")).Returns(new Usuario { Nome = "user", Cargo = "Dev" });

            var request = new ReportAverageClosedTasksByUserRequest
            {
                UsuarioSolicitante = "user",
                UsuarioConsultado = "any",
                IdProjeto = 1
            };

            // Act & Assert
            var ex = Assert.Throws<ApplicationException>(() => _reportService.AverageClosedTasksByUser(request));
            Assert.Equal("Apenas usuários gerentes podem acessar essa rotina", ex.Message);
        }

        [Fact]
        public void Should_Throw_When_User_Has_No_History()
        {
            // Arrange
            _usuarioServiceMock.Setup(x => x.GetUsuarioByName("gerente")).Returns(new Usuario { Nome = "gerente", Cargo = "Gerente" });
            _usuarioServiceMock.Setup(x => x.GetUsuarioByName("dev")).Returns(new Usuario { Nome = "dev", Cargo = "Dev" });

            _projetoServiceMock.Setup(x => x.GetHistoryTasksByProject(1)).Returns(new List<TarefaHistorico>());

            var request = new ReportAverageClosedTasksByUserRequest
            {
                UsuarioSolicitante = "gerente",
                UsuarioConsultado = "dev",
                IdProjeto = 1
            };

            // Act & Assert
            var ex = Assert.Throws<ApplicationException>(() => _reportService.AverageClosedTasksByUser(request));
            Assert.Contains("Usuário não tem histórico", ex.Message);
        }

        [Fact]
        public void Should_Return_Report_Successfully()
        {
            // Arrange
            _usuarioServiceMock.Setup(x => x.GetUsuarioByName("gerente")).Returns(new Usuario { Nome = "gerente", Cargo = "Gerente" });
            _usuarioServiceMock.Setup(x => x.GetUsuarioByName("dev")).Returns(new Usuario { Nome = "dev", Cargo = "Dev" });

            var now = DateTime.Now;

            var tasks = new List<TarefaHistorico>
            {
                new TarefaHistorico { IdTarefa = 1, Titulo="Titulo 1",AlteradoPor = new Usuario { Nome = "dev" },Descricao = "descricao 1", Status = Status.Concluida, Usuario = new Usuario { Nome = "dev" }, DataAlteracao = now },
                new TarefaHistorico { IdTarefa = 2, Titulo="Titulo 2",AlteradoPor = new Usuario { Nome = "dev" },Descricao = "descricao 2",  Status = Status.EmAndamento, Usuario = new Usuario { Nome = "dev" }, DataAlteracao = now },
                new TarefaHistorico { IdTarefa = 3, Titulo="Titulo 3",AlteradoPor = new Usuario { Nome = "dev" },Descricao = "descricao 3",  Status = Status.Concluida, Usuario = new Usuario { Nome = "dev" }, DataAlteracao = now.AddMonths(-2) }
            };

            _projetoServiceMock.Setup(x => x.GetHistoryTasksByProject(1)).Returns(tasks);

            var request = new ReportAverageClosedTasksByUserRequest
            {
                UsuarioSolicitante = "gerente",
                UsuarioConsultado = "dev",
                IdProjeto = 1
            };

            // Act
            var result = _reportService.AverageClosedTasksByUser(request);

            // Assert
            Assert.Equal("dev", result.Name);
            Assert.Equal(3, result.TarefasNoProjeto);
            Assert.Equal(2, result.TarefasConcluidas);
            Assert.Equal(1, result.TarefasEmAberto);
            Assert.Equal(1, result.MediaDeTarefasMes); // Só uma tarefa concluída no último mês
        }
    }
}