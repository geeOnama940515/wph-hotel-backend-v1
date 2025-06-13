using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPHBookingSystem.Application.Exceptions;
using WPHBookingSystem.Application.Interfaces;

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

        public async Task<GetRoomOccupancyRateResponse> ExecuteAsync(GetRoomOccupancyRateRequest request)
        {
            var room = await _unitOfWork.Rooms.GetByIdAsync(request.RoomId)
                        ?? throw new NotFoundException("Room not found.");

            var rate = room.GetOccupancyRate(request.StartDate, request.EndDate);
            return new GetRoomOccupancyRateResponse(rate);
        }
    }
}
