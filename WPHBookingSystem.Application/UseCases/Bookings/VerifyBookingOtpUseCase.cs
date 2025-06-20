using System;
using System.Threading.Tasks;
using WPHBookingSystem.Application.Common;
using WPHBookingSystem.Application.DTOs.Booking;
using WPHBookingSystem.Application.Interfaces;
using WPHBookingSystem.Application.Interfaces.Services;
using WPHBookingSystem.Domain.Entities;
using WPHBookingSystem.Domain.Exceptions;

namespace WPHBookingSystem.Application.UseCases.Bookings
{
    /// <summary>
    /// Use case responsible for verifying OTP codes for booking confirmation.
    /// This use case validates the provided OTP code and confirms the booking if valid.
    /// </summary>
    public class VerifyBookingOtpUseCase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOtpService _otpService;
        private readonly IEmailSenderService _emailService;

        /// <summary>
        /// Initializes a new instance of the VerifyBookingOtpUseCase with the required dependencies.
        /// </summary>
        /// <param name="unitOfWork">The unit of work for transaction management and data access.</param>
        /// <param name="otpService">The OTP service for validation.</param>
        /// <param name="emailService">The email service for sending confirmation emails.</param>
        /// <exception cref="ArgumentNullException">Thrown when any dependency is null.</exception>
        public VerifyBookingOtpUseCase(IUnitOfWork unitOfWork, IOtpService otpService, IEmailSenderService emailService)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _otpService = otpService ?? throw new ArgumentNullException(nameof(otpService));
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
        }

        /// <summary>
        /// Executes the OTP verification use case with the specified verification details.
        /// </summary>
        /// <param name="dto">The data transfer object containing booking ID and OTP code.</param>
        /// <returns>A result indicating success or failure of the verification.</returns>
        public async Task<Result<BookingDto>> ExecuteAsync(BookingVerificationDto dto)
        {
            try
            {
                // Begin transaction to ensure data consistency
                await _unitOfWork.BeginTransactionAsync();

                // Validate that the booking exists
                var booking = await _unitOfWork.Repository<Booking>().GetByIdAsync(dto.BookingId);
                if (booking == null)
                    return Result<BookingDto>.Failure("Booking not found.", 404);

                // Check if booking is in the correct status for verification
                if (booking.Status != Domain.Enums.BookingStatus.EmailVerificationPending)
                    return Result<BookingDto>.Failure("Booking is not pending email verification.", 400);

                // Validate the OTP code
                var isValidOtp = await _otpService.ValidateOtpAsync(dto.BookingId, dto.OtpCode, booking.EmailAddress);
                if (!isValidOtp)
                {
                    // Increment attempt counter
                    await _otpService.IncrementOtpAttemptsAsync(dto.BookingId, booking.EmailAddress);
                    return Result<BookingDto>.Failure("Invalid or expired OTP code.", 400);
                }

                // Check if maximum attempts exceeded
                var attempts = await _otpService.GetOtpAttemptsAsync(dto.BookingId, booking.EmailAddress);
                if (attempts >= 5)
                    return Result<BookingDto>.Failure("Maximum OTP attempts exceeded. Please request a new OTP.", 400);

                // Confirm the booking after successful verification
                booking.ConfirmAfterVerification();

                // Invalidate the OTP after successful verification
                await _otpService.InvalidateOtpAsync(dto.BookingId, booking.EmailAddress);

                // Update the booking in the database
                await _unitOfWork.Repository<Booking>().UpdateAsync(booking);

                // Commit the transaction
                await _unitOfWork.CommitTransactionAsync();

                // Get room information for the email
                var room = await _unitOfWork.Repository<Room>().GetByIdAsync(booking.RoomId);
                if (room == null)
                    return Result<BookingDto>.Failure("Room information not found.", 404);

                // Create BookingDto for email
                var bookingDto = new BookingDto
                {
                    Id = booking.Id,
                    GuestName = booking.GuestName,
                    RoomId = booking.RoomId,
                    CheckIn = booking.CheckIn,
                    CheckOut = booking.CheckOut,
                    Guests = booking.Guests,
                    TotalAmount = booking.TotalAmount,
                    Status = booking.Status,
                    SpecialRequests = booking.SpecialRequests,
                    Phone = booking.ContactInfo.Phone,
                    Address = booking.ContactInfo.Address,
                    RoomName = room.Name,
                    BookingToken = booking.BookingToken.ToString(),
                };

                // Send confirmation email
                await _emailService.SendBookingConfirmationAsync(bookingDto, booking.EmailAddress, booking.GuestName);

                // Return success result with booking information
                return Result<BookingDto>.Success(bookingDto, "Booking verified and confirmed successfully.");
            }
            catch (Exception ex)
            {
                // Rollback transaction on any error to maintain data consistency
                await _unitOfWork.RollbackTransactionAsync();
                return Result<BookingDto>.Failure($"Failed to verify booking: {ex.Message}", 500);
            }
        }
    }
} 