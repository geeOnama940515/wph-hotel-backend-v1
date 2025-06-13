using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static WPHBookingSystem.Application.UseCases.Rooms.UpdateStatus;
using WPHBookingSystem.Application.Interfaces;
using WPHBookingSystem.Domain.Enums;
using WPHBookingSystem.Domain.Exceptions;
using WPHBookingSystem.Application.Exceptions;

namespace WPHBookingSystem.Application.UseCases.Rooms
{
    public record UpdateRoomStatusRequest(Guid RoomId, RoomStatus NewStatus);
    public class UpdateRoomStatusUseCase
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateRoomStatusUseCase(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task ExecuteAsync(UpdateRoomStatusRequest request)
        {
            var room = await _unitOfWork.Rooms.GetByIdAsync(request.RoomId)
                        ?? throw new NotFoundException("Room not found.");

            switch (request.NewStatus)
            {
                case RoomStatus.Available:
                    room.Activate();
                    break;
                case RoomStatus.Inactive:
                    room.Deactivate();
                    break;
                case RoomStatus.Maintenance:
                    room.SetMaintenance();
                    break;
                default:
                    throw new DomainException("Invalid status update. Only Available, Inactive, and Maintenance are allowed.");
            }

            await _unitOfWork.SaveChangesAsync();
        }
    }
}
