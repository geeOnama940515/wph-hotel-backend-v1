using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPHBookingSystem.Application.DTOs.Booking;
using WPHBookingSystem.Application.Exceptions;
using WPHBookingSystem.Application.Interfaces;
using WPHBookingSystem.Domain.Enums;
using WPHBookingSystem.Domain.Exceptions;

namespace WPHBookingSystem.Application.UseCases.Bookings
{
    public class UpdateBookingStatusUseCase
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateBookingStatusUseCase(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task HandleAsync(UpdateBookingStatusRequest request)
        {
            var booking = await _unitOfWork.Bookings.GetByIdAsync(request.BookingId);
            if (booking == null)
                throw new NotFoundException("Booking not found.");

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

                default:
                    throw new DomainException("Unsupported or invalid status transition.");
            }

            await _unitOfWork.SaveChangesAsync();
        }
    }
}
