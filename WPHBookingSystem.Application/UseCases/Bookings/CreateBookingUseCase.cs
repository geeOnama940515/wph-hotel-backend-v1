using System;
using System.Threading.Tasks;
using WPHBookingSystem.Application.Common;
using WPHBookingSystem.Application.DTOs.Booking;
using WPHBookingSystem.Application.Interfaces;
using WPHBookingSystem.Domain.Entities;
using WPHBookingSystem.Domain.Exceptions;
using WPHBookingSystem.Domain.ValueObjects;

namespace WPHBookingSystem.Application.UseCases.Bookings
{
    public class CreateBookingUseCase
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateBookingUseCase(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task<Result<BookingCreatedDto>> ExecuteAsync(CreateBookingDto dto)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                var room = await _unitOfWork.Repository<Room>().GetByIdAsync(dto.RoomId);
                if (room == null)
                    return Result<BookingCreatedDto>.Failure("Room not found.", 404);

                if (!room.IsAvailable(dto.CheckIn, dto.CheckOut))
                    return Result<BookingCreatedDto>.Failure("Room is not available on selected dates.", 400);

                var contactInfo = new ContactInfo(dto.Phone, dto.Address);
                var booking = Booking.Create(
                    room.Id,
                    dto.CheckIn,
                    dto.CheckOut,
                    dto.Guests,
                    room.Price * (decimal)(dto.CheckOut - dto.CheckIn).TotalDays,
                    contactInfo,
                    dto.EmailAddress,
                    dto.SpecialRequests
                );
               // booking.BookingToken = Guid.NewGuid().ToString();

                await _unitOfWork.Repository<Booking>().AddAsync(booking);
                await _unitOfWork.CommitTransactionAsync();

                return Result<BookingCreatedDto>.Success(new BookingCreatedDto { Id = booking.Id, BookingToken = booking.BookingToken }, "Booking created successfully.");
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return Result<BookingCreatedDto>.Failure($"Failed to create booking: {ex.Message}", 500);
            }
        }
    }
}
