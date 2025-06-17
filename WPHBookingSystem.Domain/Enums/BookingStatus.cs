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
        /// <summary>
        /// The booking has been created but not yet confirmed by the hotel staff.
        /// This is the initial state for new bookings and requires manual review.
        /// </summary>
        Pending,

        /// <summary>
        /// The booking has been reviewed and confirmed by hotel staff.
        /// The room is guaranteed to be available for the guest during the specified dates.
        /// </summary>
        Confirmed,

        /// <summary>
        /// The booking has been cancelled either by the guest or hotel staff.
        /// Cancelled bookings cannot be reactivated and require a new booking to be created.
        /// </summary>
        Cancelled,

        /// <summary>
        /// The guest has checked in and is currently occupying the room.
        /// This status is set when the guest arrives and completes the check-in process.
        /// </summary>
        CheckedIn,

        /// <summary>
        /// The guest has checked out and the room is now available for cleaning.
        /// This status is set when the guest completes the check-out process.
        /// </summary>
        CheckedOut,

        /// <summary>
        /// The booking has been made and confirmed, but the guest has not yet checked in.
        /// This is an intermediate state between Confirmed and CheckedIn.
        /// </summary>
        Booked,

        /// <summary>
        /// The booking has been fully completed - the guest has checked in and checked out.
        /// This is the final state for successful bookings and indicates the entire stay is complete.
        /// </summary>
        Completed
    }
}
