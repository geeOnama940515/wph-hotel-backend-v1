using System;
using System.Threading.Tasks;
using WPHBookingSystem.Application.Common;
using WPHBookingSystem.Application.Interfaces;
using WPHBookingSystem.Domain.Entities;

namespace WPHBookingSystem.Application.UseCases.Rooms
{
    /// <summary>
    /// Use case responsible for checking room availability for specific date ranges.
    /// Provides availability information for booking decisions.
    /// </summary>
    public class CheckRoomAvailabilityUseCase
    {
        /// <summary>
        /// Request record for room availability checks.
        /// </summary>
        /// <param name="RoomId">The unique identifier of the room to check.</param>
        /// <param name="CheckIn">The check-in date for the availability check.</param>
        /// <param name="CheckOut">The check-out date for the availability check.</param>
        public record CheckRoomAvailabilityRequest(Guid RoomId, DateTime CheckIn, DateTime CheckOut);
        
        /// <summary>
        /// Response record for room availability results.
        /// </summary>
        /// <param name="IsAvailable">Indicates whether the room is available for the specified dates.</param>
        public record CheckRoomAvailabilityResponse(bool IsAvailable);

        private readonly IUnitOfWork _unitOfWork;

        public CheckRoomAvailabilityUseCase(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Checks if a room is available for booking during the specified date range.
        /// Delegates to domain logic for availability determination.
        /// </summary>
        /// <param name="request">The request containing room ID and date range to check.</param>
        /// <returns>A result containing availability information or error details.</returns>
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
