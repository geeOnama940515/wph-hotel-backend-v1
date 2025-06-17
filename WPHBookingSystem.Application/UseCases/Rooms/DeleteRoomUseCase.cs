using System;
using System.Linq;
using System.Threading.Tasks;
using WPHBookingSystem.Application.Common;
using WPHBookingSystem.Application.Interfaces;
using WPHBookingSystem.Domain.Entities;

namespace WPHBookingSystem.Application.UseCases.Rooms
{
    /// <summary>
    /// Use case responsible for deleting rooms from the system.
    /// Handles room deletion business logic and validation of deletion constraints.
    /// </summary>
    public class DeleteRoomUseCase
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteRoomUseCase(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Deletes a room after validating that it has no future bookings.
        /// Ensures data integrity by preventing deletion of rooms with active reservations.
        /// </summary>
        /// <param name="roomId">The unique identifier of the room to delete.</param>
        /// <returns>A result indicating success or failure of the deletion operation.</returns>
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
