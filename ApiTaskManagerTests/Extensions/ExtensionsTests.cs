using FluentAssertions;
using ApiTaskManager.Models;
using ApiTaskManager.Extensions;
using ApiTaskManager.Models.Request;
using ApiTaskManager.Enums;

namespace ApiTaskManagerTests.Extensions
{
    public class ExtensionsTests
    {
        [Fact]
        public void ToHistorico_DeveCriarTarefaHistoricoCorretamente()
        {
            // Arrange
            var tarefa = new Tarefa
            {
                Id = 1,
                Titulo = "Título da tarefa",
                Descricao = "Descrição da tarefa",
                DataDeVencimento = new DateTime(2025, 12, 31),
                Status = Status.Pendente,
                Usuario = "usuario1",
                Prioridade = Prioridade.Alta,
                ProjetoId = 10
            };

            var alteradoPor = "admin";
            var descricaoAlteracao = "Alterado o status da tarefa";

            // Act
            var historico = tarefa.ToHistorico(alteradoPor, descricaoAlteracao);

            // Assert
            historico.IdTarefa.Should().Be(tarefa.Id);
            historico.Titulo.Should().Be(tarefa.Titulo);
            historico.Descricao.Should().Be(tarefa.Descricao);
            historico.DataDeVencimento.Should().Be(tarefa.DataDeVencimento);
            historico.Status.Should().Be(tarefa.Status);
            historico.Usuario.Should().Be(tarefa.Usuario);
            historico.Prioridade.Should().Be(tarefa.Prioridade);
            historico.ProjetoId.Should().Be(tarefa.ProjetoId);
            historico.AlteradoPor.Should().Be(alteradoPor);
            historico.DescricaoDaAlteracao.Should().Be(descricaoAlteracao);
            historico.DataAlteracao.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(2));
        }

        [Fact]
        public void ToComentario_DeveCriarComentarioCorretamente()
        {
            // Arrange
            var comentarioRequest = new ComentarioRequest
            {
                Comentario = "Este é um comentário",
                IdTarefa = 5,
                Usuario = "comentador1"
            };

            // Act
            var comentario = comentarioRequest.ToComentario();

            // Assert
            comentario.conteudo.Should().Be(comentarioRequest.Comentario);
            comentario.IdTarefa.Should().Be(comentarioRequest.IdTarefa);
            comentario.Usuario.Should().Be(comentarioRequest.Usuario);
        }
    }
}
