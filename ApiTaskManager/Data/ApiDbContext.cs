using Microsoft.EntityFrameworkCore;
using ApiTaskManager.Models;

namespace ApiTaskManager.Database;

public class ApiDbContext : DbContext
{
    public ApiDbContext(DbContextOptions<ApiDbContext> options) : base(options) { }

    public DbSet<Projeto> Projetos => Set<Projeto>();
    public DbSet<Tarefa> Tarefas => Set<Tarefa>();
}
