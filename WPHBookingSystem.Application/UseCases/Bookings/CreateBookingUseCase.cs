using System;
using System.Threading.Tasks;
using WPHBookingSystem.Application.Common;
using WPHBookingSystem.Application.DTOs.Booking;
using WPHBookingSystem.Application.Interfaces;
using WPHBookingSystem.Application.Interfaces.Services;
using WPHBookingSystem.Domain.Entities;
using WPHBookingSystem.Domain.Exceptions;
using WPHBookingSystem.Domain.ValueObjects;

namespace WPHBookingSystem.Application.UseCases.Bookings
{
    /// <summary>
    /// Use case responsible for creating new bookings in the hotel booking system.
    /// This use case implements the business logic for booking creation, including
    /// room availability validation, pricing calculation, and booking persistence.
    /// 
    /// The use case follows the Single Responsibility Principle by focusing solely
    /// on booking creation logic and delegates data persistence to the infrastructure layer.
    /// </summary>
    public class CreateBookingUseCase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailSenderService _emailService;
        private readonly IOtpService _otpService;

        /// <summary>
        /// Initializes a new instance of the CreateBookingUseCase with the required dependencies.
        /// </summary>
        /// <param name="unitOfWork">The unit of work for transaction management and data access.</param>
        /// <param name="emailService">The email service for sending confirmation emails.</param>
        /// <param name="otpService">The OTP service for generating verification codes.</param>
        /// <exception cref="ArgumentNullException">Thrown when unitOfWork, emailService, or otpService is null.</exception>
        public CreateBookingUseCase(IUnitOfWork unitOfWork, IEmailSenderService emailService, IOtpService otpService)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
            _otpService = otpService ?? throw new ArgumentNullException(nameof(otpService));
        }

        /// <summary>
        /// Executes the booking creation use case with the specified booking details.
        /// This method orchestrates the entire booking creation process including validation,
        /// business rule enforcement, and data persistence within a transaction.
        /// </summary>
        /// <param name="dto">The data transfer object containing all necessary booking information.</param>
        /// <returns>A result containing the created booking information or error details.</returns>
        public async Task<Result<BookingCreatedDto>> ExecuteAsync(CreateBookingDto dto)
        {
            try
            {
                // Begin transaction to ensure data consistency
                await _unitOfWork.BeginTransactionAsync();

                // Validate that the room exists
                var room = await _unitOfWork.Repository<Room>().GetByIdAsync(dto.RoomId);
                if (room == null)
                    return Result<BookingCreatedDto>.Failure("Room not found.", 404);

                // Validate room availability for the requested dates
                if (!room.IsAvailable(dto.CheckIn, dto.CheckOut))
                    return Result<BookingCreatedDto>.Failure("Room is not available on selected dates.", 400);

                // Create contact information value object
                var contactInfo = new ContactInfo(dto.Phone, dto.Address);

                // Calculate total amount based on room price and duration
                var totalAmount = room.Price * (decimal)(dto.CheckOut - dto.CheckIn).TotalDays;

                // Create the booking entity using the domain factory method
                var booking = Booking.Create(
                    room.Id,
                    dto.CheckIn,
                    dto.CheckOut,
                    dto.Guests,
                    totalAmount,
                    contactInfo,
                    dto.EmailAddress,
                    dto.SpecialRequests,
                    dto.GuestName
                );

                // Set booking status to EmailVerificationPending for OTP verification
                booking.SetEmailVerificationPending();

                // Persist the booking to the database
                await _unitOfWork.Repository<Booking>().AddAsync(booking);
                
                // Generate OTP code for email verification
                var otpCode = await _otpService.GenerateOtpAsync(booking.Id, dto.EmailAddress);

                // Send OTP verification email
                var otpEmailSent = await _emailService.SendOtpVerificationAsync(dto.EmailAddress, dto.GuestName, otpCode, booking.Id);
                if (!otpEmailSent)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    return Result<BookingCreatedDto>.Failure("Failed to send OTP verification email. Please try again.", 500);
                }

                // Commit the transaction
                await _unitOfWork.CommitTransactionAsync();

                // Return success result with booking information and verification status
                return Result<BookingCreatedDto>.Success(
                    new BookingCreatedDto 
                    { 
                        Id = booking.Id, 
                        BookingToken = booking.BookingToken,
                        RequiresEmailVerification = true,
                        EmailAddress = dto.EmailAddress
                    }, 
                    "Booking created successfully. Please check your email for verification code."
                );
            }
            catch (Exception ex)
            {
                // Rollback transaction on any error to maintain data consistency
                await _unitOfWork.RollbackTransactionAsync();
                return Result<BookingCreatedDto>.Failure($"Failed to create booking: {ex.Message}", 500);
            }
        }
    }
}
