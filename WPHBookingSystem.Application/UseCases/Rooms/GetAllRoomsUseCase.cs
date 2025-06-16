using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WPHBookingSystem.Application.Common;
using WPHBookingSystem.Application.DTOs.Room;
using WPHBookingSystem.Application.Interfaces;
using WPHBookingSystem.Domain.Entities;

namespace WPHBookingSystem.Application.UseCases.Rooms
{
    public class GetAllRoomsUseCase
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetAllRoomsUseCase(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<List<RoomDto>>> ExecuteAsync()
        {
            try
            {
                var rooms = await _unitOfWork.Repository<Room>().GetAllAsync();

                var roomDtos = rooms.Select(room => new RoomDto
                {
                    Id = room.Id,
                    Name = room.Name,
                    Description = room.Description,
                    Price = room.Price,
                    Capacity = room.Capacity,
                    Images = room.Images,
                    Status = room.Status
                }).ToList();

                return Result<List<RoomDto>>.Success(roomDtos, "Rooms retrieved successfully.");
            }
            catch (Exception ex)
            {
                return Result<List<RoomDto>>.Failure($"Failed to retrieve rooms: {ex.Message}", 500);
            }
        }
    }
}
