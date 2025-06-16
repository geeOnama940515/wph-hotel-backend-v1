using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPHBookingSystem.Application.DTOs.Booking;

namespace WPHBookingSystem.Application.Interfaces.Services
{
    public interface IBookingService
    {
        Task CancelBookingAsync(Guid bookingId, string emailAddress);
        Task<Guid> CreateBookingAsync(CreateBookingDto dto, Guid userId);
        Task<List<BookingDto>> GetRoomByUserEmailAsync(string emailAddress);
    }
}
