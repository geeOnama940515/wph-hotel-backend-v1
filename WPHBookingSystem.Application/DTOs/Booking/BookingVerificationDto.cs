using System;
using System.ComponentModel.DataAnnotations;

namespace WPHBookingSystem.Application.DTOs.Booking
{
    /// <summary>
    /// Data transfer object for booking verification with OTP.
    /// </summary>
    public class BookingVerificationDto
    {
        [Required(ErrorMessage = "Booking ID is required.")]
        public Guid BookingId { get; set; }

        [Required(ErrorMessage = "OTP code is required.")]
        [StringLength(6, MinimumLength = 6, ErrorMessage = "OTP code must be exactly 6 characters.")]
        public string OtpCode { get; set; } = string.Empty;
    }
} 