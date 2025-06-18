using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WPHBookingSystem.Application.DTOs.Room;
using WPHBookingSystem.Application.Interfaces.Services;
using WPHBookingSystem.Application.UseCases.Rooms;
using static WPHBookingSystem.Application.UseCases.Rooms.CheckRoomAvailabilityUseCase;
using Microsoft.AspNetCore.Http;
using WPHBookingSystem.WebUI.Extensions;

namespace WPHBookingSystem.WebUI.Controllers
{
    /// <summary>
    /// Room controller providing hotel room management and availability endpoints.
    /// 
    /// This controller handles room-related operations including CRUD operations,
    /// availability checking, and administrative functions like occupancy rates
    /// and revenue tracking. Some endpoints require admin privileges while others
    /// are publicly accessible for room browsing and availability checking.
    /// 
    /// The controller uses the BookingSystemFacade to coordinate business logic
    /// and provides both public and administrative room management capabilities.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class RoomController : ControllerBase
    {
        private readonly IBookingSystemFacade _bookingSystemFacade;

        /// <summary>
        /// Initializes a new instance of the RoomController with the booking system facade.
        /// </summary>
        /// <param name="bookingSystemFacade">Service facade for coordinating room operations</param>
        public RoomController(IBookingSystemFacade bookingSystemFacade)
        {
            _bookingSystemFacade = bookingSystemFacade;
        }

        /// <summary>
        /// Creates a new room in the hotel inventory.
        /// 
        /// Admin-only endpoint that adds a new room with specified details including
        /// room type, capacity, amenities, and pricing information.
        /// </summary>
        /// <param name="dto">Room creation data including type, capacity, and pricing</param>
        /// <returns>Unique identifier of the created room</returns>
        /// <response code="200">Room created successfully</response>
        /// <response code="400">Invalid room data</response>
        /// <response code="401">User not authenticated</response>
        /// <response code="403">User not authorized (admin role required)</response>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Guid>> CreateRoom(CreateRoomDto dto)
        {
            var roomId = await _bookingSystemFacade.CreateRoom(dto);
            return Ok(roomId);
        }

        /// <summary>
        /// Updates an existing room's information.
        /// 
        /// Admin-only endpoint that allows modification of room details including
        /// pricing, amenities, and room type. Validates room existence before update.
        /// </summary>
        /// <param name="roomId">Unique identifier of the room to update</param>
        /// <param name="dto">Updated room information</param>
        /// <returns>Updated room details</returns>
        /// <response code="200">Room updated successfully</response>
        /// <response code="400">Invalid room data</response>
        /// <response code="404">Room not found</response>
        /// <response code="403">User not authorized (admin role required)</response>
        [HttpPut]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<RoomDto>> UpdateRoom(Guid roomId,UpdateRoomDto dto)
        {
            var room = await _bookingSystemFacade.UpdateRoom(roomId,dto);
            return Ok(room);
        }

        /// <summary>
        /// Updates the status of a room (e.g., available, maintenance, out of service).
        /// 
        /// Admin-only endpoint for managing room availability status. Useful for
        /// maintenance scheduling and temporary room unavailability.
        /// </summary>
        /// <param name="request">Room status update request with room ID and new status</param>
        /// <returns>Updated room details with new status</returns>
        /// <response code="200">Room status updated successfully</response>
        /// <response code="400">Invalid status or room not found</response>
        /// <response code="403">User not authorized (admin role required)</response>
        [HttpPut("{roomId}/status")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<RoomDto>> UpdateRoomStatus([FromBody] UpdateRoomStatusRequest request)
        {
            var room = await _bookingSystemFacade.UpdateRoomStatus(request);
            return Ok(room);
        }

        /// <summary>
        /// Retrieves detailed information about a specific room.
        /// 
        /// Public endpoint that returns comprehensive room details including
        /// amenities, pricing, current status, and availability information.
        /// </summary>
        /// <param name="roomId">Unique identifier of the room</param>
        /// <returns>Detailed room information</returns>
        /// <response code="200">Room details retrieved successfully</response>
        /// <response code="404">Room not found</response>
        [HttpGet("{roomId}")]
        public async Task<ActionResult<RoomDto>> GetRoomById(Guid roomId)
        {
            var room = await _bookingSystemFacade.GetRoomById(roomId);
            return Ok(room);
        }

        /// <summary>
        /// Retrieves all available rooms in the hotel.
        /// 
        /// Public endpoint that returns a list of all rooms with their basic
        /// information, useful for room browsing and selection.
        /// </summary>
        /// <returns>List of all rooms in the hotel</returns>
        /// <response code="200">Rooms retrieved successfully</response>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RoomDto>>> GetAllRooms()
        {
            var rooms = await _bookingSystemFacade.GetAllRooms();
            return Ok(rooms);
        }

        /// <summary>
        /// Permanently removes a room from the hotel inventory.
        /// 
        /// Admin-only endpoint that deletes a room and all associated data.
        /// Should be used carefully as this operation cannot be undone.
        /// </summary>
        /// <param name="roomId">Unique identifier of the room to delete</param>
        /// <returns>Boolean indicating successful deletion</returns>
        /// <response code="200">Room deleted successfully</response>
        /// <response code="404">Room not found</response>
        /// <response code="403">User not authorized (admin role required)</response>
        [HttpDelete("{roomId}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<bool>> DeleteRoom(Guid roomId)
        {
            var result = await _bookingSystemFacade.DeleteRoom(roomId);
            return Ok(result);
        }

        /// <summary>
        /// Checks if a specific room is available for booking during given dates.
        /// 
        /// Public endpoint that validates room availability by checking existing
        /// bookings and room status for the specified date range.
        /// </summary>
        /// <param name="request">Availability check request with room ID and date range</param>
        /// <returns>Boolean indicating room availability</returns>
        /// <response code="200">Availability check completed</response>
        /// <response code="400">Invalid request parameters</response>
        [HttpGet("room-availability")]
        public async Task<ActionResult<bool>> CheckRoomAvailability([FromBody] CheckRoomAvailabilityRequest request)
        {
            //this.request.RoomId = roomId;
            var isAvailable = await _bookingSystemFacade.CheckRoomAvailability(request);
            return Ok(isAvailable);
        }

        /// <summary>
        /// Calculates the occupancy rate for a room over a specified period.
        /// 
        /// Admin-only endpoint that provides business intelligence by calculating
        /// the percentage of time a room was occupied during the specified period.
        /// </summary>
        /// <param name="request">Occupancy rate request with room ID and date range</param>
        /// <returns>Occupancy rate as a decimal percentage</returns>
        /// <response code="200">Occupancy rate calculated successfully</response>
        /// <response code="400">Invalid request parameters</response>
        /// <response code="403">User not authorized (admin role required)</response>
        [HttpGet("room-occupancy-rate")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<decimal>> GetRoomOccupancyRate([FromBody] GetRoomOccupancyRateRequest request)
        {
            var occupancyRate = await _bookingSystemFacade.GetRoomOccupancyRate(request);
            return Ok(occupancyRate);
        }

        /// <summary>
        /// Calculates the total revenue generated by a room over a specified period.
        /// 
        /// Admin-only endpoint that provides financial reporting by calculating
        /// the total revenue from all bookings for a specific room and time period.
        /// </summary>
        /// <param name="request">Revenue calculation request with room ID and date range</param>
        /// <returns>Total revenue as a decimal value</returns>
        /// <response code="200">Revenue calculated successfully</response>
        /// <response code="400">Invalid request parameters</response>
        /// <response code="403">User not authorized (admin role required)</response>
        [HttpGet("room-revenue")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<decimal>> GetRoomRevenue([FromBody] GetRoomRevenueRequest request)
        {
            var revenue = await _bookingSystemFacade.GetRoomRevenue(request);
            return Ok(revenue);
        }

        /// <summary>
        /// Uploads multiple images to a room.
        /// 
        /// Admin-only endpoint that allows uploading multiple image files to a room.
        /// Supports various image formats and includes validation for file size and type.
        /// </summary>
        /// <param name="roomId">The ID of the room to upload images for</param>
        /// <param name="files">Collection of image files to upload</param>
        /// <returns>Upload results with file information and any errors</returns>
        /// <response code="200">Images uploaded successfully</response>
        /// <response code="400">Invalid files or room not found</response>
        /// <response code="401">User not authenticated</response>
        /// <response code="403">User not authorized (admin role required)</response>
        /// 

        [AllowAnonymous]
        [HttpPost("{roomId}/images")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadRoomImages(Guid roomId, [FromForm] List<IFormFile> files)
        {
            // If your business logic expects IFormFileCollection, convert:
            var fileCollection = new FormFileCollection();
            foreach (var file in files)
                fileCollection.Add(file);

            var result = await _bookingSystemFacade.UploadRoomImages(roomId, fileCollection);
            return this.CreateResponse(result);
        }

        /// <summary>
        /// Uploads a single image to a room.
        /// 
        /// Admin-only endpoint that allows uploading a single image file to a room.
        /// Supports various image formats and includes validation for file size and type.
        /// </summary>
        /// <param name="roomId">The ID of the room to upload the image for</param>
        /// <param name="file">The image file to upload</param>
        /// <returns>Upload result with file information</returns>
        /// <response code="200">Image uploaded successfully</response>
        /// <response code="400">Invalid file or room not found</response>
        /// <response code="401">User not authenticated</response>
        /// <response code="403">User not authorized (admin role required)</response>
        [HttpPost("{roomId}/image")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UploadRoomImage(Guid roomId, IFormFile file)
        {
            var files = new FormFileCollection { file };
            var result = await _bookingSystemFacade.UploadRoomImages(roomId, files);
            return this.CreateResponse(result);
        }
    }
} 