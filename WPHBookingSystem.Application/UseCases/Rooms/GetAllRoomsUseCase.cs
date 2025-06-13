using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPHBookingSystem.Application.DTOs.Room;
using WPHBookingSystem.Application.Interfaces;

namespace WPHBookingSystem.Application.UseCases.Rooms
{
    public class GetAllRoomsUseCase
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetAllRoomsUseCase(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<RoomDto>> ExecuteAsync()
        {
            var rooms = await _unitOfWork.Rooms.GetAllAsync();

            return rooms.Select(room => new RoomDto
            {
                Id = room.Id,
                Name = room.Name,
                Description = room.Description,
                Price = room.Price,
                Capacity = room.Capacity,
                Image = room.Image,
                Status = room.Status
            }).ToList();
        }
    }
}
