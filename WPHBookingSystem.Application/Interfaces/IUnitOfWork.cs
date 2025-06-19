using System;
using System.Threading.Tasks;
using WPHBookingSystem.Application.Interfaces.Common;

namespace WPHBookingSystem.Application.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<T> Repository<T>() where T : class;
        IBookingRepository BookingRepository { get; }
        IRoomRepository RoomRepository { get; }
        IContactMessageRepository ContactMessageRepository { get; }
        Task<int> SaveChangesAsync();
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}
