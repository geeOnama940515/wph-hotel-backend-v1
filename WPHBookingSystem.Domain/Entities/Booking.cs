using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPHBookingSystem.Domain.Entities.Common;
using WPHBookingSystem.Domain.Enums;

namespace WPHBookingSystem.Domain.Entities
{
    public class Booking : BaseAuditable
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }  // Just the reference to user

        public Guid RoomId { get; set; }
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
        public int Guests { get; set; }
        public decimal TotalAmount { get; set; }
        public BookingStatus Status { get; set; }
        public string SpecialRequests { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }

        // Navigation property to Room (since Room is domain entity)
        public virtual Room Room { get; set; }
    }
}
