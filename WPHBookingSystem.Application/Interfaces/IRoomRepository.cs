using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPHBookingSystem.Domain.Entities;

namespace WPHBookingSystem.Application.Interfaces
{   /// <summary>
    /// Interface for Room repository operations.
    /// TO DO: Add methods for room availability checks and other room-related operations.
    /// TO DO: Implement GENERIC repository pattern for better reusability.
    public interface IRoomRepository
    {
        Task<Room?> GetByIdAsync(Guid roomId);
        Task<List<Room>> GetAllAsync();
        Task AddAsync(Room room);
        Task UpdateAsync(Room room);
        Task DeleteAsync(Guid roomId);
        Task<bool> IsRoomAvailableAsync(Guid roomId, DateTime checkIn, DateTime checkOut);
    }
}
