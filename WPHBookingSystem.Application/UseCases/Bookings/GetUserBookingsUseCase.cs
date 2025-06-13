using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPHBookingSystem.Application.DTOs.Booking;
using WPHBookingSystem.Application.Interfaces;

namespace WPHBookingSystem.Application.UseCases.Bookings
{
    public class GetUserBookingsUseCase
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetUserBookingsUseCase(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<BookingDto>> ExecuteAsync(Guid userId)
        {
            var bookings = await _unitOfWork.Bookings.GetBookingsByUserIdAsync(userId);

            return bookings.Select(b => new BookingDto
            {
                Id = b.Id,
                RoomId = b.RoomId,
                CheckIn = b.CheckIn,
                CheckOut = b.CheckOut,
                Guests = b.Guests,
                TotalAmount = b.TotalAmount,
                Status = b.Status.ToString(),
                SpecialRequests = b.SpecialRequests,
                RoomName = b.Room?.Name ?? string.Empty,
            }).ToList();
        }
    }
}
