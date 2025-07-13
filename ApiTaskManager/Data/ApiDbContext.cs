using Microsoft.EntityFrameworkCore;
using ApiTaskManager.Models;

namespace ApiTaskManager.Data;

public class ApiDbContext(DbContextOptions<ApiDbContext> options) : DbContext(options)
{
    public DbSet<Projeto> Projetos => Set<Projeto>();
    public DbSet<Tarefa> Tarefas => Set<Tarefa>();
    public DbSet<TarefaHistorico> TarefasHistorico => Set<TarefaHistorico>();
}
