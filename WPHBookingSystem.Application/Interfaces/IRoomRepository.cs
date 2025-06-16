using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPHBookingSystem.Application.Interfaces.Common;
using WPHBookingSystem.Domain.Entities;

namespace WPHBookingSystem.Application.Interfaces
{   /// <summary>
    /// Interface for Room repository operations.
    /// TO DO: Add methods for room availability checks and other room-related operations.
    /// TO DO: Implement GENERIC repository pattern for better reusability.
    /// </summary>
    public interface IRoomRepository : IGenericRepository<Room>
    {
        Task<List<Room>> GetAvailableRoomsAsync(DateTime checkIn, DateTime checkOut);
        Task<bool> IsRoomAvailableAsync(Guid roomId, DateTime checkIn, DateTime checkOut);
        Task<Room?> GetByIdWithBookingsAsync(Guid id);
    }
}
