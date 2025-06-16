using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WPHBookingSystem.Application.Common;
using WPHBookingSystem.Application.DTOs.Booking;
using WPHBookingSystem.Application.Interfaces;
using WPHBookingSystem.Domain.Entities;

namespace WPHBookingSystem.Application.UseCases.Bookings
{
    public class GetUserBookingsUseCase
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetUserBookingsUseCase(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<List<BookingDto>>> ExecuteAsync(string emailAddress)
        {
            try
            {
                var bookings = await _unitOfWork.Repository<Booking>()
                    .FindAsync(b => b.EmailAddress == emailAddress);

                var bookingDtos = bookings.Select(b => new BookingDto
                {
                    Id = b.Id,
                    RoomId = b.RoomId,
                    CheckIn = b.CheckIn,
                    CheckOut = b.CheckOut,
                    Guests = b.Guests,
                    TotalAmount = b.TotalAmount,
                    Status = b.Status,
                    SpecialRequests = b.SpecialRequests,
                    RoomName = b.Room?.Name ?? string.Empty,
                }).ToList();

                return Result<List<BookingDto>>.Success(bookingDtos, "User bookings retrieved successfully.");
            }
            catch (Exception ex)
            {
                return Result<List<BookingDto>>.Failure($"Failed to retrieve user bookings: {ex.Message}", 500);
            }
        }
    }
}
