using System;
using WPHBookingSystem.Domain.Entities.Common;
using WPHBookingSystem.Domain.Enums;
using WPHBookingSystem.Domain.Exceptions;

namespace WPHBookingSystem.Domain.Entities
{
    public class Booking : BaseAuditable
    {
        public Guid Id { get; private set; }
        public Guid UserId { get; private set; }
        public Guid RoomId { get; private set; }
        public DateTime CheckIn { get; private set; }
        public DateTime CheckOut { get; private set; }
        public int Guests { get; private set; }
        public decimal TotalAmount { get; private set; }
        public BookingStatus Status { get; private set; } = BookingStatus.Pending;
        public string SpecialRequests { get; private set; } = string.Empty;
        public string Phone { get; private set; } = string.Empty;
        public string Address { get; private set; } = string.Empty;
        public virtual Room? Room { get; set; }

        // Private constructor for EF Core
        private Booking() { }

        // --- Static Factory Method ---
        public static Booking Create(
            Guid userId,
            Guid roomId,
            DateTime checkIn,
            DateTime checkOut,
            int guests,
            decimal totalAmount,
            string specialRequests = "",
            string phone = "",
            string address = "")
        {
            if (checkIn >= checkOut)
                throw new DomainException("Check-out must be after check-in.");
            if (checkIn.Date < DateTime.UtcNow.Date)
                throw new DomainException("Check-in date must not be in the past.");

            return new Booking
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                RoomId = roomId,
                CheckIn = checkIn,
                CheckOut = checkOut,
                Guests = guests,
                TotalAmount = totalAmount,
                SpecialRequests = specialRequests,
                Phone = phone,
                Address = address,
                Status = BookingStatus.Pending
            };
        }

        // --- Domain Logic Methods ---
        public void Confirm()
        {
            if (Status != BookingStatus.Pending)
                throw new DomainException("Only pending bookings can be confirmed.");
            Status = BookingStatus.Confirmed;
        }

        public void Cancel()
        {
            if (Status == BookingStatus.Completed)
                throw new DomainException("Completed bookings cannot be cancelled.");
            Status = BookingStatus.Cancelled;
        }

        public void Complete()
        {
            if (Status != BookingStatus.Confirmed)
                throw new DomainException("Only confirmed bookings can be completed.");
            if (DateTime.UtcNow < CheckOut)
                throw new DomainException("Check-out date has not passed yet.");
            Status = BookingStatus.Completed;
        }

        public void UpdateBookingDates(DateTime newCheckIn, DateTime newCheckOut)
        {
            if (Status != BookingStatus.Pending)
                throw new DomainException("Only pending bookings can be updated.");
            if (newCheckIn >= newCheckOut)
                throw new DomainException("Check-out must be after check-in.");
            if (newCheckIn.Date < DateTime.UtcNow.Date)
                throw new DomainException("Check-in date must not be in the past.");

            CheckIn = newCheckIn;
            CheckOut = newCheckOut;
        }

        public bool IsDateValid()
        {
            return CheckIn < CheckOut && CheckIn.Date >= DateTime.UtcNow.Date;
        }

        // Private helper method for date validation (kept for potential future use)
        private static void ValidateDates(DateTime checkIn, DateTime checkOut)
        {
            if (checkIn >= checkOut)
                throw new DomainException("Check-out must be after check-in.");
            if (checkIn.Date < DateTime.UtcNow.Date)
                throw new DomainException("Check-in date must not be in the past.");
        }
    }
}