using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPHBookingSystem.Domain.Entities;
using WPHBookingSystem.Domain.Enums;

namespace WPHBookingSystem.Domain.Repositories
{
    public interface IRoomRepository
    {
        Task<Room> GetByIdAsync(Guid id);
        Task<IEnumerable<Room>> ListAsync(RoomStatus? status = null);
        Task AddAsync(Room room);
        Task UpdateAsync(Room room);
        Task DeleteAsync(Room room);
    }
}
