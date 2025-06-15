using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public Task<List<Room>> GetAvailableRoomsAsync(DateTime checkIn, DateTime checkOut)
        {
            throw new NotImplementedException();
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

        public Task<bool> IsRoomAvailableAsync(Guid roomId, DateTime checkIn, DateTime checkOut)
        {
            throw new NotImplementedException();
        }
    }
}
