using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WPHBookingSystem.Application.DTOs;
using WPHBookingSystem.Application.DTOs.Room;
using WPHBookingSystem.Application.Interfaces.Services;
using WPHBookingSystem.Application.UseCases.Rooms;
using WPHBookingSystem.WebUI.Extensions;
using static WPHBookingSystem.Application.UseCases.Rooms.CheckRoomAvailabilityUseCase;

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
        private readonly ILogger<RoomController> _logger;

        /// <summary>
        /// Initializes a new instance of the RoomController with the booking system facade.
        /// </summary>
        /// <param name="bookingSystemFacade">Service facade for coordinating room operations</param>
        /// <param name="logger">Logger for logging room-related operations</param>
        public RoomController(IBookingSystemFacade bookingSystemFacade, ILogger<RoomController> logger)
        {
            _bookingSystemFacade = bookingSystemFacade;
            _logger = logger;
            _logger.LogInformation("RoomController initialized");
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
        public async Task<IActionResult> CreateRoom(CreateRoomDto dto)
        {
            var roomId = await _bookingSystemFacade.CreateRoom(dto);
            return this.CreateResponse(200, "Room created successfully", roomId);
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
        public async Task<IActionResult> UpdateRoom(Guid roomId, UpdateRoomDto dto)
        {
            var room = await _bookingSystemFacade.UpdateRoom(roomId, dto);
            return this.CreateResponse(200, "Room updated successfully", room);
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
        public async Task<IActionResult> UpdateRoomStatus([FromBody] UpdateRoomStatusRequest request)
        {
            var room = await _bookingSystemFacade.UpdateRoomStatus(request);
            return this.CreateResponse(200, "Room status updated successfully", room);
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
        public async Task<IActionResult> GetRoomById(Guid roomId)
        {
            var room = await _bookingSystemFacade.GetRoomById(roomId);
            return this.CreateResponse(200, "Room details retrieved successfully", room);
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
        public async Task<IActionResult> GetAllRooms()
        {
            var rooms = await _bookingSystemFacade.GetAllRooms();
            return this.CreateResponse(200, "Rooms retrieved successfully", rooms);
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
        public async Task<IActionResult> DeleteRoom(Guid roomId)
        {
            var result = await _bookingSystemFacade.DeleteRoom(roomId);
            return this.CreateResponse(200, "Room deleted successfully", result);
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
        public async Task<IActionResult> CheckRoomAvailability([FromBody] CheckRoomAvailabilityRequest request)
        {
            var isAvailable = await _bookingSystemFacade.CheckRoomAvailability(request);
            return this.CreateResponse(200, "Availability check completed", isAvailable);
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
        public async Task<IActionResult> GetRoomOccupancyRate([FromBody] GetRoomOccupancyRateRequest request)
        {
            var occupancyRate = await _bookingSystemFacade.GetRoomOccupancyRate(request);
            return this.CreateResponse(200, "Occupancy rate calculated successfully", occupancyRate);
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
        public async Task<IActionResult> GetRoomRevenue([FromBody] GetRoomRevenueRequest request)
        {
            var revenue = await _bookingSystemFacade.GetRoomRevenue(request);
            return this.CreateResponse(200, "Revenue calculated successfully", revenue);
        }

        /// <summary>
        /// Test endpoint for file upload debugging.
        /// </summary>
        //[HttpPost("test-upload")]
        //[AllowAnonymous]
        //[Consumes("multipart/form-data")]
        //public async Task<IActionResult> TestUpload([FromForm] FileUploadDto file)
        //{
        //    if (file == null)
        //    {
        //        return this.CreateResponse(400, "No file provided");
        //    }

        //    var result = new
        //    {
        //        fileName = file.FileName,
        //        contentType = file.ContentType,
        //        fileSize = file.Length
        //    };

        //    return this.CreateResponse(200, "File received successfully", result);
        //}

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
        [HttpPost("upload-images/{roomId}")]
        [AllowAnonymous] // Temporarily allow anonymous for debugging
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadRoomImages(Guid roomId, [FromForm] RoomImageUploadDto files)
        {
            _logger.LogInformation("=== UPLOAD ENDPOINT HIT ===");
            _logger.LogInformation("UploadRoomImages called for room {RoomId} with {FileCount} files", roomId, files.Files?.Count ?? 0);

            if (files == null || files.Files.Count == 0)
            {
                _logger.LogWarning("No files provided for room {RoomId}", roomId);
                return this.CreateResponse(400, "No files provided");
            }

            // Filter out null or empty files
            var validFiles = files.Files.Where(f => f != null && f.Length > 0).ToList();

            if (validFiles.Count == 0)
            {
                _logger.LogWarning("No valid files provided for room {RoomId}", roomId);
                return this.CreateResponse(400, "No valid files provided");
            }

            _logger.LogInformation("Processing {ValidFileCount} valid files for room {RoomId}", validFiles.Count, roomId);

            // Create a custom IFormFileCollection implementation
            var fileCollection = new CustomFormFileCollection(validFiles);
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
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadRoomImage(Guid roomId, [FromForm] RoomImageDto dto)
        {
            var file = dto.File;

            if (file == null || file.Length == 0)
            {
                return this.CreateResponse(400, "No file provided");
            }

            var fileCollection = new CustomFormFileCollection(new List<IFormFile> { file });
            var result = await _bookingSystemFacade.UploadRoomImages(roomId, fileCollection);
            return this.CreateResponse(result);
        }


        /// <summary>
        /// Test endpoint to verify controller is working.
        /// </summary>
        [HttpGet("test")]
        [AllowAnonymous]
        public IActionResult Test()
        {
            _logger.LogInformation("Test endpoint hit");
            return this.CreateResponse(200, "RoomController is working", new { timestamp = DateTime.UtcNow });
        }

        /// <summary>
        /// Test POST endpoint to verify POST routing is working.
        /// </summary>
        [HttpPost("test-post")]
        [AllowAnonymous]
        public IActionResult TestPost()
        {
            _logger.LogInformation("Test POST endpoint hit");
            return this.CreateResponse(200, "POST routing is working", new { timestamp = DateTime.UtcNow });
        }
    }

    /// <summary>
    /// Custom implementation of IFormFileCollection for handling file uploads.
    /// </summary>
    public class CustomFormFileCollection : IFormFileCollection
    {
        private readonly List<IFormFile> _files;

        public CustomFormFileCollection(List<IFormFile> files)
        {
            _files = files ?? new List<IFormFile>();
        }

        public IFormFile? this[string name] => _files.FirstOrDefault(f => f.Name == name);

        public IFormFile? this[int index] => index >= 0 && index < _files.Count ? _files[index] : null;

        public int Count => _files.Count;

        public IEnumerator<IFormFile> GetEnumerator() => _files.GetEnumerator();

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();

        public IFormFile? GetFile(string name) => this[name];

        public IReadOnlyList<IFormFile> GetFiles(string name) => _files.Where(f => f.Name == name).ToList();
    }
} 