using WPHBookingSystem.Application.Interfaces.Common;
using WPHBookingSystem.Domain.Entities;

namespace WPHBookingSystem.Application.Interfaces
{
    /// <summary>
    /// Repository interface for OTP verification data access.
    /// 
    /// This repository provides methods for storing and retrieving OTP verification
    /// records used in the booking confirmation process.
    /// </summary>
    public interface IOtpRepository : IGenericRepository<OtpVerification>
    {
        /// <summary>
        /// Gets the OTP verification record for the specified booking and email.
        /// </summary>
        /// <param name="bookingId">The booking ID</param>
        /// <param name="emailAddress">The email address</param>
        /// <returns>The OTP verification record if found; otherwise, null</returns>
        Task<OtpVerification?> GetByBookingAndEmailAsync(Guid bookingId, string emailAddress);

        /// <summary>
        /// Gets all OTP verification records for the specified booking.
        /// </summary>
        /// <param name="bookingId">The booking ID</param>
        /// <returns>Collection of OTP verification records</returns>
        Task<IEnumerable<OtpVerification>> GetByBookingIdAsync(Guid bookingId);

        /// <summary>
        /// Invalidates all OTP verification records for the specified booking.
        /// </summary>
        /// <param name="bookingId">The booking ID</param>
        /// <returns>Number of records invalidated</returns>
        Task<int> InvalidateByBookingIdAsync(Guid bookingId);

        /// <summary>
        /// Deletes expired OTP verification records.
        /// </summary>
        /// <returns>Number of records deleted</returns>
        Task<int> DeleteExpiredAsync();
    }
} 