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
        private readonly ViewBookingByTokenUseCase _viewBookingByTokenUseCase;

        // Room-related use cases
        private readonly CreateRoomUseCase _createRoomUseCase;
        private readonly UpdateRoomUseCase _updateRoomUseCase;
        private readonly UpdateRoomStatusUseCase _updateRoomStatusUseCase;
        private readonly GetRoomByIdUseCase _getRoomByIdUseCase;
        private readonly GetAllRoomsUseCase _getAllRoomsUseCase;
        private readonly DeleteRoomUseCase _deleteRoomUseCase;
        private readonly CheckRoomAvailabilityUseCase _checkRoomAvailabilityUseCase;
        private readonly GetRoomOccupancyRateUseCase _getRoomOccupancyRateUseCase;
        private readonly GetRoomRevenueUseCase _getRoomRevenueUseCase;

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
        public BookingSystemFacade(
            CreateBookingUseCase createBookingUseCase,
            UpdateBookingDatesUseCase updateBookingUseCase,
            UpdateBookingStatusUseCase updateBookingStatusUseCase,
            CancelBookingUseCase cancelBookingUseCase,
            GetUserBookingsUseCase getUserBookingsUseCase,
            CreateRoomUseCase createRoomUseCase,
            UpdateRoomUseCase updateRoomUseCase,
            UpdateRoomStatusUseCase updateRoomStatusUseCase,
            GetRoomByIdUseCase getRoomByIdUseCase,
            GetAllRoomsUseCase getAllRoomsUseCase,
            DeleteRoomUseCase deleteRoomUseCase,
            CheckRoomAvailabilityUseCase checkRoomAvailabilityUseCase,
            GetRoomOccupancyRateUseCase getRoomOccupancyRateUseCase,
            GetRoomRevenueUseCase getRoomRevenueUseCase,
            ViewBookingByTokenUseCase viewBookingByTokenUseCase)
        {
            _createBookingUseCase = createBookingUseCase;
            _updateBookingUseCase = updateBookingUseCase;
            _updateBookingStatusUseCase = updateBookingStatusUseCase;
            _cancelBookingUseCase = cancelBookingUseCase;
            _getUserBookingsUseCase = getUserBookingsUseCase;
            _createRoomUseCase = createRoomUseCase;
            _updateRoomUseCase = updateRoomUseCase;
            _updateRoomStatusUseCase = updateRoomStatusUseCase;
            _getRoomByIdUseCase = getRoomByIdUseCase;
            _getAllRoomsUseCase = getAllRoomsUseCase;
            _deleteRoomUseCase = deleteRoomUseCase;
            _checkRoomAvailabilityUseCase = checkRoomAvailabilityUseCase;
            _getRoomOccupancyRateUseCase = getRoomOccupancyRateUseCase;
            _getRoomRevenueUseCase = getRoomRevenueUseCase;
            _viewBookingByTokenUseCase = viewBookingByTokenUseCase;
        }

        #region Booking Operations

        /// <summary>
        /// Creates a new booking by delegating to the CreateBookingUseCase.
        /// This method provides error handling and ensures consistent response formatting.
        /// </summary>
        /// <param name="dto">The booking creation data transfer object.</param>
        /// <returns>A result containing the created booking information or error details.</returns>
        /// <exception cref="ApplicationException">Thrown when the use case execution fails.</exception>
        public async Task<Result<BookingCreatedDto>> CreateBooking(CreateBookingDto dto)
        {
            var result = await _createBookingUseCase.ExecuteAsync(dto);
            if (!result.IsSuccess)
                throw new ApplicationException(result.Message);
            return result;
        }

        /// <summary>
        /// Updates booking dates by delegating to the UpdateBookingDatesUseCase.
        /// This method provides error handling and ensures consistent response formatting.
        /// </summary>
        /// <param name="bookingId">The unique identifier of the booking to update.</param>
        /// <param name="dto">The data transfer object containing new dates.</param>
        /// <returns>A result containing the updated booking information or error details.</returns>
        /// <exception cref="ApplicationException">Thrown when the use case execution fails.</exception>
        public async Task<Result<BookingDto>> UpdateBooking(Guid bookingId, UpdateBookingDateDto dto)
        {
            var result = await _updateBookingUseCase.ExecuteAsync(bookingId, dto);
            if (!result.IsSuccess)
                throw new ApplicationException(result.Message);
            return result;
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

        #endregion
    }
} 