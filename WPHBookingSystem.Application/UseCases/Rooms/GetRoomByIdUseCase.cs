using System;
using System.Threading.Tasks;
using WPHBookingSystem.Application.Common;
using WPHBookingSystem.Application.DTOs.Room;
using WPHBookingSystem.Application.Interfaces;
using WPHBookingSystem.Domain.Entities;

namespace WPHBookingSystem.Application.UseCases.Rooms
{
    /// <summary>
    /// Use case responsible for retrieving a specific room by its unique identifier.
    /// Provides detailed room information for individual room views.
    /// </summary>
    public class GetRoomByIdUseCase
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetRoomByIdUseCase(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Retrieves a room by its ID and maps it to a DTO for API response.
        /// </summary>
        /// <param name="roomId">The unique identifier of the room to retrieve.</param>
        /// <returns>A result containing the room information or error details.</returns>
        public async Task<Result<RoomDto>> ExecuteAsync(Guid roomId)
        {
            try
            {
                var room = await _unitOfWork.Repository<Room>().GetByIdAsync(roomId);
                if (room == null)
                    return Result<RoomDto>.Failure("Room not found.", 404);

                return Result<RoomDto>.Success(new RoomDto
                {
                    Id = room.Id,
                    Name = room.Name,
                    Description = room.Description,
                    Price = room.Price,
                    Capacity = room.Capacity,
                    Images = room.Images,
                    Status = room.Status
                }, "Room retrieved successfully.");
            }
            catch (Exception ex)
            {
                return Result<RoomDto>.Failure($"Failed to retrieve room: {ex.Message}", 500);
            }
        }
    }
}
