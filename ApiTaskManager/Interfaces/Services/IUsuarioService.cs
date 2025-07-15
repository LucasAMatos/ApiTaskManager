using ApiTaskManager.Models.Entity;

namespace ApiTaskManager.Interfaces.Services
{
    public interface IUsuarioService
    {
        Usuario? GetUsuarioByName(string nome);
    }
}
