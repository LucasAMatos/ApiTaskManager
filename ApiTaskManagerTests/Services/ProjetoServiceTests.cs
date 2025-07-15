using ApiTaskManager.Interfaces.DAL;
using ApiTaskManager.Interfaces.Services;
using ApiTaskManager.Models.Request;
using ApiTaskManager.Models.Entity;
using ApiTaskManager.Enums;
using ApiTaskManager.Services;
using FluentAssertions;
using Moq;
using System.Linq.Expressions;

namespace ApiTaskManager.Tests.Services
{
    public class ProjetoServiceTests
    {
        private readonly Mock<IProjetoDAL> _mockProjetoDAL;
        private readonly Mock<IUsuarioService> _mockUsuarioService;
        private readonly ProjetoService _projetoService;

        public ProjetoServiceTests()
        {
            _mockProjetoDAL = new Mock<IProjetoDAL>();
            _mockUsuarioService = new Mock<IUsuarioService>();
            _projetoService = new ProjetoService(_mockProjetoDAL.Object, _mockUsuarioService.Object);
        }

        [Fact]
        public void GetProjectById_DeveRetornarProjetoQuandoExistir()
        {
            var projeto = new Projeto { 
                Id = 1, 
                Nome = "Projeto X", 
                Descricao = "Descricao", 
                AlteradoPor = new Usuario { Nome = "alterado" } 
            };

            _mockProjetoDAL.Setup(d => d.GetById<Projeto>(1)).Returns(projeto);

            var resultado = _projetoService.GetProjectById(1);

            resultado.Should().NotBeNull();
            resultado.Nome.Should().Be("Projeto X");
        }

        [Fact]
        public void CreateProject_DeveCriarProjetoERetornarId()
        {
            // Arrange
            var request = new ProjetoRequest
            {
                Nome = "Novo Projeto",
                Descricao = "Descrição",
                Usuario = "admin"
            };

            _mockProjetoDAL
                .Setup(d => d.Create(It.IsAny<Projeto>()))
                .Returns((Projeto p) => { p.Id = 99; return p; });

            _mockUsuarioService
                .Setup(d => d.GetUsuarioByName(request.Usuario))
                .Returns(new Usuario { Nome = request.Usuario });

            // Act
            var id = _projetoService.CreateProject(request);

            // Assert
            id.Should().Be(99);
        }

        [Fact]
        public void UpdateProject_ProjetoNaoExiste_DeveLancarExcecao()
        {
            var request = new ProjetoRequest { Nome = "Novo", Descricao = "x", Usuario = "admin" };

            _mockProjetoDAL
                .Setup(d => d.GetById<Projeto>(1))
                .Returns((Projeto)null);

            _mockUsuarioService
                .Setup(d => d.GetUsuarioByName(request.Usuario))
                .Returns(new Usuario { Nome = request.Usuario });

            Action act = () => _projetoService.UpdateProject(1, request);

            act.Should().Throw<ApplicationException>().WithMessage("Projeto não encontrado");
        }

        [Fact]
        public void UpdateProject_ComNomeEDescricao_DeveAtualizarTudo()
        {
            var projeto = new Projeto
            {
                Id = 1,
                Nome = "Antigo",
                Descricao = "Velho",
                AlteradoPor = new Usuario { Nome = "x" }
            };

            var request = new ProjetoRequest
            {
                Nome = "Novo",
                Descricao = "Atualizado",
                Usuario = "admin"
            };

            _mockProjetoDAL
                .Setup(d => d.GetById<Projeto>(1))
                .Returns(projeto);

            _mockUsuarioService
                .Setup(d => d.GetUsuarioByName(request.Usuario))
                .Returns(new Usuario { Nome = request.Usuario });

            _projetoService.UpdateProject(1, request);

            projeto.Nome.Should().Be("Novo");
            projeto.Descricao.Should().Be("Atualizado");
            projeto.AlteradoPor.Nome.Should().Be("admin");
            _mockProjetoDAL.Verify(d => d.Update(projeto), Times.Once);
        }

        [Fact]
        public void UpdateProject_SemNome_ComDescricao()
        {
            var projeto = new Projeto 
            { 
                Id = 2, 
                Nome = "Original", 
                Descricao = "Original", 
                AlteradoPor = new Usuario { Nome = "x" } 
            };

            var request = new ProjetoRequest
            {
                Nome = null,
                Descricao = "Nova descrição",
                Usuario = "admin"
            };

            _mockProjetoDAL
                .Setup(d => d.GetById<Projeto>(2))
                .Returns(projeto);

            _mockUsuarioService
                .Setup(d => d.GetUsuarioByName(request.Usuario))
                .Returns(new Usuario { Nome = request.Usuario });

            _projetoService.UpdateProject(2, request);

            projeto.Nome.Should().Be("Original");
            projeto.Descricao.Should().Be("Nova descrição");
            projeto.AlteradoPor.Nome.Should().Be("admin");
            _mockProjetoDAL.Verify(d => d.Update(projeto), Times.Once);
        }

        [Fact]
        public void UpdateProject_ComNome_SemDescricao()
        {
            var projeto = new Projeto 
            { 
                Id = 3, 
                Nome = "Old", 
                Descricao = "Old", 
                AlteradoPor = new Usuario { Nome = "x" } 
            };

            var request = new ProjetoRequest
            {
                Nome = "Atualizado",
                Descricao = null,
                Usuario = "admin"
            };

            _mockProjetoDAL
                .Setup(d => d.GetById<Projeto>(3))
                .Returns(projeto);

            _mockUsuarioService
                .Setup(d => d.GetUsuarioByName(request.Usuario))
                .Returns(new Usuario { Nome = request.Usuario });

            _projetoService.UpdateProject(3, request);

            projeto.Nome.Should().Be("Atualizado");
            projeto.Descricao.Should().Be("Old");
            projeto.AlteradoPor.Nome.Should().Be("admin");
            _mockProjetoDAL.Verify(d => d.Update(projeto), Times.Once);
        }

        [Fact]
        public void UpdateProject_SemNomeESemDescricao()
        {
            var projeto = new Projeto { Id = 4, Nome = "N", Descricao = "D", AlteradoPor = new Usuario { Nome = "x" } };

            var request = new ProjetoRequest
            {
                Nome = null,
                Descricao = null,
                Usuario = "admin"
            };

            _mockProjetoDAL
                .Setup(d => d.GetById<Projeto>(4))
                .Returns(projeto);

            _mockUsuarioService
                .Setup(d => d.GetUsuarioByName(request.Usuario))
                .Returns(new Usuario { Nome = request.Usuario });

            _projetoService.UpdateProject(4, request);

            projeto.Nome.Should().Be("N");
            projeto.Descricao.Should().Be("D");
            projeto.AlteradoPor.Nome.Should().Be("admin");
            _mockProjetoDAL.Verify(d => d.Update(projeto), Times.Once);
        }

        [Fact]
        public void DeleteProject_WithAllTasksCompleted_ShouldDeleteProject()
        {
            // Arrange
            var usuario = "Gerente";
            var projetoId = 1;

            var tarefa1 = new Tarefa()
            {
                Id = 1,
                Titulo = "Tarefa 1",
                Descricao = "Descricao 1",
                Usuario = new Usuario { Nome = "usuario" },
                Status = Status.Concluida
            };

            var projeto = new Projeto
            {
                Id = projetoId,
                Nome = "Projeto X",
                Descricao = "Descricao",
                AlteradoPor = new Usuario { Nome = "alterado" },
                Tarefas = new List<Tarefa>
                {
                    tarefa1
                }
            };

            _mockUsuarioService.Setup(x => x.GetUsuarioByName(usuario)).Returns(new Usuario { Nome = usuario });
            _mockProjetoDAL.Setup(x => x.GetById<Projeto>(projetoId, p => p.Tarefas)).Returns(projeto);
            _mockProjetoDAL.Setup(x => x.Delete<Projeto>(projeto));
            _mockProjetoDAL.Setup(d => d.GetById<Tarefa>(1)).Returns(tarefa1);

            // Act
            _projetoService.DeleteProject(projetoId, usuario);

            // Assert
            _mockProjetoDAL.Verify(x => x.Delete<Projeto>(projeto), Times.Once);
        }

        [Fact]
        public void DeleteProject_WithOpenTasks_ShouldThrowException()
        {
            // Arrange
            var usuario = "Gerente";
            var projetoId = 1;

            var tarefa = new Tarefa { Titulo = "Tarefa 1", Descricao = "Descricao 1", Usuario = new Usuario { Nome = "usuario" }, Status = Status.EmAndamento };

            var projeto = new Projeto
            {
                Id = projetoId,
                Nome = "Projeto X",
                Descricao = "Descricao",
                AlteradoPor = new Usuario { Nome = "alterado" },
                Tarefas = new List<Tarefa>
                {
                    tarefa,
                }
            };

            _mockUsuarioService.Setup(x => x.GetUsuarioByName(usuario)).Returns(new Usuario { Nome = usuario });
            _mockProjetoDAL.Setup(x => x.GetById<Projeto>(projetoId, p => p.Tarefas)).Returns(projeto);
            _mockProjetoDAL.Setup(d => d.GetById<Tarefa>(1)).Returns(tarefa);
            // Act & Assert
            var ex = Assert.Throws<ApplicationException>(() => _projetoService.DeleteProject(projetoId, usuario));
            Assert.Contains("Projeto possui 1 tarefas em aberto que devem ser finalizadas ou excluídas", ex.Message);
        }

        [Fact]
        public void DeleteProject_WithNoTasks_ShouldDeleteProject()
        {
            // Arrange
            var usuario = "Gerente";
            var projetoId = 1;

            var projeto = new Projeto
            {
                Id = projetoId,
                Nome = "Projeto X",
                Descricao = "Descricao",
                AlteradoPor = new Usuario { Nome = "alterado" },
                Tarefas = new List<Tarefa>()
            };

            _mockUsuarioService.Setup(x => x.GetUsuarioByName(usuario)).Returns(new Usuario { Nome = usuario });
            _mockProjetoDAL.Setup(x => x.GetById<Projeto>(projetoId, p => p.Tarefas)).Returns(projeto);

            // Act
            _projetoService.DeleteProject(projetoId, usuario);

            // Assert
            _mockProjetoDAL.Verify(x => x.Delete<Projeto>(projeto), Times.Once);
        }


        [Fact]
        public void GetAllProjectsByStatus_DeveRetornarFiltrados()
        {
            var projetos = new List<Projeto>
            {
                new() { Nome = "Projeto 1",AlteradoPor =new Usuario { Nome = "Usuario" } ,Descricao="descricao1", Status = Status.EmAndamento },
                new() { Nome = "Projeto 2",AlteradoPor =new Usuario { Nome = "Usuario" } ,Descricao="descricao2", Status = Status.Concluida },
            };

            _mockProjetoDAL.Setup(d => d.GetAll<Projeto>()).Returns(projetos);

            var resultado = _projetoService.GetAllProjectsByStatus(Status.Concluida);

            resultado.Should().BeEquivalentTo(new List<string> { "Projeto 2" });
        }
        [Fact]
        public void GetprojectTasksByStatus_ProjetoNaoExiste_DeveLancarExcecao()
        {
            // Arrange
            _mockProjetoDAL.Setup(d => d.GetById<Projeto>(10, It.IsAny<Expression<Func<Projeto, object>>>()))
                    .Returns((Projeto)null);

            // Act
            Action act = () => _projetoService.GetprojectTasksByStatus(10, Status.EmAndamento);

            // Assert
            act.Should().Throw<ApplicationException>().WithMessage("Projeto não encontrado");
        }

        [Fact]
        public void GetprojectTasksByStatus_DeveRetornarTarefasFiltradas()
        {
            // Arrange
            var tarefas = new List<Tarefa>
            {
                new() { Titulo = "Tarefa 1", Descricao="Descricao 1", Usuario= new Usuario{ Nome="usuario" }, Status = Status.EmAndamento },
                new() { Titulo = "Tarefa 2", Descricao="Descricao 2", Usuario=new Usuario { Nome = "usuario" }, Status = Status.Concluida },
                new() { Titulo = "Tarefa 3", Descricao="Descricao 3", Usuario=new Usuario { Nome = "usuario" }, Status = Status.EmAndamento }
            };

            var projeto = new Projeto { Id = 1, Nome = "Projeto 1", AlteradoPor = new Usuario { Nome = "Usuario" }, Descricao = "descricao1", Tarefas = tarefas };

            _mockProjetoDAL.Setup(d => d.GetById<Projeto>(1, It.IsAny<Expression<Func<Projeto, object>>>()))
                    .Returns(projeto);

            // Act
            var resultado = _projetoService.GetprojectTasksByStatus(1, Status.EmAndamento);

            // Assert
            resultado.Should().HaveCount(2);
            resultado.All(t => t.Status == Status.EmAndamento).Should().BeTrue();
        }

        [Fact]
        public void GetprojectTasksByStatus_NenhumaTarefaComStatus()
        {
            // Arrange
            var tarefas = new List<Tarefa>
            {
                new() { Titulo = "Tarefa 1", Descricao="Descricao 1", Usuario=new Usuario { Nome = "usuario" }, Status = Status.Concluida }
            };

            var projeto = new Projeto { Id = 2, Nome = "Projeto 1", AlteradoPor = new Usuario { Nome = "Usuario" }, Descricao = "descricao1", Tarefas = tarefas };

            _mockProjetoDAL.Setup(d => d.GetById<Projeto>(2, It.IsAny<Expression<Func<Projeto, object>>>()))
                    .Returns(projeto);

            // Act
            var resultado = _projetoService.GetprojectTasksByStatus(2, Status.EmAndamento);

            // Assert
            resultado.Should().BeEmpty();
        }

        [Fact]
        public void GetAllTasksByProject_ProjetoNaoExiste_DeveLancarExcecao()
        {
            // Arrange
            _mockProjetoDAL.Setup(d => d.GetById<Projeto>(10, It.IsAny<Expression<Func<Projeto, object>>>()))
                    .Returns((Projeto)null);

            // Act
            Action act = () => _projetoService.GetAllTasksByProject(10);

            // Assert
            act.Should().Throw<ApplicationException>().WithMessage("Projeto não encontrado");
        }

        [Fact]
        public void GetAllTasksByProject_ProjetoComTarefas_DeveRetornarTitulos()
        {
            // Arrange
            var tarefas = new List<Tarefa>
            {
                        new() { Titulo = "Tarefa 1", Descricao="Descricao 1", Usuario=new Usuario { Nome = "usuario" } },
                        new() { Titulo = "Tarefa 2", Descricao="Descricao 2", Usuario=new Usuario { Nome = "usuario" } },
            };

            var projeto = new Projeto { Id = 1, Nome = "Projeto X", Descricao = "Descricao", AlteradoPor = new Usuario { Nome = "alterado" }, Tarefas = tarefas };

            _mockProjetoDAL.Setup(d => d.GetById<Projeto>(1, It.IsAny<Expression<Func<Projeto, object>>>()))
                    .Returns(projeto);

            // Act
            var resultado = _projetoService.GetAllTasksByProject(1);

            // Assert
            resultado.Should().BeEquivalentTo(new List<string> { "Tarefa 1", "Tarefa 2" });
        }

        [Fact]
        public void GetAllTasksByProject_ProjetoSemTarefas_DeveRetornarListaVazia()
        {
            // Arrange
            var projeto = new Projeto { Id = 2, Nome = "Projeto X", Descricao = "Descricao", AlteradoPor = new Usuario { Nome = "alterado" }, Tarefas = new List<Tarefa>() };

            _mockProjetoDAL.Setup(d => d.GetById<Projeto>(2, It.IsAny<Expression<Func<Projeto, object>>>()))
                    .Returns(projeto);

            // Act
            var resultado = _projetoService.GetAllTasksByProject(2);

            // Assert
            resultado.Should().BeEmpty();
        }

        [Fact]
        public void CreateTask_ProjetoNaoExiste_DeveLancarExcecao()
        {
            _mockProjetoDAL.Setup(d => d.GetById<Projeto>(1, It.IsAny<Expression<Func<Projeto, object>>>()))
                    .Returns((Projeto)null);

            var request = new TarefaRequest { Titulo = "Nova Tarefa", CriadoPor = "admin", Descricao = "Descricao 1", UsuarioResponsavel = "usuario" };

            Action act = () => _projetoService.CreateTask(1, request);

            act.Should().Throw<ApplicationException>().WithMessage("Projeto não encontrado");
        }

        [Fact]
        public void CreateTask_ProjetoCom20Tarefas_DeveLancarExcecao()
        {
            var projeto = new Projeto
            {
                Id = 1,
                Nome = "Projeto Limite",
                Descricao = "Descricao",
                AlteradoPor = new Usuario { Nome = "alterado" },
                Tarefas = Enumerable.Range(1, 20).Select(i => new Tarefa { Titulo = $"T{i}", Usuario = new Usuario { Nome = "usuario" }, Descricao = $"Descricao{i}" }).ToList()
            };

            _mockProjetoDAL.Setup(d => d.GetById<Projeto>(1, It.IsAny<Expression<Func<Projeto, object>>>()))
                    .Returns(projeto);

            var request = new TarefaRequest { Titulo = "Nova", CriadoPor = "admin", Descricao = "Descricao 1", UsuarioResponsavel = "usuario" };

            Action act = () => _projetoService.CreateTask(1, request);

            act.Should().Throw<ApplicationException>().WithMessage("Projeto 1-Projeto Limite Possui quantidade máxima de tarefas atribuidas.");
        }

        [Fact]
        public void CreateTask_ProjetoValido_DeveCriarTarefaCorretamente()
        {
            var projeto = new Projeto
            {
                Id = 1,
                Nome = "Projeto A",
                Descricao = "Descricao",
                AlteradoPor = new Usuario { Nome = "alterado" },
                Tarefas = new List<Tarefa>()
            };

            _mockProjetoDAL.Setup(d => d.GetById<Projeto>(1, It.IsAny<Expression<Func<Projeto, object>>>()))
            .Returns(projeto);

            TarefaHistorico? historicoSalvo = null;
            _mockProjetoDAL.Setup(d => d.Create(It.IsAny<TarefaHistorico>()))
                    .Callback<TarefaHistorico>(h => historicoSalvo = h);

            var request = new TarefaRequest
            {
                Titulo = "Nova Tarefa",
                Descricao = "Descrição",
                UsuarioResponsavel = "dev",
                DataDeVencimento = new DateTime(2025, 12, 31),
                Prioridade = Prioridade.Alta,
                CriadoPor = "admin"
            };

            _mockUsuarioService
                .Setup(d => d.GetUsuarioByName(request.UsuarioResponsavel))
                .Returns(new Usuario { Nome = request.UsuarioResponsavel });

            _mockUsuarioService
                .Setup(d => d.GetUsuarioByName(request.CriadoPor))
                .Returns(new Usuario { Nome = request.CriadoPor });

            var resultado = _projetoService.CreateTask(1, request);

            // Assert
            resultado.Titulo.Should().Be("Nova Tarefa");
            resultado.Usuario.Nome.Should().Be("dev");
            resultado.Status.Should().Be(Status.EmAndamento);
            projeto.Tarefas.Should().Contain(resultado);

            _mockProjetoDAL.Verify(d => d.Update(projeto), Times.Once);
            _mockProjetoDAL.Verify(d => d.Create(It.IsAny<TarefaHistorico>()), Times.Once);

            historicoSalvo.Should().NotBeNull();
            historicoSalvo.DescricaoDaAlteracao.Should().Be("Criação Tarefa");
            historicoSalvo.AlteradoPor.Nome.Should().Be("admin");
        }

        [Fact]
        public void GetTaskByID_TarefaExiste_DeveRetornarTarefa()
        {
            // Arrange
            var tarefa = new Tarefa { Id = 1, Titulo = "Tarefa Teste", Descricao = "Descricao 1", Usuario = new Usuario { Nome = "usuario" } };

            _mockProjetoDAL.Setup(d => d.GetById<Tarefa>(1)).Returns(tarefa);

            // Act
            var resultado = _projetoService.GetTaskByID(1);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Titulo.Should().Be("Tarefa Teste");
        }

        [Fact]
        public void GetTaskByID_TarefaNaoExiste_DeveLancarExcecao()
        {
            // Arrange
            _mockProjetoDAL.Setup(d => d.GetById<Tarefa>(1)).Returns((Tarefa)null);

            // Act
            Action act = () => _projetoService.GetTaskByID(1);

            // Assert
            act.Should().Throw<ApplicationException>().WithMessage("Tarefa não encontrada");
        }

        [Fact]
        public void UpdateTask_TarefaNaoExiste_DeveLancarExcecao()
        {
            _mockProjetoDAL.Setup(d => d.GetById<Tarefa>(1)).Returns((Tarefa)null);

            var request = new TarefaUpdateRequest { AlteradoPor = "admin" };

            Action act = () => _projetoService.UpdateTask(1, request);

            act.Should().Throw<ApplicationException>().WithMessage("Tarefa não encontrada");
        }

        [Fact]
        public void UpdateTask_ComTodosOsCampos_DeveAtualizarETriggerHistorico()
        {
            // Arrange
            var tarefa = new Tarefa
            {
                Id = 1,
                Titulo = "Antigo",
                Descricao = "Desc",
                Usuario = new Usuario { Nome = "old_user" },
                DataDeVencimento = new DateTime(2025, 07, 13),
                Status = Status.EmAndamento
            };

            _mockProjetoDAL.Setup(d => d.GetById<Tarefa>(1)).Returns(tarefa);

            TarefaHistorico historicoCriado = null;

            _mockProjetoDAL.Setup(d => d.Create(It.IsAny<TarefaHistorico>()))
                    .Callback<TarefaHistorico>(h => historicoCriado = h);


            var novaData = DateTime.Today.AddDays(5);
            var request = new TarefaUpdateRequest
            {
                Titulo = "Novo Título",
                Descricao = "Nova Descrição",
                DataDeVencimento = novaData,
                Status = Status.Concluida,
                UsuarioResponsavel = "admin",
                AlteradoPor = "admin"
            };


            _mockUsuarioService
                .Setup(d => d.GetUsuarioByName(request.AlteradoPor))
                .Returns(new Usuario { Nome = request.AlteradoPor });

            // Act
            _projetoService.UpdateTask(1, request);

            // Assert
            tarefa.Titulo.Should().Be("Novo Título");
            tarefa.Descricao.Should().Be("Nova Descrição");
            tarefa.DataDeVencimento.Should().Be(novaData);
            tarefa.Status.Should().Be(Status.Concluida);
            tarefa.Usuario.Nome.Should().Be("admin");

            _mockProjetoDAL.Verify(d => d.Update(tarefa), Times.Once);
            _mockProjetoDAL.Verify(d => d.Create(It.IsAny<TarefaHistorico>()), Times.Once);

            historicoCriado.Should().NotBeNull();
            historicoCriado.AlteradoPor.Nome.Should().Be("admin");
            historicoCriado.DescricaoDaAlteracao.Should().Be("Alteração Tarefa");
        }

        [Fact]
        public void UpdateTask_ComCamposParciais_DeveAtualizarSomentePresentes()
        {
            var tarefa = new Tarefa
            {
                Id = 2,
                Titulo = "Original",
                Descricao = "Desc",
                Usuario = new Usuario { Nome = "user" },
                DataDeVencimento = DateTime.Today,
                Status = Status.EmAndamento
            };

            _mockProjetoDAL.Setup(d => d.GetById<Tarefa>(2)).Returns(tarefa);

            var request = new TarefaUpdateRequest
            {
                Titulo = null,
                Descricao = "Atualizado",
                DataDeVencimento = null,
                Status = null,
                UsuarioResponsavel = null,
                AlteradoPor = "admin"
            };

            _mockUsuarioService
                .Setup(d => d.GetUsuarioByName(request.AlteradoPor))
                .Returns(new Usuario { Nome = request.AlteradoPor });

            _projetoService.UpdateTask(2, request);

            tarefa.Titulo.Should().Be("Original");
            tarefa.Descricao.Should().Be("Atualizado");
            tarefa.Usuario.Nome.Should().Be("user");

            _mockProjetoDAL.Verify(d => d.Update(tarefa), Times.Once);
            _mockProjetoDAL.Verify(d => d.Create(It.IsAny<TarefaHistorico>()), Times.Once);
        }
        [Fact]
        public void AddComment_TarefaNaoExiste_DeveLancarExcecao()
        {
            // Arrange
            var request = new ComentarioRequest
            {
                IdTarefa = 1,
                Comentario = "Comentário",
                Usuario = "user"
            };

            _mockProjetoDAL.Setup(d => d.GetById<Tarefa>(request.IdTarefa)).Returns((Tarefa)null);

            _mockUsuarioService
                .Setup(d => d.GetUsuarioByName(request.Usuario))
                .Returns(new Usuario { Nome = request.Usuario });

            // Act
            Action act = () => _projetoService.AddComment(request.IdTarefa, request);

            // Assert
            act.Should().Throw<ApplicationException>().WithMessage("Tarefa não encontrada");
        }

        [Fact]
        public void AddComment_TarefaExiste_DeveAdicionarComentarioEAtualizar()
        {
            // Arrange
            var tarefa = new Tarefa
            {
                Id = 1,
                Titulo = "Titulo 1",
                Descricao = "Descrição",
                Usuario = new Usuario { Nome = "dev" },
                Comentarios = new List<Comentario>()
            };

            var request = new ComentarioRequest
            {
                IdTarefa = 1,
                Comentario = "Primeiro comentário",
                Usuario = "admin"
            };

            _mockProjetoDAL.Setup(d => d.GetById<Tarefa>(1)).Returns(tarefa);

            _mockUsuarioService
                .Setup(d => d.GetUsuarioByName(request.Usuario))
                .Returns(new Usuario { Nome = request.Usuario });

            TarefaHistorico historicoCriado = null;
            _mockProjetoDAL.Setup(d => d.Create(It.IsAny<TarefaHistorico>()))
                    .Callback<TarefaHistorico>(h => historicoCriado = h);

            // Act
            _projetoService.AddComment(1, request);

            // Assert
            tarefa.Comentarios.Should().HaveCount(1);
            tarefa.Comentarios[0].conteudo.Should().Be("Primeiro comentário");
            tarefa.Comentarios[0].Usuario.Nome.Should().Be("admin");

            _mockProjetoDAL.Verify(d => d.Update(tarefa), Times.Once);
            _mockProjetoDAL.Verify(d => d.Create(It.IsAny<TarefaHistorico>()), Times.Once);

            historicoCriado.Should().NotBeNull();
            historicoCriado.DescricaoDaAlteracao.Should().Be("Novo Comentário na Tarefa");
            historicoCriado.AlteradoPor.Nome.Should().Be("admin");
        }
        [Fact]
        public void CloseTask_TarefaExiste_DeveChamarDelete()
        {
            // Arrange
            var tarefa = new Tarefa {Id = 1, Titulo = "Tarefa 1", Descricao = "Descricao 1", Usuario = new Usuario { Nome = "usuario" } };

            _mockProjetoDAL.Setup(d => d.GetById<Tarefa>(1)).Returns(tarefa);

            _mockUsuarioService
                .Setup(d => d.GetUsuarioByName("usuario"))
                .Returns(new Usuario { Nome = "usuario" });

            // Act
            _projetoService.CloseTask(1, "usuario");

            // Assert
            _mockProjetoDAL.Verify(d => d.Delete(tarefa), Times.Once);
        }

        [Fact]
        public void CloseTask_TarefaNaoExiste_DeveIgnorarSemErro()
        {
            // Arrange
            _mockProjetoDAL.Setup(d => d.GetById<Tarefa>(1)).Returns((Tarefa)null);

            _mockUsuarioService
                .Setup(d => d.GetUsuarioByName("usuario"))
                .Returns(new Usuario { Nome = "usuario" });

            // Act
            var act = () => _projetoService.CloseTask(1, "usuario");

            // Assert
            act.Should().Throw<ApplicationException>().WithMessage("Tarefa não encontrada");
        }

        [Fact]
        public void GetHistoryTasksByProject_ProjetoExiste_DeveRetornarHistoricoFiltrado()
        {
            // Arrange
            int idProjeto = 1;
            var historicos = new List<TarefaHistorico>
        {
            new() { Id = 1, IdProjeto = 1, DescricaoDaAlteracao = "Criada", AlteradoPor = new Usuario { Nome="usuario" }, Descricao = "Descricao", Titulo="Titulo", Usuario = new Usuario { Nome="usuario" } },
            new() { Id = 2, IdProjeto = 2, DescricaoDaAlteracao = "Editada", AlteradoPor = new Usuario { Nome="usuario" }, Descricao = "Descricao", Titulo="Titulo", Usuario = new Usuario { Nome="usuario" }   },
            new() { Id = 3, IdProjeto = 1, DescricaoDaAlteracao = "Comentada", AlteradoPor = new Usuario { Nome="usuario" }, Descricao = "Descricao", Titulo="Titulo", Usuario = new Usuario { Nome="usuario" }   }
        };

            var projeto = new Projeto { Id = 1, Nome = "Projeto 1", AlteradoPor = new Usuario { Nome = "Gerente" }, Descricao="Projeto X" };

            _mockProjetoDAL.Setup(d => d.GetById<Projeto>(idProjeto)).Returns(projeto);
            _mockProjetoDAL.Setup(d => d.GetAll<TarefaHistorico>()).Returns(historicos);

            // Act
            var resultado = _projetoService.GetHistoryTasksByProject(idProjeto);

            // Assert
            resultado.Should().HaveCount(2);
            resultado.All(h => h.IdProjeto == idProjeto).Should().BeTrue();
        }

        [Fact]
        public void GetHistoryTasksByProject_ProjetoNaoExiste_DeveLancarExcecao()
        {
            // Arrange
            int idProjeto = 999;
            _mockProjetoDAL.Setup(d => d.GetById<Projeto>(idProjeto)).Returns((Projeto)null);

            // Act
            Action act = () => _projetoService.GetHistoryTasksByProject(idProjeto);

            // Assert
            act.Should().Throw<ApplicationException>().WithMessage("Projeto não encontrado");
        }
        [Fact]
        public void GetAllProjects_DeveRetornarNomesDosProjetos()
        {
            // Arrange
            var projetos = new List<Projeto>
            {
                new() { Nome = "Projeto A", AlteradoPor= new Usuario{ Nome="usuario" }, Descricao="ProjetoA" },
                new() { Nome = "Projeto B", AlteradoPor= new Usuario{ Nome="usuario" }, Descricao="ProjetoA"  },
                new() { Nome = "Projeto C", AlteradoPor= new Usuario{ Nome="usuario" }, Descricao="ProjetoA"  }
            };

            _mockProjetoDAL
                .Setup(d => d.GetAll<Projeto>())
                .Returns(projetos);

            // Act
            var resultado = _projetoService.GetAllProjects();

            // Assert
            resultado.Should().BeEquivalentTo(new List<string> { "Projeto A", "Projeto B", "Projeto C" });
        }

        [Fact]
        public void GetAllProjects_SemProjetos_DeveRetornarListaVazia()
        {
            // Arrange
            _mockProjetoDAL
                .Setup(d => d.GetAll<Projeto>())
                .Returns(new List<Projeto>());

            // Act
            var resultado = _projetoService.GetAllProjects();

            // Assert
            resultado.Should().BeEmpty();
        }

    }
}