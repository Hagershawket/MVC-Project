using LinkDev.IKEA.DAL.Entities;
using LinkDev.IKEA.DAL.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkDev.IKEA.DAL.Persistence.Repositories._Generic
{
    public class GenericRepository<T> : IGenericRepository<T> where T : ModelBase
    {
        private readonly ApplicationDbContext _dbContext;
        public GenericRepository(ApplicationDbContext dbContext) // ASK CLR for Object from ApplicationDbContext Implicitly
        {
            _dbContext = dbContext;
        }
        public async Task<IEnumerable<T>> GetAllAsync(bool WithAsNoTracking = true)
        {
            if (WithAsNoTracking)
                return await _dbContext.Set<T>().Where(X => !X.IsDeleted).AsNoTracking().ToListAsync();
            return await _dbContext.Set<T>().Where(X => !X.IsDeleted).ToListAsync();
        }

        public IQueryable<T> GetIQueryable()
        {
            return _dbContext.Set<T>();
        }

        public IEnumerable<T> GetIEnumerable()
        {
            return _dbContext.Set<T>();
        }

        public async Task<T?> GetAsync(int id)
        {
            return await _dbContext.Set<T>().FindAsync(id);
            // return _dbContext.Find<T>(id);

            /// var T = _dbContext.Ts.Local.FirstOrDefault(D => D.Id == id);
            /// if (T is null)
            /// 	return _dbContext.Ts.FirstOrDefault(D => D.Id == id);
            /// return T;

        }

        public void Add(T entity) => _dbContext.Set<T>().Add(entity);

        public void Update(T entity) => _dbContext.Set<T>().Update(entity);

        public void Delete(T entity)
        {
            entity.IsDeleted = true;
            _dbContext.Set<T>().Update(entity);
        }
        
    }
}
