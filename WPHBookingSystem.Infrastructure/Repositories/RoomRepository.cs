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
    public class RoomRepository(ApplicationDbContext context) : GenericRepository<Room>(context), IRoomRepository
    {
        private readonly ApplicationDbContext _context = context;

        public Task<List<Room>> GetAvailableRoomsAsync(DateTime checkIn, DateTime checkOut)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsRoomAvailableAsync(Guid roomId, DateTime checkIn, DateTime checkOut)
        {
            throw new NotImplementedException();
        }
    }
}
