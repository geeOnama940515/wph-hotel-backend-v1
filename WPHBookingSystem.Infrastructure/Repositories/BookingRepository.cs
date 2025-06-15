using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPHBookingSystem.Application.Interfaces;
using WPHBookingSystem.Domain.Entities;
using WPHBookingSystem.Infrastructure.Persistence.Data;

namespace WPHBookingSystem.Infrastructure.Repositories
{
    public class BookingRepository(ApplicationDbContext context) : GenericRepository<Booking>(context), IBookingRepository
    {
        private readonly ApplicationDbContext _context = context;  
        public Task<List<Booking>> GetBookingsByRoomIdAsync(Guid roomId)
        {
            throw new NotImplementedException();
        }

        public Task<List<Booking>> GetBookingsByUserIdAsync(Guid userId)
        {
            throw new NotImplementedException();
        }

        public Task<int> GetOccupancyRateAsync(Guid roomId, DateTime start, DateTime end)
        {
            throw new NotImplementedException();
        }

        public Task<decimal> GetTotalRevenueAsync(Guid roomId, DateTime? start = null, DateTime? end = null)
        {
            throw new NotImplementedException();
        }
    }
}
