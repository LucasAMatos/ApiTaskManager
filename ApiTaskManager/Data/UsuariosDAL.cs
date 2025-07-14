using ApiTaskManager.Interfaces.DAL;
using ApiTaskManager.Models.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace ApiTaskManager.Data
{
    public class UsuariosDAL : BaseRepository, IUsuarioDAL
    {
        private readonly UsuarioDbContext _db;
        public UsuariosDAL(UsuarioDbContext db) : base(db)
        {
            _db = db;
        }

        public DbSet<Usuario> Usuarios => _db.Usuarios;
    }
}
