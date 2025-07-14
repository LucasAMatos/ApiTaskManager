using ApiTaskManager.Models.Entity;
using Microsoft.EntityFrameworkCore;

namespace ApiTaskManager.Data
{
    public class UsuarioDbContext : DbContext
    {
        public UsuarioDbContext(DbContextOptions<UsuarioDbContext> options) : base(options) { }

        public DbSet<Usuario> Usuarios { get; set; }
    }
}
