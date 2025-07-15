using ApiTaskManager.Interfaces.DAL;
using ApiTaskManager.Models.Entity;
using Microsoft.EntityFrameworkCore;

namespace ApiTaskManager.Data;

public class ProjetosDAL : BaseRepository, IProjetoDAL
{
    private readonly ProjetoDbContext _db;
    public ProjetosDAL(ProjetoDbContext db) : base(db) 
    {
        _db = db;
    }

    public DbSet<Projeto> Projetos => _db.Projetos;
    public DbSet<Tarefa> Tarefas => _db.Tarefas;
    public DbSet<Comentario> Comentarios => _db.Comentarios;
}
