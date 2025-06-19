using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPHBookingSystem.Domain.Enums
{
    /// <summary>
    /// Represents the current status of a booking in the hotel booking system.
    /// This enum defines the various states a booking can be in throughout its lifecycle,
    /// from initial creation to completion or cancellation.
    /// </summary>
    public enum BookingStatus
    {
        Pending,
        EmailVerificationPending,
        Confirmed,
        Cancelled,
        CheckedIn,
        CheckedOut,
        Completed
    }
}
