using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPHBookingSystem.Application.Exceptions;
using WPHBookingSystem.Application.Interfaces;
using WPHBookingSystem.Domain.Exceptions;

namespace WPHBookingSystem.Application.UseCases.Rooms
{
    public class DeleteRoomUseCase
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteRoomUseCase(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task ExecuteAsync(Guid roomId)
        {
            var room = await _unitOfWork.Rooms.GetByIdAsync(roomId);

            if (room == null)
                throw new NotFoundException("Room not found.");

            // Optional: Ensure no future bookings exist before deletion
            var hasFutureBookings = room.Bookings.Any(b =>
                b.CheckIn > DateTime.UtcNow &&
                (b.Status == Domain.Enums.BookingStatus.Confirmed || b.Status == Domain.Enums.BookingStatus.Pending));

            if (hasFutureBookings)
                throw new DomainException("Cannot delete room with future bookings.");

            await _unitOfWork.Rooms.DeleteAsync(roomId);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
