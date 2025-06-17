using System;
using System.Threading.Tasks;
using WPHBookingSystem.Application.Common;
using WPHBookingSystem.Application.DTOs.Booking;
using WPHBookingSystem.Application.Interfaces;
using WPHBookingSystem.Domain.Entities;

namespace WPHBookingSystem.Application.UseCases.Bookings
{
    /// <summary>
    /// Use case responsible for retrieving booking information using a booking token.
    /// Allows guests to view booking details without authentication.
    /// </summary>
    public class ViewBookingByTokenUseCase
    {
        private readonly IUnitOfWork _unitOfWork;

        public ViewBookingByTokenUseCase(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        /// <summary>
        /// Retrieves booking information using the provided booking token.
        /// Maps domain entity to DTO for API response.
        /// </summary>
        /// <param name="bookingToken">The unique booking token for the reservation.</param>
        /// <returns>A result containing the booking information or error details.</returns>
        public async Task<Result<BookingDto>> ExecuteAsync(Guid bookingToken)
        {
            try
            {
                var booking = await _unitOfWork.Repository<Booking>().FindAsync(b => b.BookingToken == bookingToken);
                if (booking == null)
                    return Result<BookingDto>.Failure("Booking not found.", 404);

                var bookingDto = new BookingDto
                {
                    Id = booking.Id,
                    RoomId = booking.RoomId,
                    CheckIn = booking.CheckIn,
                    CheckOut = booking.CheckOut,
                    Guests = booking.Guests,
                    TotalAmount = booking.TotalAmount,
                    Status = booking.Status,
                    SpecialRequests = booking.SpecialRequests,
                    RoomName = booking.Room?.Name ?? string.Empty
                };

                return Result<BookingDto>.Success(bookingDto, "Booking retrieved successfully.");
            }
            catch (Exception ex)
            {
                return Result<BookingDto>.Failure($"Failed to retrieve booking: {ex.Message}", 500);
            }
        }
    }
} 