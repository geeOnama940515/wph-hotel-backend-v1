using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPHBookingSystem.Domain.Enums
{
    /// <summary>
    /// Represents the current status of a room in the hotel.
    /// This enum defines the various operational states a room can be in,
    /// which determines whether it can be booked or occupied by guests.
    /// </summary>
    public enum RoomStatus
    {
        /// <summary>
        /// The room is available for booking and can be reserved by guests.
        /// This is the default state for rooms that are ready for occupancy.
        /// </summary>
        Available,

        /// <summary>
        /// The room has been booked by a guest for specific dates.
        /// The room is reserved and cannot be booked by other guests during the reserved period.
        /// </summary>
        Booked,

        /// <summary>
        /// The room is currently occupied by a guest who has checked in.
        /// The room is not available for new bookings until the guest checks out.
        /// </summary>
        Occupied,

        /// <summary>
        /// The room is under maintenance or repair and cannot be booked.
        /// This status is used when the room requires repairs, cleaning, or other maintenance work.
        /// </summary>
        Maintenance,

        /// <summary>
        /// The room is inactive and not available for booking.
        /// This status is used for rooms that are temporarily or permanently taken out of service.
        /// </summary>
        Inactive
    }
}
