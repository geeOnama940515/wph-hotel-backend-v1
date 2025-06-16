using System;
using System.Linq;
using System.Threading.Tasks;
using WPHBookingSystem.Application.Common;
using WPHBookingSystem.Application.Interfaces;
using WPHBookingSystem.Domain.Entities;

namespace WPHBookingSystem.Application.UseCases.Rooms
{
    public class DeleteRoomUseCase
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteRoomUseCase(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<bool>> ExecuteAsync(Guid roomId)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                var room = await _unitOfWork.Repository<Room>().GetByIdAsync(roomId);
                if (room == null)
                    return Result<bool>.Failure("Room not found.", 404);

                // Optional: Ensure no future bookings exist before deletion
                var hasFutureBookings = room.Bookings.Any(b =>
                    b.CheckIn > DateTime.UtcNow &&
                    (b.Status == Domain.Enums.BookingStatus.Confirmed || b.Status == Domain.Enums.BookingStatus.Pending));

                if (hasFutureBookings)
                    return Result<bool>.Failure("Cannot delete room with future bookings.", 400);

                await _unitOfWork.Repository<Room>().DeleteAsync(roomId);
                await _unitOfWork.CommitTransactionAsync();

                return Result<bool>.Success(true, "Room deleted successfully.");
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return Result<bool>.Failure($"Failed to delete room: {ex.Message}", 500);
            }
        }
    }
}
