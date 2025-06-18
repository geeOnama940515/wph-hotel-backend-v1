using System;
using System.Threading.Tasks;
using WPHBookingSystem.Application.Common;
using WPHBookingSystem.Application.DTOs.Room;
using WPHBookingSystem.Application.Interfaces;
using WPHBookingSystem.Domain.Entities;
using WPHBookingSystem.Domain.Enums;

namespace WPHBookingSystem.Application.UseCases.Rooms
{
    /// <summary>
    /// Request record for updating room status operations.
    /// </summary>
    /// <param name="RoomId">The unique identifier of the room to update.</param>
    /// <param name="NewStatus">The new status to assign to the room.</param>
    public record UpdateRoomStatusRequest(Guid RoomId, RoomStatus NewStatus);
    
    /// <summary>
    /// Use case responsible for updating room operational status.
    /// Handles status transition business logic and validation.
    /// </summary>
    public class UpdateRoomStatusUseCase
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateRoomStatusUseCase(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Updates room status based on the requested status transition.
        /// Validates room existence and enforces domain business rules for status changes.
        /// </summary>
        /// <param name="request">The request containing room ID and new status.</param>
        /// <returns>A result containing the updated room information or error details.</returns>
        public async Task<Result<RoomDto>> ExecuteAsync(UpdateRoomStatusRequest request)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                var room = await _unitOfWork.Repository<Room>().GetByIdAsync(request.RoomId);
                if (room == null)
                    return Result<RoomDto>.Failure("Room not found.", 404);

                switch (request.NewStatus)
                {
                    case RoomStatus.Available:
                        room.Activate();
                        break;
                    case RoomStatus.Inactive:
                        room.Deactivate();
                        break;
                    case RoomStatus.Maintenance:
                        room.SetMaintenance();
                        break;
                    case RoomStatus.Occupied:
                        break;
                    case RoomStatus.Booked:
                        break;
                    default:
                        return Result<RoomDto>.Failure("Invalid status update. Only Available, Inactive,Occupied,Booked, and Maintenance are allowed.", 400);
                }

                await _unitOfWork.Repository<Room>().UpdateAsync(room);
                await _unitOfWork.CommitTransactionAsync();

                return Result<RoomDto>.Success(new RoomDto
                {
                    Id = room.Id,
                    Name = room.Name,
                    Description = room.Description,
                    Price = room.Price,
                    Capacity = room.Capacity,
                    Images = room.Images,
                    Status = room.Status
                }, "Room status updated successfully.");
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return Result<RoomDto>.Failure($"Failed to update room status: {ex.Message}", 500);
            }
        }
    }
}
