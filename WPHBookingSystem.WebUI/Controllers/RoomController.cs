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
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> CreateRoom(CreateRoomDto dto)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for room creation request");
                return this.CreateResponse(400, "Invalid request data");
            }

            _logger.LogInformation("Room creation attempt for room {Name}", dto.Name);
            
                var roomId = await _bookingSystemFacade.CreateRoom(dto);
                if (!roomId.IsSuccess)
                {
                    _logger.LogWarning("Room creation failed for room {Name}", dto.Name);
                    return this.CreateResponse(400, "Failed to create room");
                }
                _logger.LogInformation("Room created successfully with ID {RoomId}", roomId);
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
        [HttpPut("{roomId}")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> UpdateRoom(Guid roomId, UpdateRoomDto dto)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for room update request");
                return this.CreateResponse(400, "Invalid request data");
            }

            _logger.LogInformation("Room update attempt for room {RoomId}", roomId);
            
                var room = await _bookingSystemFacade.UpdateRoom(roomId, dto);
                if (!room.IsSuccess)
                {
                    _logger.LogWarning("Room not found for update attempt with ID {RoomId}", roomId);
                    return this.CreateResponse(404, "Room not found");
                }
                _logger.LogInformation("Room updated successfully for room {RoomId}", roomId);
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
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> UpdateRoomStatus([FromBody] UpdateRoomStatusRequest request)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for room status update request");
                return this.CreateResponse(400, "Invalid request data");
            }

            _logger.LogInformation("Room status update attempt for room {RoomId} to status {Status}", request.RoomId, request.NewStatus );
            
                var room = await _bookingSystemFacade.UpdateRoomStatus(request);
                if (!room.IsSuccess)
                {
                    _logger.LogWarning("Room not found or invalid status update for room {RoomId}", request.RoomId);
                    return this.CreateResponse(404, "Room not found or invalid status update");
                }
                _logger.LogInformation("Room status updated successfully for room {RoomId} to status {Status}", request.RoomId, request.NewStatus );
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
            _logger.LogInformation("Room retrieval attempt for room {RoomId}", roomId);
            
                var room = await _bookingSystemFacade.GetRoomById(roomId);
                if (!room.IsSuccess)
                {
                    _logger.LogWarning("Room not found for ID {RoomId}", roomId);
                    return this.CreateResponse(404, "Room not found");
                }
                _logger.LogInformation("Room details retrieved successfully for room {RoomId}", roomId);
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
            _logger.LogInformation("Get all rooms attempt");
                var rooms = await _bookingSystemFacade.GetAllRooms();
                if (rooms == null || !rooms.IsSuccess)
                {
                    _logger.LogWarning("No rooms found or retrieval failed");
                    return this.CreateResponse(404, "No rooms found");
                }
                _logger.LogInformation("All rooms retrieved successfully, count: {Count}", rooms?.Data?.Count ?? 0);
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
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> DeleteRoom(Guid roomId)
        {
            _logger.LogInformation("Room deletion attempt for room {RoomId}", roomId);
                var result = await _bookingSystemFacade.DeleteRoom(roomId);
                if (!result.IsSuccess)
                {
                    _logger.LogWarning("Room deletion failed for room {RoomId}", roomId);
                    return this.CreateResponse(404, "Room not found or deletion failed");
                }
                _logger.LogInformation("Room deleted successfully for room {RoomId}, result: {Result}", roomId, result);
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
        public async Task<IActionResult> CheckRoomAvailability([FromQuery] CheckRoomAvailabilityRequest request)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for room availability check request");
                return this.CreateResponse(400, "Invalid request data");
            }
            try
            {
                _logger.LogInformation("Room availability check attempt for room {RoomId}", request.RoomId);
                var isAvailable = await _bookingSystemFacade.CheckRoomAvailability(request);
                if (!isAvailable.IsSuccess)
                {
                    _logger.LogWarning("Room availability check failed for room {RoomId}", request.RoomId);
                    return this.CreateResponse(400, "Room not found or availability check failed");
                }
                _logger.LogInformation("Room availability check completed for room {RoomId}, available: {IsAvailable}", request.RoomId, isAvailable);
                return this.CreateResponse(200, "Availability check completed", isAvailable);
            }
            catch (Exception ex)
            {
                return this.CreateResponse(400, $"Failed to check availability : {ex.Message}");
            }


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
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> GetRoomOccupancyRate([FromBody] GetRoomOccupancyRateRequest request)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for room occupancy rate request");
                return this.CreateResponse(400, "Invalid request data");
            }

            _logger.LogInformation("Room occupancy rate calculation attempt for room {RoomId}", request.RoomId);
                var occupancyRate = await _bookingSystemFacade.GetRoomOccupancyRate(request);
                if (!occupancyRate.IsSuccess)
                {
                    _logger.LogWarning("Room occupancy rate calculation failed for room {RoomId}", request.RoomId);
                    return this.CreateResponse(400, "Room not found or occupancy rate calculation failed");
                }
                _logger.LogInformation("Room occupancy rate calculated successfully for room {RoomId}, rate: {OccupancyRate}", request.RoomId, occupancyRate);
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
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> GetRoomRevenue([FromBody] GetRoomRevenueRequest request)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for room revenue request");
                return this.CreateResponse(400, "Invalid request data");
            }

            _logger.LogInformation("Room revenue calculation attempt for room {RoomId}", request.RoomId);
                var revenue = await _bookingSystemFacade.GetRoomRevenue(request);
                if (!revenue.IsSuccess)
                {
                    _logger.LogWarning("Room revenue calculation failed for room {RoomId}", request.RoomId);
                    return this.CreateResponse(400, "Room not found or revenue calculation failed");
                }
                _logger.LogInformation("Room revenue calculated successfully for room {RoomId}, revenue: {Revenue}", request.RoomId, revenue);
                return this.CreateResponse(200, "Revenue calculated successfully", revenue);
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
                if (!result.IsSuccess)
                {
                    _logger.LogWarning("Room images upload failed for room {RoomId}", roomId);
                    return this.CreateResponse(400, "Failed to upload room images");
                }
                _logger.LogInformation("Room images uploaded successfully for room {RoomId}", roomId);
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
        [Authorize(Roles = "Administrator")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadRoomImage(Guid roomId, [FromForm] RoomImageDto dto)
        {
                var file = dto.File;

                if (file == null || file.Length == 0)
                {
                    _logger.LogWarning("No file provided for room {RoomId}", roomId);
                    return this.CreateResponse(400, "No file provided");
                }

                _logger.LogInformation("Single room image upload attempt for room {RoomId}", roomId);

                var fileCollection = new CustomFormFileCollection(new List<IFormFile> { file });
                var result = await _bookingSystemFacade.UploadRoomImages(roomId, fileCollection);
                if (!result.IsSuccess)
                {
                    _logger.LogWarning("Room image upload failed for room {RoomId}", roomId);
                    return this.CreateResponse(400, "Failed to upload room image");
                }
                _logger.LogInformation("Single room image uploaded successfully for room {RoomId}", roomId);
                return this.CreateResponse(result);

        }

        /// <summary>
        /// Creates a new room in the hotel inventory with images.
        /// 
        /// Admin-only endpoint that adds a new room with specified details and uploads
        /// associated images in a single operation.
        /// </summary>
        /// <param name="dto">Room creation data including type, capacity, pricing, and images</param>
        /// <returns>Unique identifier of the created room</returns>
        /// <response code="200">Room created successfully with images</response>
        /// <response code="400">Invalid room data or image upload failed</response>
        /// <response code="401">User not authenticated</response>
        /// <response code="403">User not authorized (admin role required)</response>
        [HttpPost("with-images")]
        [Authorize(Roles = "Administrator")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> CreateRoomWithImages([FromForm] CreateRoomWithImagesDto dto)
        {
                _logger.LogInformation("Creating room with {ImageCount} images", dto.Images?.Count ?? 0);

                // Convert to original DTO
                var createRoomDto = new CreateRoomDto
                {
                    Name = dto.Name,
                    Description = dto.Description,
                    Price = dto.Price,
                    Capacity = dto.Capacity
                };

                // Convert images to IFormFileCollection
                IFormFileCollection? imageCollection = null;
                if (dto.Images?.Any() == true)
                {
                    var validImages = dto.Images.Where(f => f != null && f.Length > 0).ToList();
                    if (validImages.Any())
                    {
                        imageCollection = new CustomFormFileCollection(validImages);
                    }
                }

                var roomId = await _bookingSystemFacade.CreateRoom(createRoomDto, imageCollection);
                if (!roomId.IsSuccess)
                {
                    _logger.LogWarning("Room creation failed with images, room name: {RoomName}", dto.Name);
                    return this.CreateResponse(400, "Failed to create room with images");
                }
                _logger.LogInformation("Room created successfully with images, room ID: {RoomId}", roomId);
                return this.CreateResponse(200, "Room created successfully with images", roomId);

        }

        /// <summary>
        /// Test endpoint to verify controller is working.
        /// </summary>
        [HttpGet("test")]
        [AllowAnonymous]
        public IActionResult Test()
        {
            _logger.LogInformation("Test endpoint called");
            return this.CreateResponse(200, "Test endpoint working", new { message = "CORS test successful", timestamp = DateTime.UtcNow });
        }

        /// <summary>
        /// Test authentication endpoint.
        /// </summary>
        [HttpGet("test-auth")]
        [Authorize]
        public IActionResult TestAuth()
        {
            return this.CreateResponse(200, "Authentication is working", new { timestamp = DateTime.UtcNow });
        }

        /// <summary>
        /// Test authorization endpoint for Administrator role.
        /// </summary>
        [HttpGet("test-admin")]
        [Authorize(Roles = "Administrator")]
        public IActionResult TestAdmin()
        {
            return this.CreateResponse(200, "Admin authorization is working", new { timestamp = DateTime.UtcNow });
        }

        /// <summary>
        /// Test POST endpoint to verify POST routing is working.
        /// </summary>
        [HttpPost("test-post")]
        [AllowAnonymous]
        public IActionResult TestPost()
        {
            return this.CreateResponse(200, "POST routing is working", new { timestamp = DateTime.UtcNow });
        }

        /// <summary>
        /// Updates room details with new images.
        /// 
        /// Admin-only endpoint that allows updating room information and managing images
        /// in a single request. Supports two modes:
        /// - Add mode (default): Keeps existing images and adds new ones
        /// - Replace mode: Removes existing images and replaces with new ones
        /// 
        /// Use ReplaceExistingImages=true to replace all existing images with new ones.
        /// Use ReplaceExistingImages=false (or omit) to add new images to existing ones.
        /// </summary>
        /// <param name="roomId">Unique identifier of the room to update</param>
        /// <param name="dto">Room update data with optional new images and replacement flag</param>
        /// <returns>Updated room details with new images</returns>
        /// <response code="200">Room updated successfully with new images</response>
        /// <response code="400">Invalid room data or images</response>
        /// <response code="404">Room not found</response>
        /// <response code="403">User not authorized (admin role required)</response>
        [HttpPut("{roomId}/with-images")]
        [Authorize(Roles = "Administrator")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UpdateRoomWithImages(Guid roomId, [FromForm] UpdateRoomWithImagesDto dto)
        {
            try
            {
                _logger.LogInformation("Room update with images attempt for room {RoomId}", roomId);
                
                var room = await _bookingSystemFacade.UpdateRoomWithImages(roomId, dto);
                _logger.LogInformation("Room updated successfully with images for room {RoomId}", roomId);
                return this.CreateResponse(200, "Room updated successfully with new images", room);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating room with images for room {RoomId}", roomId);
                return this.CreateResponse(500, "An error occurred while updating room with images");
            }
        }

        [HttpGet("cors-test")]
        [AllowAnonymous]
        public IActionResult CorsTest()
        {
            _logger.LogInformation("CORS test endpoint called");
            return this.CreateResponse(200, "CORS test successful", new { 
                message = "CORS is working", 
                timestamp = DateTime.UtcNow,
                origin = Request.Headers["Origin"].ToString(),
                method = Request.Method,
                path = Request.Path
            });
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
            _files = files;
        }

        public IFormFile this[string name] => _files.FirstOrDefault(f => f.Name == name) ?? throw new InvalidOperationException($"File '{name}' not found.");

        public int Count => _files.Count;

        public IFormFile this[int index] => throw new NotImplementedException();

        public IEnumerator<IFormFile> GetEnumerator() => _files.GetEnumerator();

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();

        public IFormFile? GetFile(string name) => this[name];

        public IReadOnlyList<IFormFile> GetFiles(string name) => _files.Where(f => f.Name == name).ToList();
    }
} 