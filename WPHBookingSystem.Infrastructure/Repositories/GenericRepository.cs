using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPHBookingSystem.Application.Exceptions;
using WPHBookingSystem.Application.Interfaces.Common;
using WPHBookingSystem.Infrastructure.Persistence.Data;

namespace WPHBookingSystem.Infrastructure.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly ApplicationDbContext _context;
        public GenericRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task AddAsync(T entity)
        {
            await _context.Set<T>().AddAsync(entity);
           // return Task.CompletedTask;
        }

        public async Task DeleteAsync(Guid id)
        {
            var result = await GetByIdAsync(id);
            if (result == null)
            {
                throw new NotFoundException("Entity not found, can't delete!");
            }
            _context.Set<T>().Remove(result);
        }

        public async Task<List<T>> GetAllAsync()
        {
            return await _context.Set<T>().ToListAsync();
        }

        public async Task<T?> GetByIdAsync(Guid id)
        {
            var result = await _context.Set<T>().FindAsync(id);

            if(result == null)
            {
                throw new NotFoundException("No Result");
            }
            return result;
        }

        public Task UpdateAsync(T entity)
        {
            _context.Set<T>().Update(entity);
            return Task.CompletedTask;
        }

        public async Task<T?> FindAsync(Func<T, bool> predicate)
        {
            return await Task.FromResult(_context.Set<T>().FirstOrDefault(predicate));
        }
    }
}
