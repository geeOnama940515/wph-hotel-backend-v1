using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WPHBookingSystem.Application.Interfaces;
using WPHBookingSystem.Application.Interfaces.Common;
using WPHBookingSystem.Infrastructure.Persistence.Data;

namespace WPHBookingSystem.Infrastructure.Repositories
{
    /// <summary>
    /// Unit of Work implementation that manages transactions and coordinates multiple repositories.
    /// Implements the Unit of Work pattern to ensure data consistency across multiple operations
    /// and provides a centralized point for transaction management.
    /// 
    /// This class coordinates the work of multiple repositories and ensures that all changes
    /// are committed or rolled back together, maintaining ACID properties.
    /// </summary>
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private readonly Dictionary<Type, object> _repositories;
        private IDbContextTransaction? _transaction;
        private IBookingRepository? _bookingRepository;
        private IRoomRepository? _roomRepository;
        private IContactMessageRepository? _contactMessageRepository;

        /// <summary>
        /// Initializes a new instance of the UnitOfWork with the specified DbContext.
        /// </summary>
        /// <param name="context">The Entity Framework DbContext for data access.</param>
        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            _repositories = new Dictionary<Type, object>();
        }

        /// <summary>
        /// Gets a generic repository for the specified entity type.
        /// Uses lazy loading to create repositories only when needed.
        /// </summary>
        /// <typeparam name="T">The entity type for the repository.</typeparam>
        /// <returns>A generic repository instance for the specified entity type.</returns>
        public IGenericRepository<T> Repository<T>() where T : class
        {
            if (_repositories.ContainsKey(typeof(T)))
                return (IGenericRepository<T>)_repositories[typeof(T)];

            var repository = new GenericRepository<T>(_context);
            _repositories.Add(typeof(T), repository);
            return repository;
        }

        /// <summary>
        /// Gets the specialized booking repository for booking-specific operations.
        /// </summary>
        public IBookingRepository BookingRepository
        {
            get
            {
                _bookingRepository ??= new BookingRepository(_context);
                return _bookingRepository;
            }
        }


        public IContactMessageRepository ContactMessageRepository
        {
            get
            {
                _contactMessageRepository ??= new ContactMessageRepository(_context);
                return _contactMessageRepository;
            }
        }

        /// <summary>
        /// Gets the specialized room repository for room-specific operations.
        /// </summary>
        public IRoomRepository RoomRepository
        {
            get
            {
                _roomRepository ??= new RoomRepository(_context);
                return _roomRepository;
            }
        }

        /// <summary>
        /// Saves all changes made in the current context to the database.
        /// </summary>
        /// <returns>The number of state entries written to the database.</returns>
        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Begins a new database transaction.
        /// All subsequent operations will be part of this transaction until committed or rolled back.
        /// </summary>
        public async Task BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        /// <summary>
        /// Commits the current transaction and saves all changes to the database.
        /// If any operation fails, the entire transaction is rolled back.
        /// </summary>
        public async Task CommitTransactionAsync()
        {
            try
            {
                await _context.SaveChangesAsync();
                if (_transaction != null)
                {
                    await _transaction.CommitAsync();
                }
            }
            catch
            {
                await RollbackTransactionAsync();
                throw;
            }
            finally
            {
                if (_transaction != null)
                {
                    _transaction.Dispose();
                    _transaction = null;
                }
            }
        }

        /// <summary>
        /// Rolls back the current transaction, discarding all changes made within the transaction.
        /// </summary>
        public async Task RollbackTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                _transaction.Dispose();
                _transaction = null;
            }
        }

        /// <summary>
        /// Disposes of the UnitOfWork and its associated resources.
        /// </summary>
        public void Dispose()
        {
            _context.Dispose();
            _transaction?.Dispose();
        }
    }
}