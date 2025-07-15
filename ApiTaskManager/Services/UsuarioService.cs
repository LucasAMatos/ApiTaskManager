using ApiTaskManager.Data;
using ApiTaskManager.Interfaces.DAL;
using ApiTaskManager.Interfaces.Services;
using ApiTaskManager.Models.Entity;

namespace ApiTaskManager.Services
{
    public class UsuarioService : IUsuarioService
    {
        IUsuarioDAL _UsuarioDAL;

        public UsuarioService(IUsuarioDAL usuarioDAL)
        {
            _UsuarioDAL = usuarioDAL;
        }
        #region Usuarios
        public Usuario? GetUsuarioByName(string nome)
        {
            return _UsuarioDAL.GetAll<Usuario>().FirstOrDefault(u => u.Nome == nome) ?? throw new ApplicationException("Usuário não encontrado");
        }
        #endregion

    }
}
