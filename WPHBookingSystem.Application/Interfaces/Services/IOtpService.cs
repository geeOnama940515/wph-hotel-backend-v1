using System;

namespace WPHBookingSystem.Application.Interfaces.Services
{
    /// <summary>
    /// Service interface for OTP (One-Time Password) operations.
    /// 
    /// This service provides functionality for generating, storing, and validating
    /// OTP codes used for booking verification.
    /// </summary>
    public interface IOtpService
    {
        /// <summary>
        /// Generates a new OTP code for the specified booking.
        /// </summary>
        /// <param name="bookingId">The booking ID to generate OTP for</param>
        /// <param name="emailAddress">The email address for the booking</param>
        /// <returns>The generated 6-digit OTP code</returns>
        Task<string> GenerateOtpAsync(Guid bookingId, string emailAddress);

        /// <summary>
        /// Validates the provided OTP code for the specified booking.
        /// </summary>
        /// <param name="bookingId">The booking ID to validate OTP for</param>
        /// <param name="otpCode">The OTP code to validate</param>
        /// <param name="emailAddress">The email address for the booking</param>
        /// <returns>True if the OTP is valid and not expired; otherwise, false</returns>
        Task<bool> ValidateOtpAsync(Guid bookingId, string otpCode, string emailAddress);

        /// <summary>
        /// Checks if an OTP exists for the specified booking.
        /// </summary>
        /// <param name="bookingId">The booking ID to check</param>
        /// <param name="emailAddress">The email address for the booking</param>
        /// <returns>True if an OTP exists; otherwise, false</returns>
        Task<bool> OtpExistsAsync(Guid bookingId, string emailAddress);

        /// <summary>
        /// Invalidates the OTP for the specified booking after successful verification.
        /// </summary>
        /// <param name="bookingId">The booking ID to invalidate OTP for</param>
        /// <param name="emailAddress">The email address for the booking</param>
        /// <returns>True if the OTP was successfully invalidated; otherwise, false</returns>
        Task<bool> InvalidateOtpAsync(Guid bookingId, string emailAddress);

        /// <summary>
        /// Gets the number of OTP attempts for the specified booking.
        /// </summary>
        /// <param name="bookingId">The booking ID to check attempts for</param>
        /// <param name="emailAddress">The email address for the booking</param>
        /// <returns>The number of OTP attempts</returns>
        Task<int> GetOtpAttemptsAsync(Guid bookingId, string emailAddress);

        /// <summary>
        /// Increments the OTP attempt counter for the specified booking.
        /// </summary>
        /// <param name="bookingId">The booking ID to increment attempts for</param>
        /// <param name="emailAddress">The email address for the booking</param>
        /// <returns>True if the attempt counter was incremented; otherwise, false</returns>
        Task<bool> IncrementOtpAttemptsAsync(Guid bookingId, string emailAddress);
    }
} 