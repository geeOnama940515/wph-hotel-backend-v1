using System;
using System.Threading.Tasks;
using WPHBookingSystem.Application.Common;
using WPHBookingSystem.Application.DTOs.Booking;
using WPHBookingSystem.Application.Interfaces;
using WPHBookingSystem.Application.Interfaces.Services;
using WPHBookingSystem.Domain.Entities;

namespace WPHBookingSystem.Application.UseCases.Bookings
{
    /// <summary>
    /// Use case responsible for resending OTP codes for booking verification.
    /// This use case generates a new OTP code and sends it to the guest's email.
    /// </summary>
    public class ResendOtpUseCase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOtpService _otpService;
        private readonly IEmailSenderService _emailService;

        /// <summary>
        /// Initializes a new instance of the ResendOtpUseCase with the required dependencies.
        /// </summary>
        /// <param name="unitOfWork">The unit of work for transaction management and data access.</param>
        /// <param name="otpService">The OTP service for generation and management.</param>
        /// <param name="emailService">The email service for sending OTP emails.</param>
        /// <exception cref="ArgumentNullException">Thrown when any dependency is null.</exception>
        public ResendOtpUseCase(IUnitOfWork unitOfWork, IOtpService otpService, IEmailSenderService emailService)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _otpService = otpService ?? throw new ArgumentNullException(nameof(otpService));
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
        }

        /// <summary>
        /// Executes the resend OTP use case with the specified details.
        /// </summary>
        /// <param name="dto">The data transfer object containing booking ID and email address.</param>
        /// <returns>A result indicating success or failure of the OTP resend.</returns>
        public async Task<Result<string>> ExecuteAsync(ResendOtpDto dto)
        {
            try
            {
                // Validate that the booking exists
                var booking = await _unitOfWork.Repository<Booking>().GetByIdAsync(dto.BookingId);
                if (booking == null)
                    return Result<string>.Failure("Booking not found.", 404);

                // Check if booking is in the correct status for verification
                if (booking.Status != Domain.Enums.BookingStatus.EmailVerificationPending)
                    return Result<string>.Failure("Booking is not pending email verification.", 400);

                // Validate that the email address matches the booking
                if (!string.Equals(booking.EmailAddress, dto.EmailAddress, StringComparison.OrdinalIgnoreCase))
                    return Result<string>.Failure("Email address does not match the booking.", 400);

                // Check if an OTP already exists and hasn't expired
                var otpExists = await _otpService.OtpExistsAsync(dto.BookingId, dto.EmailAddress);
                if (otpExists)
                {
                    // Check attempts to prevent abuse
                    var attempts = await _otpService.GetOtpAttemptsAsync(dto.BookingId, dto.EmailAddress);
                    if (attempts >= 3) // Allow max 3 resends
                        return Result<string>.Failure("Maximum OTP resend attempts exceeded. Please try again later.", 400);
                }

                // Generate a new OTP code
                var otpCode = await _otpService.GenerateOtpAsync(dto.BookingId, dto.EmailAddress);

                // Send the OTP email
                var emailSent = await _emailService.SendOtpVerificationAsync(dto.EmailAddress, booking.GuestName, otpCode, dto.BookingId);
                if (!emailSent)
                    return Result<string>.Failure("Failed to send OTP email. Please try again.", 500);

                // Return success result
                return Result<string>.Success("OTP code sent successfully.", "OTP code has been sent to your email address.");
            }
            catch (Exception ex)
            {
                return Result<string>.Failure($"Failed to resend OTP: {ex.Message}", 500);
            }
        }
    }
} 