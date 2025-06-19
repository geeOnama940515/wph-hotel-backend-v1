using System;
using System.Security.Cryptography;
using System.Text;
using WPHBookingSystem.Domain.Entities.Common;

namespace WPHBookingSystem.Domain.Entities
{
    /// <summary>
    /// Represents an OTP verification record for booking confirmation.
    /// This entity stores OTP codes, their expiration times, and attempt counters
    /// to ensure secure booking verification.
    /// </summary>
    public class OtpVerification : BaseAuditable
    {
        /// <summary>
        /// Gets the unique identifier for this OTP verification record.
        /// </summary>
        public Guid Id { get; private set; }

        /// <summary>
        /// Gets the booking ID this OTP is associated with.
        /// </summary>
        public Guid BookingId { get; private set; }

        /// <summary>
        /// Gets the email address this OTP was sent to.
        /// </summary>
        public string EmailAddress { get; private set; }

        /// <summary>
        /// Gets the hashed OTP code (not stored in plain text for security).
        /// </summary>
        public string HashedOtpCode { get; private set; }

        /// <summary>
        /// Gets the expiration time for this OTP.
        /// </summary>
        public DateTime ExpiresAt { get; private set; }

        /// <summary>
        /// Gets the number of verification attempts made.
        /// </summary>
        public int Attempts { get; private set; }

        /// <summary>
        /// Gets whether this OTP has been used successfully.
        /// </summary>
        public bool IsUsed { get; private set; }

        /// <summary>
        /// Gets whether this OTP has been invalidated.
        /// </summary>
        public bool IsInvalidated { get; private set; }

        /// <summary>
        /// Private constructor required by Entity Framework Core.
        /// </summary>
        private OtpVerification() { }

        /// <summary>
        /// Creates a new OTP verification record.
        /// </summary>
        /// <param name="bookingId">The booking ID to associate with this OTP</param>
        /// <param name="emailAddress">The email address to send OTP to</param>
        /// <param name="otpCode">The plain text OTP code (will be hashed)</param>
        /// <param name="expirationMinutes">Minutes until OTP expires (default: 15)</param>
        /// <returns>A new OtpVerification instance</returns>
        public static OtpVerification Create(Guid bookingId, string emailAddress, string otpCode, int expirationMinutes = 15)
        {
            if (string.IsNullOrWhiteSpace(emailAddress))
                throw new ArgumentException("Email address cannot be null or empty.", nameof(emailAddress));

            if (string.IsNullOrWhiteSpace(otpCode))
                throw new ArgumentException("OTP code cannot be null or empty.", nameof(otpCode));

            if (expirationMinutes <= 0)
                throw new ArgumentException("Expiration minutes must be greater than 0.", nameof(expirationMinutes));

            return new OtpVerification
            {
                Id = Guid.NewGuid(),
                BookingId = bookingId,
                EmailAddress = emailAddress.ToLowerInvariant(),
                HashedOtpCode = HashOtpCode(otpCode),
                ExpiresAt = DateTime.UtcNow.AddMinutes(expirationMinutes),
                Attempts = 0,
                IsUsed = false,
                IsInvalidated = false
            };
        }

        /// <summary>
        /// Validates the provided OTP code against the stored hash.
        /// </summary>
        /// <param name="otpCode">The OTP code to validate</param>
        /// <returns>True if the OTP is valid and not expired; otherwise, false</returns>
        public bool ValidateOtp(string otpCode)
        {
            if (IsUsed || IsInvalidated)
                return false;

            if (DateTime.UtcNow > ExpiresAt)
                return false;

            if (Attempts >= 5) // Maximum 5 attempts
                return false;

            var hashedInput = HashOtpCode(otpCode);
            return hashedInput == HashedOtpCode;
        }

        /// <summary>
        /// Increments the attempt counter.
        /// </summary>
        public void IncrementAttempts()
        {
            Attempts++;
        }

        /// <summary>
        /// Marks the OTP as used after successful verification.
        /// </summary>
        public void MarkAsUsed()
        {
            IsUsed = true;
        }

        /// <summary>
        /// Invalidates the OTP (e.g., after too many failed attempts).
        /// </summary>
        public void Invalidate()
        {
            IsInvalidated = true;
        }

        /// <summary>
        /// Checks if the OTP is expired.
        /// </summary>
        /// <returns>True if the OTP is expired; otherwise, false</returns>
        public bool IsExpired()
        {
            return DateTime.UtcNow > ExpiresAt;
        }

        /// <summary>
        /// Checks if the OTP has exceeded the maximum number of attempts.
        /// </summary>
        /// <returns>True if maximum attempts exceeded; otherwise, false</returns>
        public bool HasExceededMaxAttempts()
        {
            return Attempts >= 5;
        }

        /// <summary>
        /// Hashes the OTP code using SHA256 for secure storage.
        /// </summary>
        /// <param name="otpCode">The plain text OTP code</param>
        /// <returns>The hashed OTP code</returns>
        private static string HashOtpCode(string otpCode)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(otpCode);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
    }
} 