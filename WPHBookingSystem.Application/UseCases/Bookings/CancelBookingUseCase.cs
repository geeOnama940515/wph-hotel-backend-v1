using System;
using System.Threading.Tasks;
using WPHBookingSystem.Application.Common;
using WPHBookingSystem.Application.DTOs.Booking;
using WPHBookingSystem.Application.Interfaces;
using WPHBookingSystem.Application.Interfaces.Services;
using WPHBookingSystem.Domain.Entities;
using WPHBookingSystem.Domain.Exceptions;

namespace WPHBookingSystem.Application.UseCases.Bookings
{
    /// <summary>
    /// Use case responsible for cancelling existing bookings.
    /// Handles booking cancellation business logic and status updates.
    /// </summary>
    public class CancelBookingUseCase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailSenderService _emailService;

        public CancelBookingUseCase(IUnitOfWork unitOfWork,
            IEmailSenderService emailService)
        {
            _unitOfWork = unitOfWork;
            _emailService = emailService;
        }

        /// <summary>
        /// Cancels a booking by updating its status and persisting the changes.
        /// Validates booking existence and enforces business rules for cancellation.
        /// </summary>
        /// <param name="bookingId">The unique identifier of the booking to cancel.</param>
        /// <returns>A result containing the cancelled booking information or error details.</returns>
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
                //await _unitOfWork.RoomRepository.UpdateAsync(booking.Room, room => room.SetAvailable());

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
                await _emailService.SendBookingCancellationAsync(bookingDto, booking.EmailAddress,booking.GuestName);
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
