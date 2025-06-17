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
    /// <summary>
    /// Specialized repository for room-related data access operations.
    /// Extends GenericRepository to provide room-specific query methods and availability logic.
    /// 
    /// This repository handles complex room queries including availability checks,
    /// booking relationships, and room status management.
    /// </summary>
    public class RoomRepository(ApplicationDbContext context) : GenericRepository<Room>(context), IRoomRepository
    {
        private readonly ApplicationDbContext _context = context;

        /// <summary>
        /// Retrieves all available rooms for the specified date range.
        /// Filters rooms based on availability and excludes those with overlapping bookings.
        /// </summary>
        /// <param name="checkIn">The check-in date for availability check.</param>
        /// <param name="checkOut">The check-out date for availability check.</param>
        /// <returns>A list of available rooms ordered by price.</returns>
        /// <exception cref="ArgumentException">Thrown when check-in date is not before check-out date.</exception>
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

        /// <summary>
        /// Retrieves a room by ID including all its associated bookings.
        /// Provides complete room information with booking history.
        /// </summary>
        /// <param name="id">The unique identifier of the room.</param>
        /// <returns>The room with all its bookings, or null if not found.</returns>
        /// <exception cref="NotFoundException">Thrown when the room is not found.</exception>
        public async Task<Room?> GetByIdWithBookingsAsync(Guid id)
        {
            var room = await _context.Rooms
                .Include(r => r.Bookings)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (room is null)
                throw new NotFoundException("Room not found.");

            return room;
        }

        /// <summary>
        /// Checks if a specific room is available for the given date range.
        /// Validates room existence and checks for overlapping bookings.
        /// </summary>
        /// <param name="roomId">The unique identifier of the room to check.</param>
        /// <param name="checkIn">The check-in date for availability check.</param>
        /// <param name="checkOut">The check-out date for availability check.</param>
        /// <returns>True if the room is available, false otherwise.</returns>
        /// <exception cref="ArgumentException">Thrown when check-in date is not before check-out date.</exception>
        /// <exception cref="NotFoundException">Thrown when the room is not found.</exception>
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
