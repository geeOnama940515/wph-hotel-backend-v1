using System;

namespace WPHBookingSystem.Application.DTOs.Booking
{
    public class BookingCreatedDto
    {
        public Guid Id { get; set; }
        public Guid BookingToken { get; set; }
    }
} 