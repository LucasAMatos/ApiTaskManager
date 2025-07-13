using Microsoft.EntityFrameworkCore;

namespace ApiTaskManager.Data
{
    public class BaseDAL
    {
        private readonly ApiDbContext _context;

        public BaseDAL(ApiDbContext context)
        {
            _context = context;
        }

        public void Delete<T>(T entity) where T : class
        {
            _context.Set<T>().Remove(entity);
            _context.SaveChanges();
        }
    }
}
