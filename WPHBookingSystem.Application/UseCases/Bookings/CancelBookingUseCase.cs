using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPHBookingSystem.Application.Interfaces;
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

        public async Task ExecuteAsync(Guid bookingId, string emailAddress)
        {
            var booking = await _unitOfWork.Bookings.GetByIdAsync(bookingId)
                          ?? throw new DomainException("Booking not found.");

            if (booking.EmailAddress != emailAddress)
                throw new DomainException("You are not authorized to cancel this booking.");

            booking.Cancel();

            await _unitOfWork.Bookings.UpdateAsync(booking);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
