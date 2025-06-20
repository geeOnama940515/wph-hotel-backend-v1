using System;

namespace WPHBookingSystem.Application.DTOs.Booking
{
    /// <summary>
    /// Data transfer object returned after successful booking creation.
    /// </summary>
    public class BookingCreatedDto
    {
        /// <summary>
        /// Gets or sets the unique identifier of the created booking.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the unique booking token for guest identification.
        /// </summary>
        public Guid BookingToken { get; set; }

        /// <summary>
        /// Gets or sets whether email verification is required.
        /// </summary>
        public bool RequiresEmailVerification { get; set; }

        /// <summary>
        /// Gets or sets the email address where the verification code was sent.
        /// </summary>
        public string EmailAddress { get; set; } = string.Empty;
    }
} 