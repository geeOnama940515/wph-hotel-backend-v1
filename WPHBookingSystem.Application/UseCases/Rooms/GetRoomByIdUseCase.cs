using System;
using System.Threading.Tasks;
using WPHBookingSystem.Application.Common;
using WPHBookingSystem.Application.DTOs.Room;
using WPHBookingSystem.Application.Interfaces;
using WPHBookingSystem.Domain.Entities;

namespace WPHBookingSystem.Application.UseCases.Rooms
{
    public class GetRoomByIdUseCase
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetRoomByIdUseCase(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

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
