using System;
using System.ComponentModel.DataAnnotations;

namespace WPHBookingSystem.Application.DTOs.Booking
{
    /// <summary>
    /// Data transfer object for resending OTP verification codes.
    /// </summary>
    public class ResendOtpDto
    {
        [Required(ErrorMessage = "Booking ID is required.")]
        public Guid BookingId { get; set; }

        [Required(ErrorMessage = "Email address is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string EmailAddress { get; set; } = string.Empty;
    }
} 