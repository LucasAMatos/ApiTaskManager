using Xunit;
using Moq;
using FluentAssertions;
using System.Collections.Generic;
using ApiTaskManager.Models;
using ApiTaskManager.Models.Request;
using ApiTaskManager.Enums;
using ApiTaskManager.Interfaces;
using ApiTaskManager.Services;
using System;
using System.Linq.Expressions;

namespace ApiTaskManager.Tests.Services
{
    public class ProjetoServiceTests
    {
        private readonly Mock<IDAL> _mockDal;
        private readonly ProjetoService _service;

        public ProjetoServiceTests()
        {
            _mockDal = new Mock<IDAL>();
            _service = new ProjetoService(_mockDal.Object);
        }

        [Fact]
        public void GetProjectById_DeveRetornarProjetoQuandoExistir()
        {
            var projeto = new Projeto { Id = 1, Nome = "Projeto X", Descricao = "Descricao", AlteradoPor = new Usuario { Nome = "alterado" } };
            _mockDal.Setup(d => d.GetById<Projeto>(1)).Returns(projeto);

            var resultado = _service.GetProjectById(1);

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

            _mockDal.Setup(d => d.Create(It.IsAny<Projeto>()))
                .Returns((Projeto p) => { p.Id = 99; return p; });

            _mockDal.Setup(d => d.GetAll<Usuario>()).Returns(new List<Usuario> { new Usuario { Nome = "admin" } });

            // Act
            var id = _service.CreateProject(request);

            // Assert
            id.Should().Be(99);
        }

        [Fact]
        public void UpdateProject_ProjetoNaoExiste_DeveLancarExcecao()
        {
            _mockDal.Setup(d => d.GetById<Projeto>(1)).Returns((Projeto)null);

            _mockDal.Setup(d => d.GetAll<Usuario>()).Returns(new List<Usuario> { new Usuario { Nome = "admin" } });

            var request = new ProjetoRequest { Nome = "Novo", Descricao = "x", Usuario = "admin" };

            Action act = () => _service.UpdateProject(1, request);

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

            _mockDal.Setup(d => d.GetById<Projeto>(1)).Returns(projeto);

            _mockDal.Setup(d => d.GetAll<Usuario>()).Returns(new List<Usuario> { new Usuario { Nome = "admin" } });

            var request = new ProjetoRequest
            {
                Nome = "Novo",
                Descricao = "Atualizado",
                Usuario = "admin"
            };

            _service.UpdateProject(1, request);

            projeto.Nome.Should().Be("Novo");
            projeto.Descricao.Should().Be("Atualizado");
            projeto.AlteradoPor.Nome.Should().Be("admin");
            _mockDal.Verify(d => d.Update(projeto), Times.Once);
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

            _mockDal.Setup(d => d.GetById<Projeto>(2)).Returns(projeto);

            _mockDal.Setup(d => d.GetAll<Usuario>()).Returns(new List<Usuario> { new Usuario { Nome = "admin" } });

            var request = new ProjetoRequest
            {
                Nome = null,
                Descricao = "Nova descrição",
                Usuario = "admin"
            };

            _service.UpdateProject(2, request);

            projeto.Nome.Should().Be("Original");
            projeto.Descricao.Should().Be("Nova descrição");
            projeto.AlteradoPor.Nome.Should().Be("admin");
            _mockDal.Verify(d => d.Update(projeto), Times.Once);
        }

        [Fact]
        public void UpdateProject_ComNome_SemDescricao()
        {
            var projeto = new Projeto { Id = 3, Nome = "Old", Descricao = "Old", AlteradoPor = new Usuario { Nome = "x" } };

            _mockDal.Setup(d => d.GetById<Projeto>(3)).Returns(projeto);

            _mockDal.Setup(d => d.GetAll<Usuario>()).Returns(new List<Usuario> { new Usuario { Nome = "admin" } });
            var request = new ProjetoRequest
            {
                Nome = "Atualizado",
                Descricao = null,
                Usuario = "admin"
            };

            _service.UpdateProject(3, request);

            projeto.Nome.Should().Be("Atualizado");
            projeto.Descricao.Should().Be("Old");
            projeto.AlteradoPor.Nome.Should().Be("admin");
            _mockDal.Verify(d => d.Update(projeto), Times.Once);
        }

        [Fact]
        public void UpdateProject_SemNomeESemDescricao()
        {
            var projeto = new Projeto { Id = 4, Nome = "N", Descricao = "D", AlteradoPor = new Usuario { Nome = "x" } };

            _mockDal.Setup(d => d.GetById<Projeto>(4)).Returns(projeto);

            _mockDal.Setup(d => d.GetAll<Usuario>()).Returns(new List<Usuario> { new Usuario { Nome = "admin" } });

            var request = new ProjetoRequest
            {
                Nome = null,
                Descricao = null,
                Usuario = "admin"
            };

            _service.UpdateProject(4, request);

            projeto.Nome.Should().Be("N");
            projeto.Descricao.Should().Be("D");
            projeto.AlteradoPor.Nome.Should().Be("admin");
            _mockDal.Verify(d => d.Update(projeto), Times.Once);
        }

        [Fact]
        public void DeleteProject_ProjetoNaoExiste_DeveLancarExcecao()
        {
            // Arrange
            _mockDal.Setup(d => d.GetById<Projeto>(10)).Returns((Projeto)null);

            // Act
            Action act = () => _service.DeleteProject(10);

            // Assert
            act.Should().Throw<ApplicationException>().WithMessage("Projeto não encontrado");
        }

        [Fact]
        public void DeleteProject_ProjetoExiste_DeveChamarDelete()
        {
            // Arrange
            var projeto = new Projeto { Id = 1, Nome = "Projeto X", Descricao = "Original", AlteradoPor = new Usuario { Nome = "x" } };
            _mockDal.Setup(d => d.GetById<Projeto>(1)).Returns(projeto);

            // Act
            _service.DeleteProject(1);

            // Assert
            _mockDal.Verify(d => d.Delete(projeto), Times.Once);
        }


        [Fact]
        public void GetAllProjectsByStatus_DeveRetornarFiltrados()
        {
            var projetos = new List<Projeto>
            {
                new() { Nome = "Projeto 1",AlteradoPor =new Usuario { Nome = "Usuario" } ,Descricao="descricao1", Status = Status.EmAndamento },
                new() { Nome = "Projeto 2",AlteradoPor =new Usuario { Nome = "Usuario" } ,Descricao="descricao2", Status = Status.Concluida },
            };

            _mockDal.Setup(d => d.GetAll<Projeto>()).Returns(projetos);

            var resultado = _service.GetAllProjectsByStatus(Status.Concluida);

            resultado.Should().BeEquivalentTo(new List<string> { "Projeto 2" });
        }
        [Fact]
        public void GetprojectTasksByStatus_ProjetoNaoExiste_DeveLancarExcecao()
        {
            // Arrange
            _mockDal.Setup(d => d.GetById<Projeto>(10, It.IsAny<Expression<Func<Projeto, object>>>()))
                    .Returns((Projeto)null);

            // Act
            Action act = () => _service.GetprojectTasksByStatus(10, Status.EmAndamento);

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

            _mockDal.Setup(d => d.GetById<Projeto>(1, It.IsAny<Expression<Func<Projeto, object>>>()))
                    .Returns(projeto);

            // Act
            var resultado = _service.GetprojectTasksByStatus(1, Status.EmAndamento);

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

            _mockDal.Setup(d => d.GetById<Projeto>(2, It.IsAny<Expression<Func<Projeto, object>>>()))
                    .Returns(projeto);

            // Act
            var resultado = _service.GetprojectTasksByStatus(2, Status.EmAndamento);

            // Assert
            resultado.Should().BeEmpty();
        }

        [Fact]
        public void GetAllTasksByProject_ProjetoNaoExiste_DeveLancarExcecao()
        {
            // Arrange
            _mockDal.Setup(d => d.GetById<Projeto>(10, It.IsAny<Expression<Func<Projeto, object>>>()))
                    .Returns((Projeto)null);

            // Act
            Action act = () => _service.GetAllTasksByProject(10);

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

            _mockDal.Setup(d => d.GetById<Projeto>(1, It.IsAny<Expression<Func<Projeto, object>>>()))
                    .Returns(projeto);

            // Act
            var resultado = _service.GetAllTasksByProject(1);

            // Assert
            resultado.Should().BeEquivalentTo(new List<string> { "Tarefa 1", "Tarefa 2" });
        }

        [Fact]
        public void GetAllTasksByProject_ProjetoSemTarefas_DeveRetornarListaVazia()
        {
            // Arrange
            var projeto = new Projeto { Id = 2, Nome = "Projeto X", Descricao = "Descricao", AlteradoPor = new Usuario { Nome = "alterado" }, Tarefas = new List<Tarefa>() };

            _mockDal.Setup(d => d.GetById<Projeto>(2, It.IsAny<Expression<Func<Projeto, object>>>()))
                    .Returns(projeto);

            // Act
            var resultado = _service.GetAllTasksByProject(2);

            // Assert
            resultado.Should().BeEmpty();
        }

        [Fact]
        public void CreateTask_ProjetoNaoExiste_DeveLancarExcecao()
        {
            _mockDal.Setup(d => d.GetById<Projeto>(1, It.IsAny<Expression<Func<Projeto, object>>>()))
                    .Returns((Projeto)null);

            var request = new TarefaRequest { Titulo = "Nova Tarefa", CriadoPor = "admin", Descricao = "Descricao 1", UsuarioResponsavel = "usuario" };

            Action act = () => _service.CreateTask(1, request);

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

            _mockDal.Setup(d => d.GetById<Projeto>(1, It.IsAny<Expression<Func<Projeto, object>>>()))
                    .Returns(projeto);

            var request = new TarefaRequest { Titulo = "Nova", CriadoPor = "admin", Descricao = "Descricao 1", UsuarioResponsavel = "usuario" };

            Action act = () => _service.CreateTask(1, request);

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

            _mockDal.Setup(d => d.GetById<Projeto>(1, It.IsAny<Expression<Func<Projeto, object>>>()))
            .Returns(projeto);

            TarefaHistorico? historicoSalvo = null;
            _mockDal.Setup(d => d.Create(It.IsAny<TarefaHistorico>()))
                    .Callback<TarefaHistorico>(h => historicoSalvo = h);

            _mockDal.Setup(d => d.GetAll<Usuario>()).Returns([new Usuario { Nome = "admin" }, new Usuario { Nome = "dev" }]);

            var request = new TarefaRequest
            {
                Titulo = "Nova Tarefa",
                Descricao = "Descrição",
                UsuarioResponsavel = "dev",
                DataDeVencimento = new DateTime(2025, 12, 31),
                Prioridade = Prioridade.Alta,
                CriadoPor = "admin"
            };

            var resultado = _service.CreateTask(1, request);

            // Assert
            resultado.Titulo.Should().Be("Nova Tarefa");
            resultado.Usuario.Nome.Should().Be("dev");
            resultado.Status.Should().Be(Status.EmAndamento);
            projeto.Tarefas.Should().Contain(resultado);

            _mockDal.Verify(d => d.Update(projeto), Times.Once);
            _mockDal.Verify(d => d.Create(It.IsAny<TarefaHistorico>()), Times.Once);

            historicoSalvo.Should().NotBeNull();
            historicoSalvo.DescricaoDaAlteracao.Should().Be("Criação Tarefa");
            historicoSalvo.AlteradoPor.Nome.Should().Be("admin");
        }

        [Fact]
        public void GetTaskByID_TarefaExiste_DeveRetornarTarefa()
        {
            // Arrange
            var tarefa = new Tarefa { Id = 1, Titulo = "Tarefa Teste", Descricao = "Descricao 1", Usuario = new Usuario { Nome = "usuario" } };

            _mockDal.Setup(d => d.GetById<Tarefa>(1)).Returns(tarefa);

            // Act
            var resultado = _service.GetTaskByID(1);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Titulo.Should().Be("Tarefa Teste");
        }

        [Fact]
        public void GetTaskByID_TarefaNaoExiste_DeveLancarExcecao()
        {
            // Arrange
            _mockDal.Setup(d => d.GetById<Tarefa>(1)).Returns((Tarefa)null);

            // Act
            Action act = () => _service.GetTaskByID(1);

            // Assert
            act.Should().Throw<ApplicationException>().WithMessage("Tarefa não encontrada");
        }

        [Fact]
        public void UpdateTask_TarefaNaoExiste_DeveLancarExcecao()
        {
            _mockDal.Setup(d => d.GetById<Tarefa>(1)).Returns((Tarefa)null);

            var request = new TarefaUpdateRequest { AlteradoPor = "admin" };

            Action act = () => _service.UpdateTask(1, request);

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

            _mockDal.Setup(d => d.GetById<Tarefa>(1)).Returns(tarefa);

            TarefaHistorico historicoCriado = null;

            _mockDal.Setup(d => d.Create(It.IsAny<TarefaHistorico>()))
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

            _mockDal.Setup(d => d.GetAll<Usuario>()).Returns([new Usuario { Nome = "admin" }]);

            // Act
            _service.UpdateTask(1, request);

            // Assert
            tarefa.Titulo.Should().Be("Novo Título");
            tarefa.Descricao.Should().Be("Nova Descrição");
            tarefa.DataDeVencimento.Should().Be(novaData);
            tarefa.Status.Should().Be(Status.Concluida);
            tarefa.Usuario.Nome.Should().Be("admin");

            _mockDal.Verify(d => d.Update(tarefa), Times.Once);
            _mockDal.Verify(d => d.Create(It.IsAny<TarefaHistorico>()), Times.Once);

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

            _mockDal.Setup(d => d.GetById<Tarefa>(2)).Returns(tarefa);

            _mockDal.Setup(d => d.GetAll<Usuario>()).Returns([new Usuario { Nome = "admin" }]);

            var request = new TarefaUpdateRequest
            {
                Titulo = null,
                Descricao = "Atualizado",
                DataDeVencimento = null,
                Status = null,
                UsuarioResponsavel = null,
                AlteradoPor = "admin"
            };

            _service.UpdateTask(2, request);

            tarefa.Titulo.Should().Be("Original");
            tarefa.Descricao.Should().Be("Atualizado");
            tarefa.Usuario.Nome.Should().Be("user");

            _mockDal.Verify(d => d.Update(tarefa), Times.Once);
            _mockDal.Verify(d => d.Create(It.IsAny<TarefaHistorico>()), Times.Once);
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

            _mockDal.Setup(d => d.GetById<Tarefa>(request.IdTarefa)).Returns((Tarefa)null);

            _mockDal.Setup(d => d.GetAll<Usuario>()).Returns(new List<Usuario> { new Usuario { Nome = "user" } });

            // Act
            Action act = () => _service.AddComment(request.IdTarefa, request);

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

            _mockDal.Setup(d => d.GetById<Tarefa>(1)).Returns(tarefa);

            _mockDal.Setup(d => d.GetAll<Usuario>()).Returns(new List<Usuario> { new Usuario { Nome = "admin" } });

            TarefaHistorico historicoCriado = null;
            _mockDal.Setup(d => d.Create(It.IsAny<TarefaHistorico>()))
                    .Callback<TarefaHistorico>(h => historicoCriado = h);

            // Act
            _service.AddComment(1, request);

            // Assert
            tarefa.Comentarios.Should().HaveCount(1);
            tarefa.Comentarios[0].conteudo.Should().Be("Primeiro comentário");
            tarefa.Comentarios[0].Usuario.Nome.Should().Be("admin");

            _mockDal.Verify(d => d.Update(tarefa), Times.Once);
            _mockDal.Verify(d => d.Create(It.IsAny<TarefaHistorico>()), Times.Once);

            historicoCriado.Should().NotBeNull();
            historicoCriado.DescricaoDaAlteracao.Should().Be("Novo Comentário na Tarefa");
            historicoCriado.AlteradoPor.Nome.Should().Be("admin");
        }
        [Fact]
        public void CloseTask_TarefaExiste_DeveChamarDelete()
        {
            // Arrange
            var tarefa = new Tarefa {Id = 1, Titulo = "Tarefa 1", Descricao = "Descricao 1", Usuario = new Usuario { Nome = "usuario" } };

            _mockDal.Setup(d => d.GetById<Tarefa>(1)).Returns(tarefa);

            // Act
            _service.CloseTask(1);

            // Assert
            _mockDal.Verify(d => d.Delete(tarefa), Times.Once);
        }

        [Fact]
        public void CloseTask_TarefaNaoExiste_DeveIgnorarSemErro()
        {
            // Arrange
            _mockDal.Setup(d => d.GetById<Tarefa>(1)).Returns((Tarefa)null);

            // Act
            var act = () => _service.CloseTask(1);

            // Assert
            act.Should().NotThrow();
            _mockDal.Verify(d => d.Delete(It.IsAny<Tarefa>()), Times.Never);
        }

    }
}