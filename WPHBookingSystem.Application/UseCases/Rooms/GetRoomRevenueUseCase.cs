using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPHBookingSystem.Application.Exceptions;
using WPHBookingSystem.Application.Interfaces;

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

        public async Task<GetRoomRevenueResponse> ExecuteAsync(GetRoomRevenueRequest request)
        {
            var room = await _unitOfWork.Rooms.GetByIdAsync(request.RoomId)
                        ?? throw new NotFoundException("Room not found.");

            var total = room.CalculateRevenue(request.StartDate, request.EndDate);
            return new GetRoomRevenueResponse(total);
        }
    }
}
