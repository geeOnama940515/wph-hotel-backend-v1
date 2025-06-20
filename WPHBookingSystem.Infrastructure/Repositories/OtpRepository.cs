using Microsoft.EntityFrameworkCore;
using WPHBookingSystem.Application.Interfaces;
using WPHBookingSystem.Domain.Entities;
using WPHBookingSystem.Infrastructure.Persistence.Data;

namespace WPHBookingSystem.Infrastructure.Repositories
{
    /// <summary>
    /// Repository implementation for OTP verification data access.
    /// 
    /// This repository provides methods for storing and retrieving OTP verification
    /// records used in the booking confirmation process.
    /// </summary>
    public class OtpRepository : GenericRepository<OtpVerification>, IOtpRepository
    {
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Initializes a new instance of the OtpRepository with the application database context.
        /// </summary>
        /// <param name="context">The application database context</param>
        public OtpRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        /// <summary>
        /// Gets the OTP verification record for the specified booking and email.
        /// </summary>
        /// <param name="bookingId">The booking ID</param>
        /// <param name="emailAddress">The email address</param>
        /// <returns>The OTP verification record if found; otherwise, null</returns>
        public async Task<OtpVerification?> GetByBookingAndEmailAsync(Guid bookingId, string emailAddress)
        {
            return await _context.OtpVerifications
                .Where(o => o.BookingId == bookingId && 
                           o.EmailAddress.ToLower() == emailAddress.ToLower() &&
                           !o.IsUsed && 
                           !o.IsInvalidated &&
                           o.ExpiresAt > DateTime.UtcNow)
                .OrderByDescending(o => o.CreatedAt)
                .FirstOrDefaultAsync();
        }

        /// <summary>
        /// Gets all OTP verification records for the specified booking.
        /// </summary>
        /// <param name="bookingId">The booking ID</param>
        /// <returns>Collection of OTP verification records</returns>
        public async Task<IEnumerable<OtpVerification>> GetByBookingIdAsync(Guid bookingId)
        {
            return await _context.OtpVerifications
                .Where(o => o.BookingId == bookingId)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
        }

        /// <summary>
        /// Invalidates all OTP verification records for the specified booking.
        /// </summary>
        /// <param name="bookingId">The booking ID</param>
        /// <returns>Number of records invalidated</returns>
        public async Task<int> InvalidateByBookingIdAsync(Guid bookingId)
        {
            var otpRecords = await _context.OtpVerifications
                .Where(o => o.BookingId == bookingId && !o.IsInvalidated)
                .ToListAsync();

            foreach (var record in otpRecords)
            {
                record.Invalidate();
            }

            return otpRecords.Count;
        }

        /// <summary>
        /// Deletes expired OTP verification records.
        /// </summary>
        /// <returns>Number of records deleted</returns>
        public async Task<int> DeleteExpiredAsync()
        {
            var expiredRecords = await _context.OtpVerifications
                .Where(o => o.ExpiresAt < DateTime.UtcNow)
                .ToListAsync();

            _context.OtpVerifications.RemoveRange(expiredRecords);
            return expiredRecords.Count;
        }
    }
} 