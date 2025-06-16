using System;
using System.Threading.Tasks;
using WPHBookingSystem.Application.Common;
using WPHBookingSystem.Application.Interfaces;
using WPHBookingSystem.Domain.Entities;

namespace WPHBookingSystem.Application.UseCases.Rooms
{
    public record GetRoomRevenueRequest(Guid RoomId, DateTime? StartDate = null, DateTime? EndDate = null);
    public record GetRoomRevenueResponse(decimal TotalRevenue);
    public class GetRoomRevenueUseCase
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetRoomRevenueUseCase(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<GetRoomRevenueResponse>> ExecuteAsync(GetRoomRevenueRequest request)
        {
            try
            {
                var room = await _unitOfWork.Repository<Room>().GetByIdAsync(request.RoomId);
                if (room == null)
                    return Result<GetRoomRevenueResponse>.Failure("Room not found.", 404);

                var total = room.CalculateRevenue(request.StartDate, request.EndDate);
                return Result<GetRoomRevenueResponse>.Success(new GetRoomRevenueResponse(total), "Room revenue retrieved successfully.");
            }
            catch (Exception ex)
            {
                return Result<GetRoomRevenueResponse>.Failure($"Failed to retrieve room revenue: {ex.Message}", 500);
            }
        }
    }
}
