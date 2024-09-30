using LinkDev.IKEA.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkDev.IKEA.DAL.Persistence.Repositories._Generic
{
    public interface IGenericRepository <T> where T : ModelBase
    {
        Task<T?> GetAsync(int id);
        Task<IEnumerable<T>> GetAllAsync(bool WithAsNoTracking = true);
        IQueryable<T> GetIQueryable();
        IEnumerable<T> GetIEnumerable();
        void Add(T entity);
        void Update(T entity);
        void Delete(T entity);
    }
}
