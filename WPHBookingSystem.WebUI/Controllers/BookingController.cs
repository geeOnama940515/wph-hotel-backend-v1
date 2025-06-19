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
    [Route("api/bookings")]
    public class BookingController : ControllerBase
    {
        private readonly IBookingSystemFacade _facade;
        private readonly IEmailSenderService _emailService;
        private readonly ILogger<BookingController> _logger;

        /// <summary>
        /// Initializes a new instance of the BookingController with the booking system facade.
        /// </summary>
        /// <param name="facade">Service facade for coordinating booking operations</param>
        /// <param name="emailService">Service for sending emails</param>
        /// <param name="logger">Logger for booking operations</param>
        public BookingController(IBookingSystemFacade facade, IEmailSenderService emailService, ILogger<BookingController> logger)
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
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Invalid model state for booking creation request");
                    return this.CreateResponse(400, "Invalid request data");
                }

                _logger.LogInformation("Booking creation attempt for room {RoomId}", dto.RoomId);           
                var result = await _facade.CreateBooking(dto);
                if (!result.IsSuccess)
                {
                    _logger.LogWarning("Failed to create booking for room {RoomId}: {Message}", dto.RoomId, result.Message);
                    return this.CreateResponse(result.StatusCode, result.Message);
                }
                _logger.LogInformation("Booking created successfully for room {RoomId}", dto.RoomId);
                return this.CreateResponse(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while creating booking for room {RoomId}: {ErrorMessage}", dto.RoomId, ex.Message);
                return this.CreateResponse(500, $"Booking creation failed: {ex.Message}");
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
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Invalid model state for booking dates update request");
                    return this.CreateResponse(400, "Invalid request data");
                }

                _logger.LogInformation("Booking dates update attempt for booking {BookingId}", bookingId);
                

                var result = await _facade.UpdateBooking(bookingId, dto);
                if (!result.IsSuccess)
                {
                    _logger.LogWarning("Failed to update booking dates for booking {BookingId}: {Message}", bookingId, result.Message);
                    return this.CreateResponse(result.StatusCode, result.Message);
                }
                _logger.LogInformation("Booking dates updated successfully for booking {BookingId}", bookingId);
                return this.CreateResponse(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while updating booking dates for booking {BookingId}: {ErrorMessage}", bookingId, ex.Message);
                return this.CreateResponse(500, $"Booking dates update failed: {ex.Message}");
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
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Invalid model state for booking status update request");
                    return this.CreateResponse(400, "Invalid request data");
                }

                _logger.LogInformation("Booking status update attempt for booking {BookingId} to status {Status}", bookingId, request.NewStatus);
                
                var result = await _facade.UpdateBookingStatus(request);
                if (!result.IsSuccess)
                {
                    _logger.LogWarning("Failed to update booking status for booking {BookingId}: {Message}", bookingId, result.Message);
                    return this.CreateResponse(result.StatusCode, result.Message);
                }
                _logger.LogInformation("Booking status updated successfully for booking {BookingId} to status {Status}", bookingId, request.NewStatus);
                return this.CreateResponse(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while updating booking status for booking {BookingId}: {ErrorMessage}", bookingId, ex.Message);
                return this.CreateResponse(500, $"Booking status update failed: {ex.Message}");
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
            try
            {
                _logger.LogInformation("Booking cancellation attempt for booking {BookingId}", bookingId);
                
                var result = await _facade.CancelBooking(bookingId);
                if (!result.IsSuccess)
                {
                    _logger.LogWarning("Failed to cancel booking {BookingId}: {Message}", bookingId, result.Message);
                    return this.CreateResponse(result.StatusCode, result.Message);
                }
                _logger.LogInformation("Booking cancelled successfully for booking {BookingId}", bookingId);
                return this.CreateResponse(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while cancelling booking {BookingId}: {ErrorMessage}", bookingId, ex.Message);
                return this.CreateResponse(500, $"Booking cancellation failed: {ex.Message}");
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
        /// <response code="200">User bookings retrieved successfully</response>
        /// <response code="400">Invalid email address</response>
        /// <response code="401">User not authenticated</response>
        [AllowAnonymous]
        [HttpGet("{emailAddres}/get-bookings")]
        public async Task<IActionResult> GetUserBookings(string emailAddres)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(emailAddres))
                {
                    _logger.LogWarning("Empty or null email address provided for booking retrieval");
                    return this.CreateResponse(400, "Email address is required");
                }

                _logger.LogInformation("Retrieving bookings for user with email {EmailAddress}", emailAddres);
                
                var result = await _facade.GetUserBookings(emailAddres);
                if (!result.IsSuccess)
                {
                    _logger.LogWarning("Failed to retrieve bookings for email {EmailAddress}: {Message}", emailAddres, result.Message);
                    return this.CreateResponse(result.StatusCode, result.Message);
                }
                _logger.LogInformation("Successfully retrieved bookings for email {EmailAddress}", emailAddres);
                return this.CreateResponse(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while retrieving bookings for email {EmailAddress}: {ErrorMessage}", emailAddres, ex.Message);
                return this.CreateResponse(500, $"Failed to retrieve bookings: {ex.Message}");
            }
        }

        /// <summary>
        /// Retrieves all bookings in the system.
        /// 
        /// Returns a list of all bookings including current, past, and cancelled bookings.
        /// This endpoint provides administrative access to all booking data for management purposes.
        /// </summary>
        /// <returns>Standardized response with list of all bookings</returns>
        /// <response code="200">All bookings retrieved successfully</response>
        /// <response code="401">User not authenticated</response>
        /// <response code="403">User not authorized for administrative access</response>
        [HttpGet]
        public async Task<IActionResult> GetAllBookings()
        {
            try
            {
                _logger.LogInformation("Retrieving all bookings in the system");
                var result = await _facade.GetAllBookings();
                if (!result.IsSuccess)
                {
                    _logger.LogWarning("Failed to retrieve all bookings: {Message}", result.Message);
                    return this.CreateResponse(result.StatusCode, result.Message);
                }
                _logger.LogInformation("Successfully retrieved {Count} bookings from the system", 
                    result.Data?.Count ?? 0);
                return this.CreateResponse(200,"Bookings Retrieved",result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while retrieving all bookings: {ErrorMessage}", ex.Message);
                return this.CreateResponse(500, $"Failed to retrieve bookings: {ex.Message}");
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
            try
            {
                _logger.LogInformation("View booking by token attempt for token {BookingToken}", bookingToken);         
                var result = await _facade.ViewBookingByToken(bookingToken);
                if (result == null)
                {
                    _logger.LogWarning("Failed to retrieve booking details for token {BookingToken}: Booking not found", bookingToken);
                    return this.CreateResponse(404, "Booking not found");
                }
                if (!result.IsSuccess)
                {
                    _logger.LogWarning("Failed to retrieve booking details for token {BookingToken}: {Message}", bookingToken, result.Message);
                    return this.CreateResponse(result.StatusCode, result.Message);
                }
                _logger.LogInformation("Booking details retrieved successfully for token {BookingToken}", bookingToken);
                return this.CreateResponse(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while viewing booking by token {BookingToken}: {ErrorMessage}", bookingToken, ex.Message);
                return this.CreateResponse(500, $"Failed to retrieve booking details: {ex.Message}");
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
                    Address = "123 Test Street",
                    BookingToken = "ce4de5b2-f0a8-4207-aa26-09ac33102f6d",
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

        /// <summary>
        /// Debug endpoint to test booking creation without email.
        /// </summary>
        [HttpPost("debug-create")]
        [AllowAnonymous]
        public async Task<IActionResult> DebugCreateBooking([FromBody] CreateBookingDto dto)
        {
            try
            {
                _logger.LogInformation("Debug booking creation attempt for room {RoomId}", dto.RoomId);
                
                // Test if the facade is working
                var result = await _facade.CreateBooking(dto);
                
                _logger.LogInformation("Debug booking creation result: Success={IsSuccess}, Message={Message}", 
                    result.IsSuccess, result.Message);
                
                return this.CreateResponse(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Debug booking creation failed for room {RoomId}: {ErrorMessage}", dto.RoomId, ex.Message);
                return this.CreateResponse(500, $"Debug booking creation failed: {ex.Message}");
            }
        }
    }

    public class TestEmailRequest
    {
        public string Email { get; set; } = string.Empty;
        public string GuestName { get; set; } = string.Empty;
    }
} 