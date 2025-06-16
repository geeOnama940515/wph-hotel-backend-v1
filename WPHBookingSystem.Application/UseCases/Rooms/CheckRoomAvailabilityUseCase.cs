using System;
using System.Threading.Tasks;
using WPHBookingSystem.Application.Common;
using WPHBookingSystem.Application.Interfaces;
using WPHBookingSystem.Domain.Entities;

namespace WPHBookingSystem.Application.UseCases.Rooms
{
    public class CheckRoomAvailabilityUseCase
    {
        public record CheckRoomAvailabilityRequest(Guid RoomId, DateTime CheckIn, DateTime CheckOut);
        public record CheckRoomAvailabilityResponse(bool IsAvailable);

        private readonly IUnitOfWork _unitOfWork;

        public CheckRoomAvailabilityUseCase(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<CheckRoomAvailabilityResponse>> ExecuteAsync(CheckRoomAvailabilityRequest request)
        {
            try
            {
                var room = await _unitOfWork.Repository<Room>().GetByIdAsync(request.RoomId);
                if (room == null)
                    return Result<CheckRoomAvailabilityResponse>.Failure("Room not found.", 404);

                var isAvailable = room.IsAvailable(request.CheckIn, request.CheckOut);
                return Result<CheckRoomAvailabilityResponse>.Success(new CheckRoomAvailabilityResponse(isAvailable), "Room availability checked successfully.");
            }
            catch (Exception ex)
            {
                return Result<CheckRoomAvailabilityResponse>.Failure($"Failed to check room availability: {ex.Message}", 500);
            }
        }
    }
}
