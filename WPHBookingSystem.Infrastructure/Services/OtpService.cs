using System;
using System.Security.Cryptography;
using System.Threading.Tasks;
using WPHBookingSystem.Application.Interfaces;
using WPHBookingSystem.Application.Interfaces.Services;
using WPHBookingSystem.Domain.Entities;

namespace WPHBookingSystem.Infrastructure.Services
{
    /// <summary>
    /// Service implementation for OTP (One-Time Password) operations.
    /// 
    /// This service provides functionality for generating, storing, and validating
    /// OTP codes used for booking verification.
    /// </summary>
    public class OtpService : IOtpService
    {
        private readonly IOtpRepository _otpRepository;
        private readonly IUnitOfWork _unitOfWork;

        /// <summary>
        /// Initializes a new instance of the OtpService with the required dependencies.
        /// </summary>
        /// <param name="otpRepository">The OTP repository for data access</param>
        /// <param name="unitOfWork">The unit of work for transaction management</param>
        public OtpService(IOtpRepository otpRepository, IUnitOfWork unitOfWork)
        {
            _otpRepository = otpRepository ?? throw new ArgumentNullException(nameof(otpRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        /// <summary>
        /// Generates a new OTP code for the specified booking.
        /// </summary>
        /// <param name="bookingId">The booking ID to generate OTP for</param>
        /// <param name="emailAddress">The email address for the booking</param>
        /// <returns>The generated 6-digit OTP code</returns>
        public async Task<string> GenerateOtpAsync(Guid bookingId, string emailAddress)
        {
            // Invalidate any existing OTPs for this booking
            await _otpRepository.InvalidateByBookingIdAsync(bookingId);

            // Generate a new 6-digit OTP code
            var otpCode = GenerateRandomOtp();

            // Create OTP verification record
            var otpVerification = OtpVerification.Create(bookingId, emailAddress, otpCode);

            // Save to database
            await _otpRepository.AddAsync(otpVerification);
            await _unitOfWork.SaveChangesAsync();

            return otpCode;
        }

        /// <summary>
        /// Validates the provided OTP code for the specified booking.
        /// </summary>
        /// <param name="bookingId">The booking ID to validate OTP for</param>
        /// <param name="otpCode">The OTP code to validate</param>
        /// <param name="emailAddress">The email address for the booking</param>
        /// <returns>True if the OTP is valid and not expired; otherwise, false</returns>
        public async Task<bool> ValidateOtpAsync(Guid bookingId, string otpCode, string emailAddress)
        {
            // Get the OTP verification record
            var otpRecord = await _otpRepository.GetByBookingAndEmailAsync(bookingId, emailAddress);
            if (otpRecord == null)
                return false;

            // Validate the OTP code
            var isValid = otpRecord.ValidateOtp(otpCode);
            if (isValid)
            {
                // Mark as used if valid
                otpRecord.MarkAsUsed();
                await _otpRepository.UpdateAsync(otpRecord);
                await _unitOfWork.SaveChangesAsync();
            }
            else
            {
                // Increment attempts if invalid
                otpRecord.IncrementAttempts();
                await _otpRepository.UpdateAsync(otpRecord);
                await _unitOfWork.SaveChangesAsync();
            }

            return isValid;
        }

        /// <summary>
        /// Checks if an OTP exists for the specified booking.
        /// </summary>
        /// <param name="bookingId">The booking ID to check</param>
        /// <param name="emailAddress">The email address for the booking</param>
        /// <returns>True if an OTP exists; otherwise, false</returns>
        public async Task<bool> OtpExistsAsync(Guid bookingId, string emailAddress)
        {
            var otpRecord = await _otpRepository.GetByBookingAndEmailAsync(bookingId, emailAddress);
            return otpRecord != null;
        }

        /// <summary>
        /// Invalidates the OTP for the specified booking after successful verification.
        /// </summary>
        /// <param name="bookingId">The booking ID to invalidate OTP for</param>
        /// <param name="emailAddress">The email address for the booking</param>
        /// <returns>True if the OTP was successfully invalidated; otherwise, false</returns>
        public async Task<bool> InvalidateOtpAsync(Guid bookingId, string emailAddress)
        {
            var otpRecord = await _otpRepository.GetByBookingAndEmailAsync(bookingId, emailAddress);
            if (otpRecord == null)
                return false;

            otpRecord.Invalidate();
            await _otpRepository.UpdateAsync(otpRecord);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        /// <summary>
        /// Gets the number of OTP attempts for the specified booking.
        /// </summary>
        /// <param name="bookingId">The booking ID to check attempts for</param>
        /// <param name="emailAddress">The email address for the booking</param>
        /// <returns>The number of OTP attempts</returns>
        public async Task<int> GetOtpAttemptsAsync(Guid bookingId, string emailAddress)
        {
            var otpRecord = await _otpRepository.GetByBookingAndEmailAsync(bookingId, emailAddress);
            return otpRecord?.Attempts ?? 0;
        }

        /// <summary>
        /// Increments the OTP attempt counter for the specified booking.
        /// </summary>
        /// <param name="bookingId">The booking ID to increment attempts for</param>
        /// <param name="emailAddress">The email address for the booking</param>
        /// <returns>True if the attempt counter was incremented; otherwise, false</returns>
        public async Task<bool> IncrementOtpAttemptsAsync(Guid bookingId, string emailAddress)
        {
            var otpRecord = await _otpRepository.GetByBookingAndEmailAsync(bookingId, emailAddress);
            if (otpRecord == null)
                return false;

            otpRecord.IncrementAttempts();
            await _otpRepository.UpdateAsync(otpRecord);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        /// <summary>
        /// Generates a random 6-digit OTP code.
        /// </summary>
        /// <returns>A 6-digit OTP code</returns>
        private static string GenerateRandomOtp()
        {
            using var rng = RandomNumberGenerator.Create();
            var bytes = new byte[4];
            rng.GetBytes(bytes);
            
            var random = new Random(BitConverter.ToInt32(bytes, 0));
            return random.Next(100000, 999999).ToString();
        }
    }
} 