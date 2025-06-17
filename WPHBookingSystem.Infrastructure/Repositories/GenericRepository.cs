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
    /// <summary>
    /// Generic repository implementation that provides common CRUD operations for any entity type.
    /// Implements the repository pattern to abstract data access logic and provide a consistent
    /// interface for database operations.
    /// 
    /// This repository uses Entity Framework Core for data access and provides async operations
    /// for better performance and scalability.
    /// </summary>
    /// <typeparam name="T">The entity type that this repository handles.</typeparam>
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly ApplicationDbContext _context;
        
        /// <summary>
        /// Initializes a new instance of the GenericRepository with the specified DbContext.
        /// </summary>
        /// <param name="context">The Entity Framework DbContext for data access.</param>
        public GenericRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        
        /// <summary>
        /// Adds a new entity to the database asynchronously.
        /// </summary>
        /// <param name="entity">The entity to add to the database.</param>
        public async Task AddAsync(T entity)
        {
            await _context.Set<T>().AddAsync(entity);
           // return Task.CompletedTask;
        }

        /// <summary>
        /// Deletes an entity from the database by its ID.
        /// Throws NotFoundException if the entity doesn't exist.
        /// </summary>
        /// <param name="id">The unique identifier of the entity to delete.</param>
        /// <exception cref="NotFoundException">Thrown when the entity is not found.</exception>
        public async Task DeleteAsync(Guid id)
        {
            var result = await GetByIdAsync(id);
            if (result == null)
            {
                throw new NotFoundException("Entity not found, can't delete!");
            }
            _context.Set<T>().Remove(result);
        }

        /// <summary>
        /// Retrieves all entities of type T from the database.
        /// </summary>
        /// <returns>A list of all entities.</returns>
        public async Task<List<T>> GetAllAsync()
        {
            return await _context.Set<T>().ToListAsync();
        }

        /// <summary>
        /// Retrieves an entity by its unique identifier.
        /// Throws NotFoundException if the entity doesn't exist.
        /// </summary>
        /// <param name="id">The unique identifier of the entity to retrieve.</param>
        /// <returns>The entity if found, null otherwise.</returns>
        /// <exception cref="NotFoundException">Thrown when the entity is not found.</exception>
        public async Task<T?> GetByIdAsync(Guid id)
        {
            var result = await _context.Set<T>().FindAsync(id);

            if(result == null)
            {
                throw new NotFoundException("No Result");
            }
            return result;
        }

        /// <summary>
        /// Updates an existing entity in the database.
        /// </summary>
        /// <param name="entity">The entity to update.</param>
        public Task UpdateAsync(T entity)
        {
            _context.Set<T>().Update(entity);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Finds an entity using a predicate function.
        /// </summary>
        /// <param name="predicate">The predicate function to filter entities.</param>
        /// <returns>The first entity that matches the predicate, or null if not found.</returns>
        public async Task<T?> FindAsync(Func<T, bool> predicate)
        {
            return await Task.FromResult(_context.Set<T>().FirstOrDefault(predicate));
        }
    }
}
