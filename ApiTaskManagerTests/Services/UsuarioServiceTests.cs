using ApiTaskManager.Interfaces.DAL;
using ApiTaskManager.Models.Entity;
using ApiTaskManager.Services;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace ApiTaskManager.Tests.Services
{
    public class UsuarioServiceTests
    {
        private readonly Mock<IUsuarioDAL> _usuarioDalMock;
        private readonly UsuarioService _usuarioService;

        public UsuarioServiceTests()
        {
            _usuarioDalMock = new Mock<IUsuarioDAL>();
            _usuarioService = new UsuarioService(_usuarioDalMock.Object);
        }

        [Fact]
        public void Should_Return_User_When_Exists()
        {
            // Arrange
            var usuarios = new List<Usuario>
            {
                new Usuario { Id = 1, Nome = "gerente", Cargo = "Gerente" },
                new Usuario { Id = 2, Nome = "dev", Cargo = "Dev" }
            };

            _usuarioDalMock.Setup(dal => dal.GetAll<Usuario>())
                           .Returns(usuarios);

            // Act
            var result = _usuarioService.GetUsuarioByName("dev");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("dev", result.Nome);
        }

        [Fact]
        public void Should_Throw_Exception_When_User_Not_Found()
        {
            // Arrange
            var usuarios = new List<Usuario>
            {
                new Usuario { Id = 1, Nome = "gerente", Cargo = "Gerente" }
            };

            _usuarioDalMock.Setup(dal => dal.GetAll<Usuario>())
                           .Returns(usuarios);

            // Act & Assert
            var ex = Assert.Throws<ApplicationException>(() => _usuarioService.GetUsuarioByName("Usuário não encontrado"));
            Assert.Equal("Usuário não encontrado", ex.Message);
        }
    }
}
