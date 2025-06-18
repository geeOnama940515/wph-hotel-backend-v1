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

        Available,
        Booked,
        Occupied,
        Maintenance,
        Inactive
    }
}
