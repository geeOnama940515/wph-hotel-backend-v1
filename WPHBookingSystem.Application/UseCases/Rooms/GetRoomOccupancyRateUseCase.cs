using System;
using System.Threading.Tasks;
using WPHBookingSystem.Application.Common;
using WPHBookingSystem.Application.Interfaces;
using WPHBookingSystem.Domain.Entities;

namespace WPHBookingSystem.Application.UseCases.Rooms
{
    /// <summary>
    /// Request record for room occupancy rate calculations.
    /// </summary>
    /// <param name="RoomId">The unique identifier of the room to calculate occupancy for.</param>
    /// <param name="StartDate">The start date for the occupancy calculation period.</param>
    /// <param name="EndDate">The end date for the occupancy calculation period.</param>
    public record GetRoomOccupancyRateRequest(Guid RoomId, DateTime StartDate, DateTime EndDate);
    
    /// <summary>
    /// Response record for room occupancy rate results.
    /// </summary>
    /// <param name="OccupancyRate">The occupancy rate as a percentage (0-100).</param>
    public record GetRoomOccupancyRateResponse(int OccupancyRate);
    
    /// <summary>
    /// Use case responsible for calculating room occupancy rates for business intelligence.
    /// Provides insights into room utilization patterns.
    /// </summary>
    public class GetRoomOccupancyRateUseCase
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetRoomOccupancyRateUseCase(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Calculates the occupancy rate for a room during the specified date range.
        /// Delegates to domain logic for occupancy calculation.
        /// </summary>
        /// <param name="request">The request containing room ID and date range for calculation.</param>
        /// <returns>A result containing the occupancy rate percentage or error details.</returns>
        public async Task<Result<GetRoomOccupancyRateResponse>> ExecuteAsync(GetRoomOccupancyRateRequest request)
        {
            try
            {
                var room = await _unitOfWork.Repository<Room>().GetByIdAsync(request.RoomId);
                if (room == null)
                    return Result<GetRoomOccupancyRateResponse>.Failure("Room not found.", 404);

                var rate = room.GetOccupancyRate(request.StartDate, request.EndDate);
                return Result<GetRoomOccupancyRateResponse>.Success(new GetRoomOccupancyRateResponse(rate), "Room occupancy rate retrieved successfully.");
            }
            catch (Exception ex)
            {
                return Result<GetRoomOccupancyRateResponse>.Failure($"Failed to retrieve room occupancy rate: {ex.Message}", 500);
            }
        }
    }
}
