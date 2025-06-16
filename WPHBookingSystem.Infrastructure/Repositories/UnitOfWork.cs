using WPHBookingSystem.Application.Interfaces;
using WPHBookingSystem.Infrastructure.Persistence.Data;

namespace WPHBookingSystem.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        // Backing fields
        private IRoomRepository? _rooms;
        private IBookingRepository? _bookings;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        public IRoomRepository Rooms => _rooms ??= new RoomRepository(_context);
        public IBookingRepository Bookings => _bookings ??= new BookingRepository(_context);

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}