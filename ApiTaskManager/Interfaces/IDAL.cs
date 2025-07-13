using System.Linq.Expressions;

namespace ApiTaskManager.Interfaces
{
    public interface IDAL
    {
        T? GetById<T>(int id) where T : class;
        T? GetById<T>(int id, params Expression<Func<T, object>>[] includes) where T : class;
        void Delete<T>(T entity) where T : class;
        void Update<T>(T entity) where T : class;
        ICollection<T> GetAll<T>() where T : class;
        T Create<T>(T entity) where T : class;
    }
}
