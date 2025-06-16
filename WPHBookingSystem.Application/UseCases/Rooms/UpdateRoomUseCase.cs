using System;
using System.Threading.Tasks;
using WPHBookingSystem.Application.Common;
using WPHBookingSystem.Application.DTOs.Room;
using WPHBookingSystem.Application.Interfaces;
using WPHBookingSystem.Domain.Entities;

namespace WPHBookingSystem.Application.UseCases.Rooms
{
    public class UpdateRoomUseCase
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateRoomUseCase(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<RoomDto>> ExecuteAsync(Guid roomId, UpdateRoomDto dto)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                var room = await _unitOfWork.Repository<Room>().GetByIdAsync(roomId);
                if (room == null)
                    return Result<RoomDto>.Failure("Room not found.", 404);

                room.UpdateDetails(dto.Name, dto.Description, dto.Price, dto.Capacity, dto.Images);
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
                }, "Room updated successfully.");
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return Result<RoomDto>.Failure($"Failed to update room: {ex.Message}", 500);
            }
        }
    }
}
