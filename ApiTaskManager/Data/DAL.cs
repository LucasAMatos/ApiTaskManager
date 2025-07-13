using ApiTaskManager.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ApiTaskManager.Data
{
    public class DAL(DbContext context) : IDAL
    {
        private readonly DbContext _context = context;

        public T Create<T>(T entity) where T : class
        {
            _context.Set<T>().Add(entity);
            _context.SaveChanges();

            return entity;
        }

        public ICollection<T> GetAll<T>() where T : class
        {
            return [.. context.Set<T>()];
        }

        public void Update<T>(T entity) where T : class
        {
            _context.Set<T>().Update(entity);
            _context.SaveChanges();
        }

        public void Delete<T>(T entity) where T : class
        {
            _context.Set<T>().Remove(entity);
            _context.SaveChanges();
        }

        public T? GetById<T>(int id) where T : class
        {
            return context.Set<T>().Find(id);
        }

        public T? GetById<T>(int id, params Expression<Func<T, object>>[] includes) where T : class
        {
            IQueryable<T> query = context.Set<T>();

            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            return query.FirstOrDefault(e => EF.Property<int>(e, "Id") == id);
        }
    }
}
