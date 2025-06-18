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
    /// Use case responsible for retrieving all bookings in the system.
    /// Provides administrative view of all booking data for management purposes.
    /// </summary>
    public class GetAllBookingsUseCase
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetAllBookingsUseCase(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Retrieves all bookings in the system.
        /// Maps domain entities to DTOs for API response.
        /// </summary>
        /// <returns>A result containing the list of all bookings or error details.</returns>
        public async Task<Result<List<BookingDto>>> ExecuteAsync()
        {
            try
            {
                var bookings = await _unitOfWork.BookingRepository.GetAllBookingsWithRoomAsync();

                var bookingDtos = bookings.Select(b => new BookingDto
                {
                    Id = b.Id,
                    RoomId = b.RoomId,
                    GuestName = b.GuestName,
                    EmailAddress = b.EmailAddress,
                    Phone = b.ContactInfo?.Phone ?? string.Empty,
                    Address = b.ContactInfo?.Address ?? string.Empty,
                    CheckIn = b.CheckIn,
                    CheckOut = b.CheckOut,
                    Guests = b.Guests,
                    TotalAmount = b.TotalAmount,
                    Status = b.Status,
                    SpecialRequests = b.SpecialRequests,
                    RoomName = b.Room?.Name ?? string.Empty,
                    CreatedAt = b.CreatedAt.DateTime,
                    UpdatedAt = b.UpdatedAt?.DateTime ?? default(DateTime)
                }).ToList();

                return Result<List<BookingDto>>.Success(bookingDtos, "All bookings retrieved successfully.");
            }
            catch (Exception ex)
            {
                return Result<List<BookingDto>>.Failure($"Failed to retrieve all bookings: {ex.Message}", 500);
            }
        }
    }
} 