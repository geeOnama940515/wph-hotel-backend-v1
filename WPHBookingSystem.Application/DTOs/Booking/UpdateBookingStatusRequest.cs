using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPHBookingSystem.Domain.Enums;

namespace WPHBookingSystem.Application.DTOs.Booking
{
    public class UpdateBookingStatusRequest
    {
        public Guid BookingId { get; set; }
        public BookingStatus NewStatus { get; set; }
    }
}
