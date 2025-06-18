using WPHBookingSystem.Domain.Entities.Common;
using WPHBookingSystem.Domain.Enums;
using WPHBookingSystem.Domain.Exceptions;
using WPHBookingSystem.Domain.ValueObjects;

namespace WPHBookingSystem.Domain.Entities
{
    /// <summary>
    /// Represents a hotel room in the booking system.
    /// This entity encapsulates all the information and behavior related to a hotel room,
    /// including its availability, bookings, pricing, and operational status.
    /// 
    /// The Room entity follows the Domain-Driven Design pattern with encapsulated business logic
    /// and invariant protection through private setters and validation methods.
    /// </summary>
    public class Room : BaseAuditable
    {
        /// <summary>
        /// Gets the unique identifier for this room.
        /// This ID is automatically generated when the room is created.
        /// </summary>
        public Guid Id { get; private set; }

        /// <summary>
        /// Gets the name or number of the room.
        /// This is typically a human-readable identifier like "Room 101" or "Deluxe Suite A".
        /// </summary>
        public string Name { get; private set; } = string.Empty;

        /// <summary>
        /// Gets the detailed description of the room.
        /// This includes information about amenities, features, and any special characteristics.
        /// </summary>
        public string Description { get; private set; } = string.Empty;

        /// <summary>
        /// Gets the price per night for this room.
        /// This is the base rate before any discounts or additional charges.
        /// </summary>
        public decimal Price { get; private set; }

        /// <summary>
        /// Gets the maximum number of guests that can occupy this room.
        /// This determines the room's capacity for booking purposes.
        /// </summary>
        public int Capacity { get; private set; }

        /// <summary>
        /// Gets the collection of images associated with this room.
        /// These images are used to showcase the room in the booking interface.
        /// </summary>
        public List<GalleryImage> Images { get; private set; } = new();

        /// <summary>
        /// Gets the current operational status of the room.
        /// This determines whether the room can be booked or is unavailable.
        /// </summary>
        public RoomStatus Status { get; private set; } = RoomStatus.Available;

        /// <summary>
        /// Private collection of bookings associated with this room.
        /// This is encapsulated to maintain data integrity and provide controlled access.
        /// </summary>
        private readonly List<Booking> _bookings = new();

        /// <summary>
        /// Gets a read-only collection of all bookings associated with this room.
        /// This provides controlled access to the bookings while preventing external modification.
        /// </summary>
        public IReadOnlyCollection<Booking> Bookings => _bookings.AsReadOnly();

        /// <summary>
        /// Private constructor required by Entity Framework Core for materialization.
        /// This constructor should not be used directly in business logic.
        /// </summary>
        private Room() { }

        /// <summary>
        /// Creates a new room with the specified details.
        /// This is the primary factory method for creating room instances.
        /// </summary>
        /// <param name="name">The name or number of the room. Cannot be null or empty.</param>
        /// <param name="description">The description of the room. Can be null or empty.</param>
        /// <param name="price">The price per night. Must be greater than zero.</param>
        /// <param name="capacity">The maximum number of guests. Must be greater than zero.</param>
        /// <param name="images">The collection of images for the room. Can be null or empty.</param>
        /// <returns>A new Room instance with the specified details.</returns>
        /// <exception cref="DomainException">Thrown when validation fails for any of the parameters.</exception>
        public static Room Create(string name, string description, decimal price, int capacity, List<GalleryImage>? images = null)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new DomainException("Room name is required.");
            if (price <= 0)
                throw new DomainException("Room price must be greater than zero.");
            if (capacity <= 0)
                throw new DomainException("Room capacity must be greater than zero.");

            return new Room
            {
                Id = Guid.NewGuid(),
                Name = name.Trim(),
                Description = description?.Trim() ?? string.Empty,
                Price = price,
                Capacity = capacity,
                Status = RoomStatus.Available,
                Images = images ?? new List<GalleryImage>()
            };
        }

        /// <summary>
        /// Updates the room's details with new information.
        /// This method validates all input parameters before updating the room properties.
        /// </summary>
        /// <param name="name">The new name or number of the room. Cannot be null or empty.</param>
        /// <param name="description">The new description of the room. Can be null or empty.</param>
        /// <param name="price">The new price per night. Must be greater than zero.</param>
        /// <param name="capacity">The new maximum number of guests. Must be greater than zero.</param>
        /// <param name="images">The new collection of images for the room. Can be null or empty.</param>
        /// <exception cref="DomainException">Thrown when validation fails for any of the parameters.</exception>
        public void UpdateDetails(string name, string description, decimal price, int capacity, List<GalleryImage> images)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new DomainException("Room name is required.");
            if (price <= 0)
                throw new DomainException("Room price must be greater than zero.");
            if (capacity <= 0)
                throw new DomainException("Room capacity must be greater than zero.");

            Name = name.Trim();
            Description = description?.Trim() ?? string.Empty;
            Price = price;
            Capacity = capacity;
            Images = images;
        }

        /// <summary>
        /// Adds a booking to this room's collection of bookings.
        /// This method validates that the booking belongs to this room and that
        /// the room is available during the booking period.
        /// </summary>
        /// <param name="booking">The booking to add to this room.</param>
        /// <exception cref="DomainException">Thrown when the booking is invalid or the room is not available.</exception>
        public void AddBooking(Booking booking)
        {
            if (booking == null)
                throw new DomainException("Booking cannot be null.");
            if (booking.RoomId != Id)
                throw new DomainException("Booking does not belong to this room.");
            if (!IsAvailable(booking.CheckIn, booking.CheckOut))
                throw new DomainException("Room is not available during the selected dates.");

            _bookings.Add(booking);
        }

        /// <summary>
        /// Checks if the room is available for booking during the specified date range.
        /// This method considers both the room's status and any existing bookings
        /// that might conflict with the requested dates.
        /// </summary>
        /// <param name="start">The start date of the requested booking period.</param>
        /// <param name="end">The end date of the requested booking period.</param>
        /// <returns>true if the room is available during the specified period; otherwise, false.</returns>
        /// <exception cref="DomainException">Thrown when the start date is not before the end date.</exception>
        public bool IsAvailable(DateTime start, DateTime end)
        {
            if (start >= end)
                throw new DomainException("Start date must be before end date.");

            if (Status != RoomStatus.Available)
                return false;

            // Check for overlapping confirmed or pending bookings
            return _bookings.All(b =>
                (b.Status != BookingStatus.Confirmed && b.Status != BookingStatus.Pending) ||
                end <= b.CheckIn || start >= b.CheckOut);
        }

        /// <summary>
        /// Activates the room, making it available for booking.
        /// This method changes the room status to Available.
        /// </summary>
        /// <exception cref="DomainException">Thrown when the room is already active.</exception>
        public void Activate()
        {
            if (Status == RoomStatus.Available)
                throw new DomainException("Room is already active.");

            Status = RoomStatus.Available;
        }

        /// <summary>
        /// Deactivates the room, making it unavailable for new bookings.
        /// This method checks for future bookings before allowing deactivation.
        /// </summary>
        /// <exception cref="DomainException">Thrown when the room is already inactive or has future bookings.</exception>
        public void Deactivate()
        {
            if (Status == RoomStatus.Inactive)
                throw new DomainException("Room is already inactive.");

            // Check for future confirmed bookings
            bool hasFutureBookings = _bookings.Any(b =>
                b.CheckIn > DateTime.UtcNow &&
                (b.Status == BookingStatus.Confirmed || b.Status == BookingStatus.Pending));

            if (hasFutureBookings)
                throw new DomainException("Cannot deactivate room with upcoming bookings.");

            Status = RoomStatus.Inactive;
        }

        /// <summary>
        /// Sets the room status to maintenance, making it unavailable for booking.
        /// This method checks for future bookings before allowing maintenance status.
        /// </summary>
        /// <exception cref="DomainException">Thrown when the room is already under maintenance or has future bookings.</exception>
        public void SetMaintenance()
        {
            if (Status == RoomStatus.Maintenance)
                throw new DomainException("Room is already under maintenance.");

            // Check for future confirmed bookings
            bool hasFutureBookings = _bookings.Any(b =>
                b.CheckIn > DateTime.UtcNow &&
                (b.Status == BookingStatus.Confirmed || b.Status == BookingStatus.Pending));

            if (hasFutureBookings)
                throw new DomainException("Cannot set room to maintenance with upcoming bookings.");

            Status = RoomStatus.Maintenance;
        }

        /// <summary>
        /// Calculates the total revenue generated by this room for completed bookings.
        /// This method can filter revenue by date range if specified.
        /// </summary>
        /// <param name="startDate">Optional start date for revenue calculation. If null, includes all completed bookings.</param>
        /// <param name="endDate">Optional end date for revenue calculation. If null, includes all completed bookings.</param>
        /// <returns>The total revenue generated by completed bookings in the specified period.</returns>
        public decimal CalculateRevenue(DateTime? startDate = null, DateTime? endDate = null)
        {
            var completedBookings = _bookings.Where(b => b.Status == BookingStatus.Completed);

            if (startDate.HasValue)
                completedBookings = completedBookings.Where(b => b.CheckOut >= startDate.Value);

            if (endDate.HasValue)
                completedBookings = completedBookings.Where(b => b.CheckIn <= endDate.Value);

            return completedBookings.Sum(b => b.TotalAmount);
        }

        /// <summary>
        /// Calculates the occupancy rate for this room during the specified date range.
        /// The occupancy rate is the percentage of days the room was occupied during the period.
        /// </summary>
        /// <param name="startDate">The start date for occupancy calculation.</param>
        /// <param name="endDate">The end date for occupancy calculation.</param>
        /// <returns>The occupancy rate as a percentage (0-100).</returns>
        /// <exception cref="DomainException">Thrown when the start date is not before the end date.</exception>
        public int GetOccupancyRate(DateTime startDate, DateTime endDate)
        {
            if (startDate >= endDate)
                throw new DomainException("Start date must be before end date.");

            var totalDays = (endDate - startDate).Days;
            if (totalDays <= 0) return 0;

            var occupiedDays = _bookings
                .Where(b => b.Status == BookingStatus.Completed)
                .Where(b => b.CheckOut > startDate && b.CheckIn < endDate)
                .Sum(b =>
                {
                    var effectiveStart = b.CheckIn > startDate ? b.CheckIn : startDate;
                    var effectiveEnd = b.CheckOut < endDate ? b.CheckOut : endDate;
                    return (effectiveEnd - effectiveStart).Days;
                });

            return (int)Math.Round((double)occupiedDays / totalDays * 100);
        }
    }
}