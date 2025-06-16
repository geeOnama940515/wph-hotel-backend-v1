using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPHBookingSystem.Application.DTOs.Booking;

namespace WPHBookingSystem.Application.Interfaces.Services
{
    public class BookingService : IBookingService
    {
        /// <summary>
        /// Thin facade service that delegates to booking use cases.
        /// Keeps controllers clean while maintaining separation of concerns.
        /// </summary>
        public Task CancelBookingAsync(Guid bookingId, string emailAddress)
        {
            throw new NotImplementedException();
        }

        public Task<Guid> CreateBookingAsync(CreateBookingDto dto, Guid userId)
        {
            throw new NotImplementedException();
        }

        public Task<List<BookingDto>> GetRoomByUserEmailAsync(string emailAddress)
        {
            throw new NotImplementedException();
        }
    }
}
