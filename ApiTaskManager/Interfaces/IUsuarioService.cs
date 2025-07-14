using ApiTaskManager.Models;

namespace ApiTaskManager.Interfaces
{
    public interface IUsuarioService
    {
        Usuario? GetUsuarioByName(string nome);
    }
}
