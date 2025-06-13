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
    public class GetRoomByIdUseCase
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetRoomByIdUseCase(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<RoomDto> ExecuteAsync(Guid roomId)
        {
            var room = await _unitOfWork.Rooms.GetByIdAsync(roomId);

            if (room == null)
                throw new NotFoundException("Room not found.");

            return new RoomDto
            {
                Id = room.Id,
                Name = room.Name,
                Description = room.Description,
                Price = room.Price,
                Capacity = room.Capacity,
                Image = room.Image,
                Status = room.Status
            };
        }
    }
}
