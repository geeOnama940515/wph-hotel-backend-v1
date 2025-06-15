using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPHBookingSystem.Application.Exceptions;
using WPHBookingSystem.Application.Interfaces;

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

        public async Task<CheckRoomAvailabilityResponse> ExecuteAsync(CheckRoomAvailabilityRequest request)
        {
            var room = await _unitOfWork.Rooms.GetByIdWithBookingsAsync(request.RoomId)
                            ?? throw new NotFoundException("Room not found.");

            var isAvailable = room.IsAvailable(request.CheckIn, request.CheckOut);
            return new CheckRoomAvailabilityResponse(isAvailable);
        }
    }
}
