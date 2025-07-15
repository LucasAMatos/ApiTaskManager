using ApiTaskManager.Models;
using ApiTaskManager.Models.Entity;
using Microsoft.EntityFrameworkCore;

namespace ApiTaskManager.Data
{
    public class ProjetoDbContext : DbContext
    {
        public ProjetoDbContext(DbContextOptions<ProjetoDbContext> options) : base(options) { }

        public DbSet<Projeto> Projetos { get; set; }
        public DbSet<Tarefa> Tarefas { get; set; }
        public DbSet<Comentario> Comentarios { get; set; }
    }
}
