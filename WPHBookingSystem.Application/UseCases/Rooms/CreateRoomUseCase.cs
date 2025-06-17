using System;
using System.Threading.Tasks;
using WPHBookingSystem.Application.Common;
using WPHBookingSystem.Application.DTOs.Room;
using WPHBookingSystem.Application.Interfaces;
using WPHBookingSystem.Domain.Entities;

namespace WPHBookingSystem.Application.UseCases.Rooms
{
    /// <summary>
    /// Use case responsible for creating new rooms in the hotel booking system.
    /// Handles room creation business logic and persistence within a transaction.
    /// </summary>
    public class CreateRoomUseCase
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateRoomUseCase(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Creates a new room with the specified details and persists it to the database.
        /// Uses domain factory method for validation and business rule enforcement.
        /// </summary>
        /// <param name="dto">The room creation data transfer object.</param>
        /// <returns>A result containing the created room ID or error details.</returns>
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
