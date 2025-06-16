using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WPHBookingSystem.Application.Exceptions;
using WPHBookingSystem.Application.Interfaces;
using WPHBookingSystem.Domain.Entities;
using WPHBookingSystem.Infrastructure.Persistence.Data;

namespace WPHBookingSystem.Infrastructure.Repositories
{
    public class RoomRepository(ApplicationDbContext context) : GenericRepository<Room>(context), IRoomRepository
    {
        private readonly ApplicationDbContext _context = context;

        public async Task<List<Room>> GetAvailableRoomsAsync(DateTime checkIn, DateTime checkOut)
        {
            if (checkIn >= checkOut)
                throw new ArgumentException("Check-in date must be before check-out date.");

            // Get all rooms that don't have any overlapping bookings
            var availableRooms = await _context.Rooms
                .Include(r => r.Bookings)
                .Where(r => r.IsAvailable(checkIn,checkOut))
                .Where(r => !r.Bookings.Any(b =>
                    (b.CheckIn <= checkOut && b.CheckOut >= checkIn) && // Overlapping booking
                    b.Status != Domain.Enums.BookingStatus.Cancelled)) // Not cancelled
                .OrderBy(r => r.Price)
                .ToListAsync();

            return availableRooms;
        }

        public async Task<Room?> GetByIdWithBookingsAsync(Guid id)
        {
            var room = await _context.Rooms
                .Include(r => r.Bookings)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (room is null)
                throw new NotFoundException("Room not found.");

            return room;
        }

        public async Task<bool> IsRoomAvailableAsync(Guid roomId, DateTime checkIn, DateTime checkOut)
        {
            if (checkIn >= checkOut)
                throw new ArgumentException("Check-in date must be before check-out date.");

            var room = await _context.Rooms
                .Include(r => r.Bookings)
                .FirstOrDefaultAsync(r => r.Id == roomId);

            if (room is null)
                throw new NotFoundException("Room not found.");

            if (!room.IsAvailable(checkIn, checkOut))
                return false;

            // Check if there are any overlapping bookings that aren't cancelled
            var hasOverlappingBooking = room.Bookings.Any(b =>
                (b.CheckIn <= checkOut && b.CheckOut >= checkIn) && // Overlapping booking
                b.Status != Domain.Enums.BookingStatus.Cancelled); // Not cancelled

            return !hasOverlappingBooking;
        }
    }
}
