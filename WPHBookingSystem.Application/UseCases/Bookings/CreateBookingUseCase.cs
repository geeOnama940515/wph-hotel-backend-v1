using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPHBookingSystem.Application.DTOs.Booking;
using WPHBookingSystem.Application.Interfaces;
using WPHBookingSystem.Domain.Entities;
using WPHBookingSystem.Domain.Exceptions;

namespace WPHBookingSystem.Application.UseCases.Bookings
{
    public class CreateBookingUseCase
    {
        private readonly IUnitOfWork _unitOfWork;
        public CreateBookingUseCase(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }
        public async Task<Guid> ExecuteAsync(CreateBookingDto dto, Guid userId)
        {
            var room = await _unitOfWork.Rooms.GetByIdAsync(dto.RoomId)
                       ?? throw new DomainException("Room not found.");

            if (!room.IsAvailable(dto.CheckIn, dto.CheckOut))
                throw new DomainException("Room is not available on selected dates.");

            var booking = Booking.Create(
                userId,
                room.Id,
                dto.CheckIn,
                dto.CheckOut,
                dto.Guests,
                room.Price * (decimal)(dto.CheckOut - dto.CheckIn).TotalDays,
                dto.SpecialRequests,
                dto.Phone,
                dto.Address
            );

            await _unitOfWork.Bookings.AddAsync(booking);
            await _unitOfWork.SaveChangesAsync();

            return booking.Id;
        }
    }
}
