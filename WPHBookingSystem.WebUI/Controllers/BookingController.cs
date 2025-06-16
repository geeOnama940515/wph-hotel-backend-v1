using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WPHBookingSystem.Application.DTOs.Booking;
using WPHBookingSystem.Application.Interfaces.Services;

namespace WPHBookingSystem.WebUI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class BookingController : ControllerBase
    {
        private readonly IBookingSystemFacade _bookingSystemFacade;

        public BookingController(IBookingSystemFacade bookingSystemFacade)
        {
            _bookingSystemFacade = bookingSystemFacade;
        }

        [HttpPost]
        public async Task<ActionResult<Guid>> CreateBooking(CreateBookingDto dto)
        {
            var userId = Guid.Parse(User.FindFirst("sub")?.Value ?? throw new UnauthorizedAccessException());
            var bookingId = await _bookingSystemFacade.CreateBooking(dto, userId);
            return Ok(bookingId);
        }

        [HttpPut("update-dates")]
        public async Task<ActionResult<BookingDto>> UpdateBookingDates(UpdateBookingDateDto dto)
        {
            var booking = await _bookingSystemFacade.UpdateBooking(dto);
            return Ok(booking);
        }

        [HttpPut("update-status")]
        public async Task<ActionResult<BookingDto>> UpdateBookingStatus(UpdateBookingStatusRequest request)
        {
            var booking = await _bookingSystemFacade.UpdateBookingStatus(request);
            return Ok(booking);
        }

        [HttpPut("{bookingId}/cancel")]
        public async Task<ActionResult<BookingDto>> CancelBooking(Guid bookingId)
        {
            var booking = await _bookingSystemFacade.CancelBooking(bookingId);
            return Ok(booking);
        }

        [HttpGet("user")]
        public async Task<ActionResult<IEnumerable<BookingDto>>> GetUserBookings()
        {
            var userId = Guid.Parse(User.FindFirst("sub")?.Value ?? throw new UnauthorizedAccessException());
            var bookings = await _bookingSystemFacade.GetUserBookings(userId);
            return Ok(bookings);
        }
    }
} 