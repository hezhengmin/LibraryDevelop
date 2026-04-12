using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zheng.Bll.Interfaces
{
    public interface IGenericRepository<T>
    {
        Task<IEnumerable<T>> GetAllAsync();
        void Add(T item);
        void Update(T item);
        void Delete(T item);
        IQueryable<T> GetAll();
    }
}
