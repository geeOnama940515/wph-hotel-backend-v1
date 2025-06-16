using System;
using System.Threading.Tasks;
using WPHBookingSystem.Application.Common;
using WPHBookingSystem.Application.DTOs.Room;
using WPHBookingSystem.Application.Interfaces;
using WPHBookingSystem.Domain.Entities;

namespace WPHBookingSystem.Application.UseCases.Rooms
{
    public class CreateRoomUseCase
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateRoomUseCase(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Guid>> ExecuteAsync(CreateRoomDto dto)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                var room = Room.Create(dto.Name, dto.Description, dto.Price, dto.Capacity, dto.Images);
                await _unitOfWork.Repository<Room>().AddAsync(room);
                await _unitOfWork.CommitTransactionAsync();

                return Result<Guid>.Success(room.Id, "Room created successfully.");
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return Result<Guid>.Failure($"Failed to create room: {ex.Message}", 500);
            }
        }
    }
}
