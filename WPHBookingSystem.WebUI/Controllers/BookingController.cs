using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using WPHBookingSystem.Application.Common;
using WPHBookingSystem.Application.DTOs.Booking;
using WPHBookingSystem.Application.Interfaces.Services;
using WPHBookingSystem.Domain.Enums;
using WPHBookingSystem.WebUI.Extensions;

namespace WPHBookingSystem.WebUI.Controllers
{
    /// <summary>
    /// Booking controller providing hotel booking management endpoints.
    /// 
    /// This controller handles all booking-related operations including creation, updates,
    /// cancellation, and retrieval. It requires authentication for most operations and
    /// uses the BookingSystemFacade to coordinate business logic across multiple use cases.
    /// 
    /// All endpoints (except view by token) require valid JWT authentication and
    /// use standardized response formats through the ControllerExtensions.
    /// </summary>
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class BookingController : ControllerBase
    {
        private readonly IBookingSystemFacade _facade;
        private readonly IEmailService _emailService;
        private readonly ILogger<BookingController> _logger;

        /// <summary>
        /// Initializes a new instance of the BookingController with the booking system facade.
        /// </summary>
        /// <param name="facade">Service facade for coordinating booking operations</param>
        /// <param name="emailService">Service for sending emails</param>
        /// <param name="logger">Logger for booking operations</param>
        public BookingController(IBookingSystemFacade facade, IEmailService emailService, ILogger<BookingController> logger)
        {
            _facade = facade;
            _emailService = emailService;
            _logger = logger;
        }

        /// <summary>
        /// Creates a new hotel booking for the authenticated user.
        /// 
        /// Validates room availability, user authentication, and booking constraints
        /// before creating the booking. Returns booking details upon successful creation.
        /// </summary>
        /// <param name="dto">Booking creation data including room, dates, and guest information</param>
        /// <returns>Standardized response with booking details or error information</returns>
        /// <response code="201">Booking created successfully</response>
        /// <response code="400">Invalid booking data or room not available</response>
        /// <response code="401">User not authenticated</response>
        /// 

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> CreateBooking(CreateBookingDto dto)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for booking creation request");
                return this.CreateResponse(400, "Invalid request data");
            }

            _logger.LogInformation("Booking creation attempt for room {RoomId}", dto.RoomId);
            
            try
            {
                var result = await _facade.CreateBooking(dto);
                _logger.LogInformation("Booking created successfully for room {RoomId}", dto.RoomId);
                return this.CreateResponse(result);
            }
            catch (ApplicationException ex)
            {
                // For domain-related exceptions like "Room not available"
                _logger.LogWarning("Application exception during booking creation: {Message}", ex.Message);
                return this.CreateResponse(400, ex.Message);
            }
            catch (Exception ex)
            {
                // For unexpected errors
                _logger.LogError(ex, "Unexpected error occurred while creating a booking for room {RoomId}", dto.RoomId);
                return this.CreateResponse(500, "An unexpected error occurred.");
            }
        }


        /// <summary>
        /// Updates the check-in and check-out dates for an existing booking.
        /// 
        /// Validates date availability and booking ownership before allowing updates.
        /// Only the booking owner can modify their booking dates.
        /// </summary>
        /// <param name="bookingId">Unique identifier of the booking to update</param>
        /// <param name="dto">New check-in and check-out dates</param>
        /// <returns>Standardized response with updated booking details</returns>
        /// <response code="200">Booking dates updated successfully</response>
        /// <response code="400">Invalid dates or booking not found</response>
        /// <response code="403">User not authorized to modify this booking</response>
        [HttpPut("{bookingId}/dates")]
        public async Task<IActionResult> UpdateBookingDates(Guid bookingId, UpdateBookingDateDto dto)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for booking dates update request");
                return this.CreateResponse(400, "Invalid request data");
            }

            _logger.LogInformation("Booking dates update attempt for booking {BookingId}", bookingId);
            
            try
            {
                var result = await _facade.UpdateBooking(bookingId, dto);
                _logger.LogInformation("Booking dates updated successfully for booking {BookingId}", bookingId);
                return this.CreateResponse(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating booking dates for booking {BookingId}", bookingId);
                return this.CreateResponse(500, $"An error occurred while updating booking dates: {ex.Message}");
            }
        }

        /// <summary>
        /// Updates the status of a booking (e.g., confirmed, cancelled, completed).
        /// 
        /// Allows administrative status changes and user-initiated cancellations.
        /// Status changes are validated against business rules and booking state.
        /// </summary>
        /// <param name="bookingId">Unique identifier of the booking</param>
        /// <param name="request">New status and optional reason for the change</param>
        /// <returns>Standardized response with updated booking information</returns>
        /// <response code="200">Booking status updated successfully</response>
        /// <response code="400">Invalid status or booking not found</response>
        [HttpPut("{bookingId}/status")]
        public async Task<IActionResult> UpdateBookingStatus(Guid bookingId, UpdateBookingStatusRequest request)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for booking status update request");
                return this.CreateResponse(400, "Invalid request data");
            }

            _logger.LogInformation("Booking status update attempt for booking {BookingId} to status {Status}", bookingId, request.NewStatus);
            
            try
            {
                var result = await _facade.UpdateBookingStatus(request);
                _logger.LogInformation("Booking status updated successfully for booking {BookingId} to status {Status}", bookingId, request.NewStatus);
                return this.CreateResponse(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating booking status for booking {BookingId}", bookingId);
                return this.CreateResponse(500, $"An error occurred while updating booking status: {ex.Message}");
            }
        }

        /// <summary>
        /// Cancels an existing booking.
        /// 
        /// Marks the booking as cancelled and may apply cancellation policies
        /// based on the timing of the cancellation relative to check-in date.
        /// </summary>
        /// <param name="bookingId">Unique identifier of the booking to cancel</param>
        /// <returns>Standardized response confirming cancellation</returns>
        /// <response code="200">Booking cancelled successfully</response>
        /// <response code="400">Booking not found or already cancelled</response>
        /// <response code="403">User not authorized to cancel this booking</response>
        [HttpPut("{bookingId}/cancel")]
        public async Task<IActionResult> CancelBooking(Guid bookingId)
        {
            _logger.LogInformation("Booking cancellation attempt for booking {BookingId}", bookingId);
            
            try
            {
                var result = await _facade.CancelBooking(bookingId);
                _logger.LogInformation("Booking cancelled successfully for booking {BookingId}", bookingId);
                return this.CreateResponse(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cancelling booking {BookingId}", bookingId);
                return this.CreateResponse(500, $"An error occurred while cancelling the booking: {ex.Message}");
            }
        }

        /// <summary>
        /// Retrieves all bookings for a specific user by email address.
        /// 
        /// Returns a list of all bookings associated with the provided email,
        /// including current, past, and cancelled bookings. Requires authentication
        /// and validates user identity from JWT token.
        /// </summary>
        /// <param name="emailAddres">Email address of the user whose bookings to retrieve</param>
        /// <returns>Standardized response with list of user bookings</returns>
        /// <response code="200">Bookings retrieved successfully</response>
        /// <response code="401">User not authenticated</response>
        /// <response code="403">User not authorized to view these bookings</response>
        /// 

        [AllowAnonymous]
        [HttpGet("{emailAddres}/get-bookings")]
        public async Task<IActionResult> GetUserBookings(string emailAddres)
        {
            if (string.IsNullOrEmpty(emailAddres))
            {
                _logger.LogWarning("Get user bookings request with empty email address");
                return this.CreateResponse(400, "Email address is required");
            }

            _logger.LogInformation("Get user bookings attempt for email {Email}", emailAddres);
            
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    _logger.LogWarning("User not authenticated for bookings retrieval");
                    return this.CreateResponse(401, "User not authenticated");
                }

                var result = await _facade.GetUserBookings(emailAddres);
                _logger.LogInformation("User bookings retrieved successfully for email {Email}", emailAddres);
                return this.CreateResponse(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user bookings for email {Email}", emailAddres);
                return this.CreateResponse(500, $"An error occurred while retrieving user bookings: {ex.Message}");
            }
        }

        /// <summary>
        /// Retrieves booking details using a public booking token.
        /// 
        /// This endpoint is publicly accessible and allows viewing booking information
        /// using a unique token, useful for sharing booking details or confirmation emails.
        /// No authentication required as the token serves as the access mechanism.
        /// </summary>
        /// <param name="bookingToken">Unique token associated with the booking</param>
        /// <returns>Standardized response with booking details</returns>
        /// <response code="200">Booking details retrieved successfully</response>
        /// <response code="404">Booking not found or invalid token</response>
        [AllowAnonymous]
        [HttpGet("view/{bookingToken}")]
        public async Task<IActionResult> ViewBookingByToken(Guid bookingToken)
        {
            _logger.LogInformation("View booking by token attempt for token {BookingToken}", bookingToken);
            
            try
            {
                var result = await _facade.ViewBookingByToken(bookingToken);
                _logger.LogInformation("Booking details retrieved successfully for token {BookingToken}", bookingToken);
                return this.CreateResponse(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving booking details for token {BookingToken}", bookingToken);
                return this.CreateResponse(500, $"An error occurred while retrieving booking details: {ex.Message}");
            }
        }

        /// <summary>
        /// Test endpoint for email service.
        /// </summary>
        [HttpPost("test-email")]
        [AllowAnonymous]
        public async Task<IActionResult> TestEmail([FromBody] TestEmailRequest request)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for test email request");
                return this.CreateResponse(400, "Invalid request data");
            }

            _logger.LogInformation("Test email sending attempt for email {Email}", request.Email);
            
            try
            {
                var bookingDto = new BookingDto
                {
                    Id = Guid.NewGuid(),
                    GuestName = "Gregorio Amano",
                    RoomId = Guid.NewGuid(),
                    RoomName = "Test Room",
                    CheckIn = DateTime.Now.AddDays(7),
                    CheckOut = DateTime.Now.AddDays(10),
                    Guests = 2,
                    TotalAmount = 300.00m,
                    Status = Domain.Enums.BookingStatus.Confirmed,
                    SpecialRequests = "Test special request",
                    Phone = "+1234567890",
                    Address = "123 Test Street"
                };

                var result = await _emailService.SendBookingConfirmationAsync(bookingDto, request.Email, request.GuestName);
                
                if (result)
                {
                    _logger.LogInformation("Test email sent successfully to {Email}", request.Email);
                    return this.CreateResponse(200, "Test email sent successfully");
                }
                else
                {
                    _logger.LogWarning("Test email sending failed for {Email}", request.Email);
                    return this.CreateResponse(500, "Failed to send test email");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending test email to {Email}", request.Email);
                return this.CreateResponse(500, $"Error sending test email: {ex.Message}");
            }
        }
    }

    public class TestEmailRequest
    {
        public string Email { get; set; } = string.Empty;
        public string GuestName { get; set; } = string.Empty;
    }
} 