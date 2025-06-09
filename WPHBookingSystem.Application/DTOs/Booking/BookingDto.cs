using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPHBookingSystem.Domain.Enums;

namespace WPHBookingSystem.Application.DTOs.Booking
{
    public class BookingDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid RoomId { get; set; }

        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }

        public int Guests { get; set; }
        public decimal TotalAmount { get; set; }

        public BookingStatus Status { get; set; }

        public string SpecialRequests { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;

        public string? RoomName { get; set; } // optional projection
    }
}
