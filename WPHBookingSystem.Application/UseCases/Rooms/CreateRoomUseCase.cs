using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPHBookingSystem.Application.DTOs.Room;
using WPHBookingSystem.Application.Interfaces;
using WPHBookingSystem.Domain.Entities;

namespace WPHBookingSystem.Application.UseCases.Rooms
{
    public class CreateRoomUseCase
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateRoomUseCase(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Guid> ExecuteAsync(CreateRoomDto dto)
        {
            var room = Room.Create(dto.Name, dto.Description, dto.Price, dto.Capacity, dto.Images);
            await _unitOfWork.Rooms.AddAsync(room);
            await _unitOfWork.SaveChangesAsync();
            return room.Id;
        }
    }
}
