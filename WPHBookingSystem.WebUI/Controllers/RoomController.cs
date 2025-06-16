using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WPHBookingSystem.Application.DTOs.Room;
using WPHBookingSystem.Application.Interfaces.Services;

namespace WPHBookingSystem.WebUI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoomController : ControllerBase
    {
        private readonly IBookingSystemFacade _bookingSystemFacade;

        public RoomController(IBookingSystemFacade bookingSystemFacade)
        {
            _bookingSystemFacade = bookingSystemFacade;
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Guid>> CreateRoom(CreateRoomDto dto)
        {
            var roomId = await _bookingSystemFacade.CreateRoom(dto);
            return Ok(roomId);
        }

        [HttpPut]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<RoomDto>> UpdateRoom(UpdateRoomDto dto)
        {
            var room = await _bookingSystemFacade.UpdateRoom(dto);
            return Ok(room);
        }

        [HttpPut("{roomId}/status")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<RoomDto>> UpdateRoomStatus(Guid roomId, [FromBody] bool isAvailable)
        {
            var room = await _bookingSystemFacade.UpdateRoomStatus(roomId, isAvailable);
            return Ok(room);
        }

        [HttpGet("{roomId}")]
        public async Task<ActionResult<RoomDto>> GetRoomById(Guid roomId)
        {
            var room = await _bookingSystemFacade.GetRoomById(roomId);
            return Ok(room);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<RoomDto>>> GetAllRooms()
        {
            var rooms = await _bookingSystemFacade.GetAllRooms();
            return Ok(rooms);
        }

        [HttpDelete("{roomId}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<bool>> DeleteRoom(Guid roomId)
        {
            var result = await _bookingSystemFacade.DeleteRoom(roomId);
            return Ok(result);
        }

        [HttpGet("{roomId}/availability")]
        public async Task<ActionResult<bool>> CheckRoomAvailability(
            Guid roomId,
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate)
        {
            var isAvailable = await _bookingSystemFacade.CheckRoomAvailability(roomId, startDate, endDate);
            return Ok(isAvailable);
        }

        [HttpGet("{roomId}/occupancy-rate")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<decimal>> GetRoomOccupancyRate(
            Guid roomId,
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate)
        {
            var occupancyRate = await _bookingSystemFacade.GetRoomOccupancyRate(roomId, startDate, endDate);
            return Ok(occupancyRate);
        }

        [HttpGet("{roomId}/revenue")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<decimal>> GetRoomRevenue(
            Guid roomId,
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate)
        {
            var revenue = await _bookingSystemFacade.GetRoomRevenue(roomId, startDate, endDate);
            return Ok(revenue);
        }
    }
} 