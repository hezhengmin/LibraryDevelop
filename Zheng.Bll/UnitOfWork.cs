using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using Zheng.Bll.Interfaces;
using Zheng.Infra.Data.Models;

namespace Zheng.Bll
{
    /*
    Scoped
    https://learn.microsoft.com/zh-tw/dotnet/architecture/microservices/microservice-ddd-cqrs-patterns/infrastructure-persistence-layer-design
    https://learn.microsoft.com/zh-tw/aspnet/mvc/overview/older-versions/getting-started-with-ef-5-using-mvc-4/implementing-the-repository-and-unit-of-work-patterns-in-an-asp-net-mvc-application
    https://github.com/arch/UnitOfWork
    https://www.linkedin.com/pulse/implementing-unit-work-repository-pattern-net-core-bany-elmarjeh-es5cf/
    
    Singleton
    https://github.com/xiaohuidong99/WordReminder
    */
    public class UnitOfWork : IUnitOfWork
    {
        private readonly LibraryContext _context;
        private bool disposed = false;

        public UnitOfWork(LibraryContext context)
        {
            _context = context;
            Console.WriteLine($"UniofOfWork {_context.ContextId.GetHashCode()}");
        }

        public int SaveChanges()
        {
            return _context.SaveChanges();
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
            this.disposed = true;
        }
    }
}
