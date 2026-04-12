using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zheng.Bll.Interfaces;
using Zheng.Infra.Data.Models;
using Zheng.Infra.Data.Models;

namespace Zheng.Bll
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly LibraryContext _context;

        public GenericRepository(LibraryContext context)
        {
            _context = context;
            Console.WriteLine($"GenericRepository {_context.ContextId.GetHashCode()}");
        }

        public void Add(T item)
        {
            _context.Set<T>().AddAsync(item);
        }

        public void Delete(T item)
        {
             _context.Set<T>().Remove(item);
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
           return await _context.Set<T>().ToListAsync();
        }

        public void Update(T item)
        {
            _context.Entry(item).State = EntityState.Modified;
            _context.Set<T>().Update(item);
        }

        public IQueryable<T> GetAll()
        {
            return _context.Set<T>().AsQueryable();  
        }
    }
}
