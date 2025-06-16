using System;
using System.Threading.Tasks;
using WPHBookingSystem.Application.Common;
using WPHBookingSystem.Application.Interfaces;
using WPHBookingSystem.Domain.Entities;

namespace WPHBookingSystem.Application.UseCases.Rooms
{
    public record GetRoomOccupancyRateRequest(Guid RoomId, DateTime StartDate, DateTime EndDate);
    public record GetRoomOccupancyRateResponse(int OccupancyRate);
    public class GetRoomOccupancyRateUseCase
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetRoomOccupancyRateUseCase(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

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
