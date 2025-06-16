using System;
using System.Threading.Tasks;
using WPHBookingSystem.Application.Common;
using WPHBookingSystem.Application.DTOs.Booking;
using WPHBookingSystem.Application.Interfaces;
using WPHBookingSystem.Domain.Entities;
using WPHBookingSystem.Domain.Exceptions;

namespace WPHBookingSystem.Application.UseCases.Bookings
{
    public class CancelBookingUseCase
    {
        private readonly IUnitOfWork _unitOfWork;

        public CancelBookingUseCase(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<BookingDto>> ExecuteAsync(Guid bookingId)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                var booking = await _unitOfWork.Repository<Booking>().GetByIdAsync(bookingId);
                if (booking == null)
                    return Result<BookingDto>.Failure("Booking not found.", 404);

                //if (booking.EmailAddress != emailAddress)
                //    return Result<BookingDto>.Failure("You are not authorized to cancel this booking.", 403);

                booking.Cancel();
                await _unitOfWork.Repository<Booking>().UpdateAsync(booking);
                await _unitOfWork.CommitTransactionAsync();

                return Result<BookingDto>.Success(new BookingDto
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
                }, "Booking cancelled successfully.");
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return Result<BookingDto>.Failure($"Failed to cancel booking: {ex.Message}", 500);
            }
        }
    }
}
