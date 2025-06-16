using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WPHBookingSystem.Application.DTOs.Room;
using WPHBookingSystem.Application.Interfaces.Services;
using WPHBookingSystem.Application.UseCases.Rooms;
using static WPHBookingSystem.Application.UseCases.Rooms.CheckRoomAvailabilityUseCase;

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
        public async Task<ActionResult<RoomDto>> UpdateRoom(Guid roomId,UpdateRoomDto dto)
        {
            var room = await _bookingSystemFacade.UpdateRoom(roomId,dto);
            return Ok(room);
        }

        [HttpPut("{roomId}/status")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<RoomDto>> UpdateRoomStatus([FromBody] UpdateRoomStatusRequest request)
        {
            var room = await _bookingSystemFacade.UpdateRoomStatus(request);
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

        [HttpGet("room-availability")]
        public async Task<ActionResult<bool>> CheckRoomAvailability([FromBody] CheckRoomAvailabilityRequest request)
        {
            //this.request.RoomId = roomId;
            var isAvailable = await _bookingSystemFacade.CheckRoomAvailability(request);
            return Ok(isAvailable);
        }

        [HttpGet("room-occupancy-rate")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<decimal>> GetRoomOccupancyRate([FromBody] GetRoomOccupancyRateRequest request)
        {
            var occupancyRate = await _bookingSystemFacade.GetRoomOccupancyRate(request);
            return Ok(occupancyRate);
        }

        [HttpGet("room-revenue")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<decimal>> GetRoomRevenue([FromBody] GetRoomRevenueRequest request)
        {
            var revenue = await _bookingSystemFacade.GetRoomRevenue(request);
            return Ok(revenue);
        }
    }
} 