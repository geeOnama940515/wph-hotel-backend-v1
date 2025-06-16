using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using WPHBookingSystem.Application.Common;
using WPHBookingSystem.Application.DTOs.Booking;
using WPHBookingSystem.Application.Interfaces.Services;
using WPHBookingSystem.WebUI.Extensions;

namespace WPHBookingSystem.WebUI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class BookingController : ControllerBase
    {
        private readonly IBookingSystemFacade _facade;

        public BookingController(IBookingSystemFacade facade)
        {
            _facade = facade;
        }

        [HttpPost]
        public async Task<IActionResult> CreateBooking(CreateBookingDto dto)
        {
            var result = await _facade.CreateBooking(dto);
            return this.CreateResponse(result);
        }

        [HttpPut("{bookingId}/dates")]
        public async Task<IActionResult> UpdateBookingDates(Guid bookingId, UpdateBookingDateDto dto)
        {
            var result = await _facade.UpdateBooking(bookingId, dto);
            return this.CreateResponse(result);
        }

        [HttpPut("{bookingId}/status")]
        public async Task<IActionResult> UpdateBookingStatus(Guid bookingId, UpdateBookingStatusRequest request)
        {
            var result = await _facade.UpdateBookingStatus(request);
            return this.CreateResponse(result);
        }

        [HttpPut("{bookingId}/cancel")]
        public async Task<IActionResult> CancelBooking(Guid bookingId)
        {
            var result = await _facade.CancelBooking(bookingId);
            return this.CreateResponse(result);
        }

        [HttpGet("{emailAddres}/get-bookings")]
        public async Task<IActionResult> GetUserBookings(string emailAddres)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return this.CreateResponse(401, "User not authenticated");

            var result = await _facade.GetUserBookings(emailAddres);
            return this.CreateResponse(result);
        }

        [HttpGet("view/{bookingToken}")]
        public async Task<IActionResult> ViewBookingByToken(Guid bookingToken)
        {
            var result = await _facade.ViewBookingByToken(bookingToken);
            return this.CreateResponse(result);
        }
    }
} 