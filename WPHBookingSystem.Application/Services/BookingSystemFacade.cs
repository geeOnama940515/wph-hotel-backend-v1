using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WPHBookingSystem.Application.Common;
using WPHBookingSystem.Application.DTOs.Booking;
using WPHBookingSystem.Application.DTOs.Room;
using WPHBookingSystem.Application.Interfaces.Services;
using WPHBookingSystem.Application.UseCases.Bookings;
using WPHBookingSystem.Application.UseCases.Rooms;
using static WPHBookingSystem.Application.UseCases.Rooms.CheckRoomAvailabilityUseCase;
using WPHBookingSystem.Application.DTOs.ContactMessage;
using WPHBookingSystem.Application.UseCases.ContactMessages;

namespace WPHBookingSystem.Application.Services
{
    /// <summary>
    /// Implementation of the booking system facade that provides a simplified interface
    /// to the complex booking system operations. This class orchestrates various use cases
    /// and provides a unified entry point for all booking and room management functionality.
    /// 
    /// The facade pattern implementation here:
    /// - Simplifies the API for the presentation layer
    /// - Coordinates multiple use cases when needed
    /// - Provides consistent error handling and response formatting
    /// - Reduces coupling between the presentation layer and individual use cases
    /// </summary>
    public class BookingSystemFacade : IBookingSystemFacade
    {
        #region Private Fields - Use Case Dependencies

        // Booking-related use cases
        private readonly CreateBookingUseCase _createBookingUseCase;
        private readonly UpdateBookingDatesUseCase _updateBookingUseCase;
        private readonly UpdateBookingStatusUseCase _updateBookingStatusUseCase;
        private readonly CancelBookingUseCase _cancelBookingUseCase;
        private readonly GetUserBookingsUseCase _getUserBookingsUseCase;
        private readonly GetAllBookingsUseCase _getAllBookingsUseCase;
        private readonly ViewBookingByTokenUseCase _viewBookingByTokenUseCase;

        // Room-related use cases
        private readonly CreateRoomUseCase _createRoomUseCase;
        private readonly UpdateRoomUseCase _updateRoomUseCase;
        private readonly UpdateRoomWithImagesUseCase _updateRoomWithImagesUseCase;
        private readonly UpdateRoomStatusUseCase _updateRoomStatusUseCase;
        private readonly GetRoomByIdUseCase _getRoomByIdUseCase;
        private readonly GetAllRoomsUseCase _getAllRoomsUseCase;
        private readonly DeleteRoomUseCase _deleteRoomUseCase;
        private readonly CheckRoomAvailabilityUseCase _checkRoomAvailabilityUseCase;
        private readonly GetRoomOccupancyRateUseCase _getRoomOccupancyRateUseCase;
        private readonly GetRoomRevenueUseCase _getRoomRevenueUseCase;
        private readonly UploadRoomImagesUseCase _uploadRoomImagesUseCase;

        // Contact message use cases
        private readonly CreateContactMessageUseCase _createContactMessageUseCase;
        private readonly GetAllContactMessagesUseCase _getAllContactMessagesUseCase;
        private readonly GetContactMessageByIdUseCase _getContactMessageByIdUseCase;
        private readonly UpdateContactMessageUseCase _updateContactMessageUseCase;
        private readonly DeleteContactMessageUseCase _deleteContactMessageUseCase;
        private readonly IEmailSenderService _emailSenderService;

        // OTP verification use cases
        private readonly VerifyBookingOtpUseCase _verifyBookingOtpUseCase;
        private readonly ResendOtpUseCase _resendOtpUseCase;

        #endregion

        /// <summary>
        /// Initializes a new instance of the BookingSystemFacade with all required use case dependencies.
        /// This constructor uses dependency injection to receive all the use cases it needs to orchestrate.
        /// </summary>
        /// <param name="createBookingUseCase">Use case for creating new bookings.</param>
        /// <param name="updateBookingUseCase">Use case for updating booking dates.</param>
        /// <param name="updateBookingStatusUseCase">Use case for updating booking status.</param>
        /// <param name="cancelBookingUseCase">Use case for cancelling bookings.</param>
        /// <param name="getUserBookingsUseCase">Use case for retrieving user bookings.</param>
        /// <param name="getAllBookingsUseCase">Use case for retrieving all bookings.</param>
        /// <param name="createRoomUseCase">Use case for creating new rooms.</param>
        /// <param name="updateRoomUseCase">Use case for updating room details.</param>
        /// <param name="updateRoomStatusUseCase">Use case for updating room status.</param>
        /// <param name="getRoomByIdUseCase">Use case for retrieving room by ID.</param>
        /// <param name="getAllRoomsUseCase">Use case for retrieving all rooms.</param>
        /// <param name="deleteRoomUseCase">Use case for deleting rooms.</param>
        /// <param name="checkRoomAvailabilityUseCase">Use case for checking room availability.</param>
        /// <param name="getRoomOccupancyRateUseCase">Use case for calculating room occupancy rate.</param>
        /// <param name="getRoomRevenueUseCase">Use case for calculating room revenue.</param>
        /// <param name="viewBookingByTokenUseCase">Use case for viewing booking by token.</param>
        /// <param name="uploadRoomImagesUseCase">Use case for uploading room images.</param>
        /// <param name="createContactMessageUseCase">Use case for creating a new contact message.</param>
        /// <param name="getAllContactMessagesUseCase">Use case for retrieving all contact messages.</param>
        /// <param name="getContactMessageByIdUseCase">Use case for retrieving a contact message by ID.</param>
        /// <param name="updateContactMessageUseCase">Use case for updating a contact message.</param>
        /// <param name="deleteContactMessageUseCase">Use case for deleting a contact message.</param>
        /// <param name="updateRoomWithImagesUseCase">Use case for updating room images.</param>
        /// <param name="emailSenderService">Service for sending email replies.</param>
        /// <param name="verifyBookingOtpUseCase">Use case for verifying booking OTP.</param>
        /// <param name="resendOtpUseCase">Use case for resending OTP.</param>
        public BookingSystemFacade(
            CreateBookingUseCase createBookingUseCase,
            UpdateBookingDatesUseCase updateBookingUseCase,
            UpdateBookingStatusUseCase updateBookingStatusUseCase,
            CancelBookingUseCase cancelBookingUseCase,
            GetUserBookingsUseCase getUserBookingsUseCase,
            GetAllBookingsUseCase getAllBookingsUseCase,
            CreateRoomUseCase createRoomUseCase,
            UpdateRoomUseCase updateRoomUseCase,
            UpdateRoomWithImagesUseCase updateRoomWithImagesUseCase,
            UpdateRoomStatusUseCase updateRoomStatusUseCase,
            GetRoomByIdUseCase getRoomByIdUseCase,
            GetAllRoomsUseCase getAllRoomsUseCase,
            DeleteRoomUseCase deleteRoomUseCase,
            CheckRoomAvailabilityUseCase checkRoomAvailabilityUseCase,
            GetRoomOccupancyRateUseCase getRoomOccupancyRateUseCase,
            GetRoomRevenueUseCase getRoomRevenueUseCase,
            ViewBookingByTokenUseCase viewBookingByTokenUseCase,
            UploadRoomImagesUseCase uploadRoomImagesUseCase,
            CreateContactMessageUseCase createContactMessageUseCase,
            GetAllContactMessagesUseCase getAllContactMessagesUseCase,
            GetContactMessageByIdUseCase getContactMessageByIdUseCase,
            UpdateContactMessageUseCase updateContactMessageUseCase,
            DeleteContactMessageUseCase deleteContactMessageUseCase,
            IEmailSenderService emailSenderService,
            VerifyBookingOtpUseCase verifyBookingOtpUseCase,
            ResendOtpUseCase resendOtpUseCase)
        {
            _createBookingUseCase = createBookingUseCase;
            _updateBookingUseCase = updateBookingUseCase;
            _updateBookingStatusUseCase = updateBookingStatusUseCase;
            _cancelBookingUseCase = cancelBookingUseCase;
            _getUserBookingsUseCase = getUserBookingsUseCase;
            _getAllBookingsUseCase = getAllBookingsUseCase;
            _createRoomUseCase = createRoomUseCase;
            _updateRoomUseCase = updateRoomUseCase;
            _updateRoomWithImagesUseCase = updateRoomWithImagesUseCase;
            _updateRoomStatusUseCase = updateRoomStatusUseCase;
            _getRoomByIdUseCase = getRoomByIdUseCase;
            _getAllRoomsUseCase = getAllRoomsUseCase;
            _deleteRoomUseCase = deleteRoomUseCase;
            _checkRoomAvailabilityUseCase = checkRoomAvailabilityUseCase;
            _getRoomOccupancyRateUseCase = getRoomOccupancyRateUseCase;
            _getRoomRevenueUseCase = getRoomRevenueUseCase;
            _viewBookingByTokenUseCase = viewBookingByTokenUseCase;
            _uploadRoomImagesUseCase = uploadRoomImagesUseCase;
            _createContactMessageUseCase = createContactMessageUseCase;
            _getAllContactMessagesUseCase = getAllContactMessagesUseCase;
            _getContactMessageByIdUseCase = getContactMessageByIdUseCase;
            _updateContactMessageUseCase = updateContactMessageUseCase;
            _deleteContactMessageUseCase = deleteContactMessageUseCase;
            _emailSenderService = emailSenderService;
            _verifyBookingOtpUseCase = verifyBookingOtpUseCase;
            _resendOtpUseCase = resendOtpUseCase;
        }

        #region Booking Operations

        /// <summary>
        /// Creates a new booking by delegating to the CreateBookingUseCase.
        /// This method provides error handling and ensures consistent response formatting.
        /// </summary>
        /// <param name="dto">The booking creation data transfer object.</param>
        /// <returns>A result containing the created booking information or error details.</returns>
        public async Task<Result<BookingCreatedDto>> CreateBooking(CreateBookingDto dto)
        {
            return await _createBookingUseCase.ExecuteAsync(dto);
        }

        /// <summary>
        /// Updates booking dates by delegating to the UpdateBookingDatesUseCase.
        /// This method provides error handling and ensures consistent response formatting.
        /// </summary>
        /// <param name="bookingId">The unique identifier of the booking to update.</param>
        /// <param name="dto">The data transfer object containing new dates.</param>
        /// <returns>A result containing the updated booking information or error details.</returns>
        public async Task<Result<BookingDto>> UpdateBooking(Guid bookingId, UpdateBookingDateDto dto)
        {
            return await _updateBookingUseCase.ExecuteAsync(bookingId, dto);
        }

        /// <summary>
        /// Updates booking status by delegating to the UpdateBookingStatusUseCase.
        /// </summary>
        /// <param name="request">The request containing booking ID and new status.</param>
        /// <returns>A result containing the updated booking information or error details.</returns>
        public async Task<Result<BookingDto>> UpdateBookingStatus(UpdateBookingStatusRequest request)
        {
            return await _updateBookingStatusUseCase.ExecuteAsync(request);
        }

        /// <summary>
        /// Cancels a booking by delegating to the CancelBookingUseCase.
        /// </summary>
        /// <param name="bookingId">The unique identifier of the booking to cancel.</param>
        /// <returns>A result containing the cancelled booking information or error details.</returns>
        public async Task<Result<BookingDto>> CancelBooking(Guid bookingId)
        {
            return await _cancelBookingUseCase.ExecuteAsync(bookingId);
        }

        /// <summary>
        /// Retrieves user bookings by delegating to the GetUserBookingsUseCase.
        /// </summary>
        /// <param name="emailAddress">The email address of the user.</param>
        /// <returns>A result containing the list of user bookings or error details.</returns>
        public async Task<Result<List<BookingDto>>> GetUserBookings(string emailAddress)
        {
            return await _getUserBookingsUseCase.ExecuteAsync(emailAddress);
        }

        /// <summary>
        /// Retrieves all bookings by delegating to the GetAllBookingsUseCase.
        /// </summary>
        /// <returns>A result containing the list of all bookings or error details.</returns>
        public async Task<Result<List<BookingDto>>> GetAllBookings()
        {
            return await _getAllBookingsUseCase.ExecuteAsync();
        }

        #endregion

        #region Room Operations

        /// <summary>
        /// Creates a new room by delegating to the CreateRoomUseCase.
        /// </summary>
        /// <param name="dto">The data transfer object containing room creation information.</param>
        /// <returns>A result containing the ID of the created room or error details.</returns>
        public async Task<Result<Guid>> CreateRoom(CreateRoomDto dto)
        {
            return await _createRoomUseCase.ExecuteAsync(dto);
        }

        /// <summary>
        /// Creates a new room with images by delegating to the CreateRoomUseCase.
        /// </summary>
        /// <param name="dto">The data transfer object containing room creation information.</param>
        /// <param name="images">Optional collection of image files to upload with the room.</param>
        /// <returns>A result containing the ID of the created room or error details.</returns>
        public async Task<Result<Guid>> CreateRoom(CreateRoomDto dto, IFormFileCollection? images = null)
        {
            return await _createRoomUseCase.ExecuteAsync(dto, images);
        }

        /// <summary>
        /// Updates room details by delegating to the UpdateRoomUseCase.
        /// </summary>
        /// <param name="roomId">The unique identifier of the room to update.</param>
        /// <param name="dto">The data transfer object containing updated room information.</param>
        /// <returns>A result containing the updated room information or error details.</returns>
        public async Task<Result<RoomDto>> UpdateRoom(Guid roomId, UpdateRoomDto dto)
        {
            return await _updateRoomUseCase.ExecuteAsync(roomId, dto);
        }

        /// <summary>
        /// Updates room details with new images by delegating to the UpdateRoomWithImagesUseCase.
        /// </summary>
        /// <param name="roomId">The unique identifier of the room to update.</param>
        /// <param name="dto">The data transfer object containing updated room information and new images.</param>
        /// <returns>A result containing the updated room information or error details.</returns>
        public async Task<Result<RoomDto>> UpdateRoomWithImages(Guid roomId, UpdateRoomWithImagesDto dto)
        {
            return await _updateRoomWithImagesUseCase.ExecuteAsync(roomId, dto);
        }

        /// <summary>
        /// Updates room status by delegating to the UpdateRoomStatusUseCase.
        /// </summary>
        /// <param name="request">The request containing room ID and new status.</param>
        /// <returns>A result containing the updated room information or error details.</returns>
        public async Task<Result<RoomDto>> UpdateRoomStatus(UpdateRoomStatusRequest request)
        {
            return await _updateRoomStatusUseCase.ExecuteAsync(request);
        }

        /// <summary>
        /// Retrieves room by ID by delegating to the GetRoomByIdUseCase.
        /// </summary>
        /// <param name="roomId">The unique identifier of the room to retrieve.</param>
        /// <returns>A result containing the room information or error details.</returns>
        public async Task<Result<RoomDto>> GetRoomById(Guid roomId)
        {
            return await _getRoomByIdUseCase.ExecuteAsync(roomId);
        }

        /// <summary>
        /// Retrieves all rooms by delegating to the GetAllRoomsUseCase.
        /// </summary>
        /// <returns>A result containing the list of all rooms or error details.</returns>
        public async Task<Result<List<RoomDto>>> GetAllRooms()
        {
            return await _getAllRoomsUseCase.ExecuteAsync();
        }

        /// <summary>
        /// Deletes a room by delegating to the DeleteRoomUseCase.
        /// </summary>
        /// <param name="roomId">The unique identifier of the room to delete.</param>
        /// <returns>A result indicating success or failure of the deletion operation.</returns>
        public async Task<Result<bool>> DeleteRoom(Guid roomId)
        {
            return await _deleteRoomUseCase.ExecuteAsync(roomId);
        }

        /// <summary>
        /// Checks room availability by delegating to the CheckRoomAvailabilityUseCase.
        /// </summary>
        /// <param name="request">The request containing room ID and date range to check.</param>
        /// <returns>A result containing availability information or error details.</returns>
        public async Task<Result<CheckRoomAvailabilityResponse>> CheckRoomAvailability(CheckRoomAvailabilityRequest request)
        {
            return await _checkRoomAvailabilityUseCase.ExecuteAsync(request);
        }

        /// <summary>
        /// Calculates room occupancy rate by delegating to the GetRoomOccupancyRateUseCase.
        /// </summary>
        /// <param name="request">The request containing room ID and date range for calculation.</param>
        /// <returns>A result containing the occupancy rate percentage or error details.</returns>
        public async Task<Result<GetRoomOccupancyRateResponse>> GetRoomOccupancyRate(GetRoomOccupancyRateRequest request)
        {
            return await _getRoomOccupancyRateUseCase.ExecuteAsync(request);
        }

        /// <summary>
        /// Calculates room revenue by delegating to the GetRoomRevenueUseCase.
        /// </summary>
        /// <param name="request">The request containing room ID and date range for calculation.</param>
        /// <returns>A result containing the revenue amount or error details.</returns>
        public async Task<Result<GetRoomRevenueResponse>> GetRoomRevenue(GetRoomRevenueRequest request)
        {
            return await _getRoomRevenueUseCase.ExecuteAsync(request);
        }

        /// <summary>
        /// Views booking by token by delegating to the ViewBookingByTokenUseCase.
        /// This method provides error handling and ensures consistent response formatting.
        /// </summary>
        /// <param name="bookingToken">The unique booking token for the reservation.</param>
        /// <returns>A result containing the booking information or error details.</returns>
        /// <exception cref="ApplicationException">Thrown when the use case execution fails.</exception>
        public async Task<Result<BookingDto>> ViewBookingByToken(Guid bookingToken)
        {
            var result = await _viewBookingByTokenUseCase.ExecuteAsync(bookingToken);
            if (!result.IsSuccess)
                throw new ApplicationException(result.Message);
            return result;
        }

        /// <summary>
        /// Uploads multiple images to a room by delegating to the UploadRoomImagesUseCase.
        /// </summary>
        /// <param name="roomId">The unique identifier of the room to upload images for.</param>
        /// <param name="files">Collection of image files to upload.</param>
        /// <returns>A result containing upload information and any errors.</returns>
        public async Task<Result<ImageUploadResponseDto>> UploadRoomImages(Guid roomId, IFormFileCollection files)
        {
            return await _uploadRoomImagesUseCase.ExecuteAsync(roomId, files);
        }

        #endregion

        #region OTP Verification Operations

        /// <summary>
        /// Verifies booking OTP by delegating to the VerifyBookingOtpUseCase.
        /// </summary>
        /// <param name="dto">The data transfer object containing booking ID and OTP code.</param>
        /// <returns>A result containing the verified booking information or error details.</returns>
        public async Task<Result<BookingDto>> VerifyBookingOtp(BookingVerificationDto dto)
        {
            return await _verifyBookingOtpUseCase.ExecuteAsync(dto);
        }

        /// <summary>
        /// Resends OTP by delegating to the ResendOtpUseCase.
        /// </summary>
        /// <param name="dto">The data transfer object containing booking ID and email address.</param>
        /// <returns>A result indicating success or failure of the OTP resend operation.</returns>
        public async Task<Result<string>> ResendOtp(ResendOtpDto dto)
        {
            return await _resendOtpUseCase.ExecuteAsync(dto);
        }

        #endregion

        #region Contact Message Operations

        /// <summary>
        /// Creates a new contact message by delegating to the CreateContactMessageUseCase.
        /// </summary>
        /// <param name="dto">The data transfer object containing contact message creation information.</param>
        /// <returns>A result containing the ID of the created contact message or error details.</returns>
        public async Task<Result<Guid>> CreateContactMessage(CreateContactMessageDto dto)
        {
            return await _createContactMessageUseCase.ExecuteAsync(dto);
        }

        /// <summary>
        /// Retrieves all contact messages by delegating to the GetAllContactMessagesUseCase.
        /// </summary>
        /// <returns>A result containing the list of all contact messages or error details.</returns>
        public async Task<Result<List<ContactMessageDto>>> GetAllContactMessages()
        {
            var data = await _getAllContactMessagesUseCase.ExecuteAsync();
            return Result<List<ContactMessageDto>>.Success(data);
        }

        /// <summary>
        /// Retrieves a contact message by ID by delegating to the GetContactMessageByIdUseCase.
        /// </summary>
        /// <param name="id">The unique identifier of the contact message to retrieve.</param>
        /// <returns>A result containing the contact message information or error details.</returns>
        public async Task<Result<ContactMessageDto>> GetContactMessageById(Guid id)
        {
            var data = await _getContactMessageByIdUseCase.ExecuteAsync(id);
            if (data == null) return Result<ContactMessageDto>.Failure("Not found", 404);
            return Result<ContactMessageDto>.Success(data);
        }

        /// <summary>
        /// Updates a contact message by delegating to the UpdateContactMessageUseCase.
        /// </summary>
        /// <param name="id">The unique identifier of the contact message to update.</param>
        /// <param name="dto">The data transfer object containing updated contact message information.</param>
        /// <returns>A result indicating success or failure of the update operation.</returns>
        public async Task<Result> UpdateContactMessage(Guid id, UpdateContactMessageDto dto)
        {
            return await _updateContactMessageUseCase.ExecuteAsync(id, dto);
        }

        /// <summary>
        /// Deletes a contact message by delegating to the DeleteContactMessageUseCase.
        /// </summary>
        /// <param name="id">The unique identifier of the contact message to delete.</param>
        /// <returns>A result indicating success or failure of the deletion operation.</returns>
        public async Task<Result> DeleteContactMessage(Guid id)
        {
            return await _deleteContactMessageUseCase.ExecuteAsync(id);
        }

        /// <summary>
        /// Replies to a contact message by delegating to the UpdateContactMessageUseCase.
        /// </summary>
        /// <param name="subject">The subject of the reply.</param>
        /// <param name="email">The email address of the contact message recipient.</param>
        /// <param name="body">The body of the reply.</param>
        /// <returns>A result indicating success or failure of the reply operation.</returns>
        public async Task<Result> ReplyToContactMessage(string subject, string email, string body)
        {
            // Find the original message by email (get the latest one)
            var allMessages = await _getAllContactMessagesUseCase.ExecuteAsync();
            var original = allMessages.FirstOrDefault(m => m.EmailAddress == email);
            if (original == null)
                return Result.Failure("Original message not found for this email.", 404);
            var sent = await _emailSenderService.SendContactMessageReplyAsync(email, original.Fullname, body, original.Message);
            if (!sent)
                return Result.Failure("Failed to send reply email.");
            return Result.Success("Reply sent successfully.");
        }

        #endregion
    }
} 