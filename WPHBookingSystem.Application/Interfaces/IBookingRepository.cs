using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPHBookingSystem.Application.Interfaces.Common;
using WPHBookingSystem.Domain.Entities;

namespace WPHBookingSystem.Application.Interfaces
{
    public interface IBookingRepository : IGenericRepository<Booking>
    {
        Task<List<Booking>> GetBookingsByRoomIdAsync(Guid roomId);
        Task<List<Booking>> GetBookingsByEmailAsync(string emailAddress);
        Task<decimal> GetTotalRevenueAsync(Guid roomId, DateTime? start = null, DateTime? end = null);
        Task<int> GetOccupancyRateAsync(Guid roomId, DateTime start, DateTime end);
    }
}
