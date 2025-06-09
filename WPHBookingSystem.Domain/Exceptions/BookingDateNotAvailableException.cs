using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPHBookingSystem.Domain.Exceptions
{
    public class BookingDateNotAvailableException : Exception
    {
        public BookingDateNotAvailableException()
            : base("The selected booking dates are not available.")
        {
        }

        public BookingDateNotAvailableException(string message)
            : base(message)
        {
        }
    }
}
