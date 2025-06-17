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
    /// <summary>
    /// Use case responsible for retrieving all bookings for a specific user.
    /// Provides user-centric view of booking history and current reservations.
    /// </summary>
    public class GetUserBookingsUseCase
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetUserBookingsUseCase(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Retrieves all bookings associated with the specified email address.
        /// Maps domain entities to DTOs for API response.
        /// </summary>
        /// <param name="emailAddress">The email address of the user whose bookings to retrieve.</param>
        /// <returns>A result containing the list of user bookings or error details.</returns>
        public async Task<Result<List<BookingDto>>> ExecuteAsync(string emailAddress)
        {
            try
            {
                var bookings = await _unitOfWork.BookingRepository.GetBookingsByEmailAsync(emailAddress);

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
