using System;
using WPHBookingSystem.Domain.Entities.Common;
using WPHBookingSystem.Domain.Enums;
using WPHBookingSystem.Domain.Exceptions;
using WPHBookingSystem.Domain.ValueObjects;

namespace WPHBookingSystem.Domain.Entities
{
    /// <summary>
    /// Represents a booking reservation in the hotel booking system.
    /// This entity encapsulates all the information and behavior related to a guest's booking,
    /// including dates, contact information, payment details, and booking status.
    /// 
    /// The Booking entity follows the Domain-Driven Design pattern with encapsulated business logic
    /// and invariant protection through private setters and validation methods.
    /// </summary>
    public class Booking : BaseAuditable
    {
        /// <summary>
        /// Gets the unique identifier for this booking.
        /// This ID is automatically generated when the booking is created.
        /// </summary>
        public Guid Id { get; private set; }

        /// <summary>
        /// Gets the unique identifier of the user who made this booking.
        /// This links the booking to the user account in the system.
        /// </summary>
        public string GuestName { get; private set; }

        /// <summary>
        /// Gets the unique identifier of the room that is booked.
        /// This links the booking to the specific room in the hotel.
        /// </summary>
        public Guid RoomId { get; private set; }

        /// <summary>
        /// Gets the check-in date for this booking.
        /// This is the date when the guest is expected to arrive and check in.
        /// </summary>
        public DateTime CheckIn { get; private set; }

        /// <summary>
        /// Gets the check-out date for this booking.
        /// This is the date when the guest is expected to check out and leave.
        /// </summary>
        public DateTime CheckOut { get; private set; }

        /// <summary>
        /// Gets the number of guests for this booking.
        /// This determines the occupancy and may affect pricing or room selection.
        /// </summary>
        public int Guests { get; private set; }

        /// <summary>
        /// Gets the contact information for the guest making this booking.
        /// This includes phone number and address for communication purposes.
        /// </summary>
        public ContactInfo ContactInfo { get; private set; }

        /// <summary>
        /// Gets the email address for the guest making this booking.
        /// This is used for booking confirmations and communication.
        /// </summary>
        public string EmailAddress { get; private set; }

        /// <summary>
        /// Gets the total amount for this booking.
        /// This is the total cost including any taxes, fees, or additional charges.
        /// </summary>
        public decimal TotalAmount { get; private set; }

        /// <summary>
        /// Gets the current status of this booking.
        /// This tracks the booking through its lifecycle from creation to completion.
        /// </summary>
        public BookingStatus Status { get; private set; } = BookingStatus.Pending;

        /// <summary>
        /// Gets any special requests or notes for this booking.
        /// This can include dietary requirements, accessibility needs, or other special arrangements.
        /// </summary>
        public string SpecialRequests { get; private set; } = string.Empty;

        /// <summary>
        /// Gets the unique booking token for this reservation.
        /// This token is used for guest identification and booking management without exposing the booking ID.
        /// </summary>
        public Guid BookingToken { get; private set; }

        /// <summary>
        /// Gets or sets the navigation property to the associated room.
        /// This is used by Entity Framework Core for relationship mapping.
        /// </summary>
        public virtual Room? Room { get; set; }

        /// <summary>
        /// Private constructor required by Entity Framework Core for materialization.
        /// This constructor should not be used directly in business logic.
        /// </summary>
        private Booking() { }

        /// <summary>
        /// Creates a new booking with the specified details.
        /// This is the primary factory method for creating booking instances.
        /// </summary>
        /// <param name="roomId">The ID of the room being booked.</param>
        /// <param name="checkIn">The check-in date. Must be in the future.</param>
        /// <param name="checkOut">The check-out date. Must be after check-in.</param>
        /// <param name="guests">The number of guests. Must be greater than zero.</param>
        /// <param name="totalAmount">The total amount for the booking. Must be greater than zero.</param>
        /// <param name="contactInfo">The contact information for the guest.</param>
        /// <param name="emailAddress">The email address for the guest. Can be empty.</param>
        /// <param name="specialRequests">Any special requests or notes. Can be empty.</param>
        /// <returns>A new Booking instance with the specified details.</returns>
        /// <exception cref="DomainException">Thrown when validation fails for any of the parameters.</exception>
        public static Booking Create(
            Guid roomId,
            DateTime checkIn,
            DateTime checkOut,
            int guests,
            decimal totalAmount,
            ContactInfo contactInfo,
            string emailAddress = "",
            string specialRequests = "",
            string guestName =""
        )
        {
            if (checkIn >= checkOut)
                throw new DomainException("Check-out must be after check-in.");
            if (checkIn.Date < DateTime.UtcNow.Date)
                throw new DomainException("Check-in date must not be in the past.");

            return new Booking
            {
                Id = Guid.NewGuid(),
                RoomId = roomId,
                CheckIn = checkIn,
                CheckOut = checkOut,
                Guests = guests,
                TotalAmount = totalAmount,
                ContactInfo = contactInfo,
                EmailAddress = emailAddress,
                SpecialRequests = specialRequests,
                Status = BookingStatus.Pending,
                BookingToken = Guid.NewGuid(),
                GuestName = guestName
            };
        }

        /// <summary>
        /// Confirms this booking, changing its status from Pending to Confirmed.
        /// Only pending bookings can be confirmed.
        /// </summary>
        /// <exception cref="DomainException">Thrown when the booking is not in Pending status.</exception>
        public void Confirm()
        {
            if (Status != BookingStatus.Pending)
                throw new DomainException("Only pending bookings can be confirmed.");
            Status = BookingStatus.Confirmed;
        }
        public void CheckedIn()
        {
            if (Status != BookingStatus.Confirmed)
                throw new DomainException("Only pending bookings can be checked in.");
            Status = BookingStatus.CheckedIn;
        }
        public void CheckedOut()
        {
            if (Status != BookingStatus.CheckedIn)
                throw new DomainException("Only checked in bookings can be checked out.");
            Status = BookingStatus.CheckedOut;
        }
        /// <summary>
        /// Cancels this booking, changing its status to Cancelled.
        /// Completed bookings cannot be cancelled.
        /// </summary>
        /// <exception cref="DomainException">Thrown when the booking is already completed.</exception>
        public void Cancel()
        {
            if (Status == BookingStatus.Completed)
                throw new DomainException("Completed bookings cannot be cancelled.");
            Status = BookingStatus.Cancelled;
        }

        /// <summary>
        /// Completes this booking, changing its status to Completed.
        /// Only confirmed bookings can be completed, and the check-out date must have passed.
        /// </summary>
        /// <exception cref="DomainException">Thrown when the booking is not confirmed or check-out date hasn't passed.</exception>
        public void Complete()
        {
            if (Status != BookingStatus.CheckedOut)
                throw new DomainException("Only checked out bookings can be completed.");
            if (DateTime.UtcNow < CheckOut)
                throw new DomainException("Check-out date has not passed yet.");
            Status = BookingStatus.Completed;
        }

        /// <summary>
        /// Updates the booking dates for this reservation.
        /// Only pending bookings can have their dates updated.
        /// </summary>
        /// <param name="newCheckIn">The new check-in date. Must be in the future.</param>
        /// <param name="newCheckOut">The new check-out date. Must be after check-in.</param>
        /// <exception cref="DomainException">Thrown when the booking is not pending or dates are invalid.</exception>
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

        /// <summary>
        /// Validates that the booking dates are valid.
        /// Checks that check-in is before check-out and that check-in is not in the past.
        /// </summary>
        /// <returns>true if the booking dates are valid; otherwise, false.</returns>
        public bool IsDateValid()
        {
            return CheckIn < CheckOut && CheckIn.Date >= DateTime.UtcNow.Date;
        }
    }
}
