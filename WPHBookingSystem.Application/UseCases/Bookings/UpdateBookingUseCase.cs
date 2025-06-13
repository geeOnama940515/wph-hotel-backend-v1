using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPHBookingSystem.Application.DTOs.Booking;
using WPHBookingSystem.Application.Interfaces;
using WPHBookingSystem.Domain.Exceptions;

namespace WPHBookingSystem.Application.UseCases.Bookings
{
    public class UpdateBookingDatesUseCase
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateBookingDatesUseCase(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task ExecuteAsync(Guid bookingId, Guid userId, UpdateBookingDateDto dto)
        {
            var booking = await _unitOfWork.Bookings.GetByIdAsync(bookingId)
                          ?? throw new DomainException("Booking not found.");

            if (booking.UserId != userId)
                throw new DomainException("You are not authorized to update this booking.");

            booking.UpdateBookingDates(dto.CheckIn, dto.CheckOut);

            await _unitOfWork.Bookings.UpdateAsync(booking);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
