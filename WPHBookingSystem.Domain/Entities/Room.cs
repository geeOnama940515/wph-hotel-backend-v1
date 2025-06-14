using System;
using System.Collections.Generic;
using System.Linq;
using WPHBookingSystem.Domain.Entities.Common;
using WPHBookingSystem.Domain.Enums;
using WPHBookingSystem.Domain.Exceptions;

namespace WPHBookingSystem.Domain.Entities
{
    public class Room : BaseAuditable
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; } = string.Empty;
        public string Description { get; private set; } = string.Empty;
        public decimal Price { get; private set; }
        public int Capacity { get; private set; }
        public string? Image { get; private set; } = string.Empty;
        public string? Image2 { get; private set; } = string.Empty;
        public string? Image3 { get; private set; } = string.Empty;
        public string? Image4 { get; private set; } = string.Empty;
        public string? Image5 { get; private set; } = string.Empty;
        public string? Image6 { get; private set; } = string.Empty;
        public RoomStatus Status { get; private set; } = RoomStatus.Available;

        private readonly List<Booking> _bookings = new();
        public IReadOnlyCollection<Booking> Bookings => _bookings.AsReadOnly();

        // Private constructor for EF Core
        private Room() { }

        // Static factory method
        public static Room Create(string name, string description, decimal price, int capacity, string image = "")
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
                Image = image?.Trim() ?? string.Empty,
                Status = RoomStatus.Available
            };
        }

        public void UpdateDetails(string name, string description, decimal price, int capacity, string image = "")
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
            Image = image?.Trim() ?? string.Empty;
        }

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

        public void Activate()
        {
            if (Status == RoomStatus.Available)
                throw new DomainException("Room is already active.");

            Status = RoomStatus.Available;
        }

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

        public decimal CalculateRevenue(DateTime? startDate = null, DateTime? endDate = null)
        {
            var completedBookings = _bookings.Where(b => b.Status == BookingStatus.Completed);

            if (startDate.HasValue)
                completedBookings = completedBookings.Where(b => b.CheckOut >= startDate.Value);

            if (endDate.HasValue)
                completedBookings = completedBookings.Where(b => b.CheckIn <= endDate.Value);

            return completedBookings.Sum(b => b.TotalAmount);
        }

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