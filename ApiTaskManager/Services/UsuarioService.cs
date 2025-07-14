using ApiTaskManager.Interfaces;
using ApiTaskManager.Models;

namespace ApiTaskManager.Services
{
    public class UsuarioService(IDAL _DAL) : IUsuarioService
    {

        #region Usuarios
        public Usuario? GetUsuarioByName(string nome)
        {
            return _DAL.GetAll<Usuario>().First(u => u.Nome == nome) ?? throw new ApplicationException("Usuário não encontrado");
        }
        #endregion

    }
}
