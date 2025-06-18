using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

        /// <summary>
        /// Initializes a new instance of the BookingController with the booking system facade.
        /// </summary>
        /// <param name="facade">Service facade for coordinating booking operations</param>
        /// <param name="emailService">Service for sending emails</param>
        public BookingController(IBookingSystemFacade facade, IEmailService emailService)
        {
            _facade = facade;
            _emailService = emailService;
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
        [HttpPost]
        public async Task<IActionResult> CreateBooking(CreateBookingDto dto)
        {
            var result = await _facade.CreateBooking(dto);
            return this.CreateResponse(result);
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
            var result = await _facade.UpdateBooking(bookingId, dto);
            return this.CreateResponse(result);
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
            var result = await _facade.UpdateBookingStatus(request);
            return this.CreateResponse(result);
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
            var result = await _facade.CancelBooking(bookingId);
            return this.CreateResponse(result);
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
        [HttpGet("{emailAddres}/get-bookings")]
        public async Task<IActionResult> GetUserBookings(string emailAddres)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return this.CreateResponse(401, "User not authenticated");

            var result = await _facade.GetUserBookings(emailAddres);
            return this.CreateResponse(result);
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
            var result = await _facade.ViewBookingByToken(bookingToken);
            return this.CreateResponse(result);
        }

        /// <summary>
        /// Test endpoint for email service.
        /// </summary>
        [HttpPost("test-email")]
        [AllowAnonymous]
        public async Task<IActionResult> TestEmail([FromBody] TestEmailRequest request)
        {
            try
            {
                var bookingDto = new BookingDto
                {
                    Id = Guid.NewGuid(),
                    UserId = Guid.NewGuid(),
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
                    return this.CreateResponse(200, "Test email sent successfully");
                }
                else
                {
                    return this.CreateResponse(500, "Failed to send test email");
                }
            }
            catch (Exception ex)
            {
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