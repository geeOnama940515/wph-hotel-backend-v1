using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPHBookingSystem.Application.DTOs.Booking
{
    public class UpdateBookingDatesDto
    {
        public Guid BookingId { get; set; }
        public DateTime NewCheckIn { get; set; }
        public DateTime NewCheckOut { get; set; }
    }
}
