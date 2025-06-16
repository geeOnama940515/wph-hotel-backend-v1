using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WPHBookingSystem.Application.Interfaces;
using WPHBookingSystem.Domain.Entities;
using WPHBookingSystem.Infrastructure.Persistence.Data;

namespace WPHBookingSystem.Infrastructure.Repositories
{
    public class BookingRepository(ApplicationDbContext context) : GenericRepository<Booking>(context), IBookingRepository
    {
        private readonly ApplicationDbContext _context = context;

        public async Task<List<Booking>> GetBookingsByRoomIdAsync(Guid roomId)
        {
            return await _context.Bookings
                .Include(b => b.Room)
                .Where(b => b.RoomId == roomId)
                .OrderByDescending(b => b.CheckIn)
                .ToListAsync();
        }

        public async Task<List<Booking>> GetBookingsByEmailAsync(string emailAddress)
        {
            return await _context.Bookings
                .Include(b => b.Room)
                .Where(b => b.EmailAddress.ToLower() == emailAddress.ToLower())
                .OrderByDescending(b => b.CheckIn)
                .ToListAsync();
        }

        public async Task<int> GetOccupancyRateAsync(Guid roomId, DateTime start, DateTime end)
        {
            var totalDays = (end - start).TotalDays;
            if (totalDays <= 0)
                return 0;

            var bookings = await _context.Bookings
                .Where(b => b.RoomId == roomId &&
                           ((b.CheckIn >= start && b.CheckIn < end) ||
                            (b.CheckOut > start && b.CheckOut <= end) ||
                            (b.CheckIn <= start && b.CheckOut >= end)))
                .ToListAsync();

            var occupiedDays = bookings.Sum(b =>
            {
                var bookingStart = b.CheckIn > start ? b.CheckIn : start;
                var bookingEnd = b.CheckOut < end ? b.CheckOut : end;
                return (bookingEnd - bookingStart).TotalDays;
            });

            return (int)((occupiedDays / totalDays) * 100);
        }

        public async Task<decimal> GetTotalRevenueAsync(Guid roomId, DateTime? start = null, DateTime? end = null)
        {
            var query = _context.Bookings.Where(b => b.RoomId == roomId);

            if (start.HasValue)
                query = query.Where(b => b.CheckIn >= start.Value);

            if (end.HasValue)
                query = query.Where(b => b.CheckOut <= end.Value);

            return await query.SumAsync(b => b.TotalPrice);
        }
    }
}
