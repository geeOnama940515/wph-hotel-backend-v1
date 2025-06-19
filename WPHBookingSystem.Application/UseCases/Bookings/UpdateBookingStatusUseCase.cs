using System;
using System.Threading.Tasks;
using WPHBookingSystem.Application.Common;
using WPHBookingSystem.Application.DTOs.Booking;
using WPHBookingSystem.Application.Interfaces;
using WPHBookingSystem.Domain.Entities;
using WPHBookingSystem.Domain.Enums;
using WPHBookingSystem.Domain.Exceptions;

namespace WPHBookingSystem.Application.UseCases.Bookings
{
    /// <summary>
    /// Use case responsible for updating booking status transitions.
    /// Handles status change business logic and enforces valid state transitions.
    /// </summary>
    public class UpdateBookingStatusUseCase
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateBookingStatusUseCase(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Updates the status of a booking based on the requested status transition.
        /// Validates booking existence and enforces domain business rules for status changes.
        /// </summary>
        /// <param name="request">The request containing booking ID and new status.</param>
        /// <returns>A result containing the updated booking information or error details.</returns>
        public async Task<Result<BookingDto>> ExecuteAsync(UpdateBookingStatusRequest request)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                var booking = await _unitOfWork.Repository<Booking>().GetByIdAsync(request.BookingId);
                if (booking == null)
                    return Result<BookingDto>.Failure("Booking not found.", 404);

                switch (request.NewStatus)
                {
                    case BookingStatus.Confirmed:
                        booking.Confirm();
                        break;
                    case BookingStatus.Cancelled:
                        booking.Cancel();
                        break;
                    case BookingStatus.Completed:
                        booking.Complete();
                        break;
                    case BookingStatus.CheckedIn:
                        booking.CheckedIn();
                        break;
                    case BookingStatus.CheckedOut:
                        booking.CheckedOut();
                        break;
                    default:
                        return Result<BookingDto>.Failure("Unsupported or invalid status transition.", 400);
                }

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
                }, "Booking status updated successfully.");
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return Result<BookingDto>.Failure($"Failed to update booking status: {ex.Message}", 500);
            }
        }
    }
}
