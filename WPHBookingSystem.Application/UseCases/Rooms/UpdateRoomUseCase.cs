using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPHBookingSystem.Application.DTOs.Room;
using WPHBookingSystem.Application.Exceptions;
using WPHBookingSystem.Application.Interfaces;
using WPHBookingSystem.Domain.Exceptions;

namespace WPHBookingSystem.Application.UseCases.Rooms
{
    public class UpdateRoomUseCase
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateRoomUseCase(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task ExecuteAsync(Guid roomId, UpdateRoomDto dto)
        {
            var room = await _unitOfWork.Rooms.GetByIdAsync(roomId);

            if (room == null)
                throw new NotFoundException("Room not found.");

            room.UpdateDetails(dto.Name, dto.Description, dto.Price, dto.Capacity, dto.Images);

            await _unitOfWork.SaveChangesAsync();
        }
    }
}
