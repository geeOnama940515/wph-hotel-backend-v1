using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WPHBookingSystem.Application.Common;
using WPHBookingSystem.Application.DTOs.Booking;
using WPHBookingSystem.Application.DTOs.Room;
using WPHBookingSystem.Application.UseCases.Bookings;
using WPHBookingSystem.Application.UseCases.Rooms;
using static WPHBookingSystem.Application.UseCases.Rooms.CheckRoomAvailabilityUseCase;

namespace WPHBookingSystem.Application.Interfaces.Services
{
    /// <summary>
    /// Facade interface that provides a simplified interface to the complex booking system subsystem.
    /// This interface acts as a single entry point for all booking and room management operations,
    /// hiding the complexity of individual use cases and providing a clean API for the presentation layer.
    /// 
    /// The facade pattern is used here to:
    /// - Simplify the interface between the presentation layer and the application layer
    /// - Reduce coupling between the client code and the subsystem
    /// - Provide a unified interface for related operations
    /// </summary>
    public interface IBookingSystemFacade
    {
        #region Booking Operations

        /// <summary>
        /// Creates a new booking for a room with the specified details.
        /// This operation validates room availability, calculates pricing, and creates the booking.
        /// </summary>
        /// <param name="dto">The booking creation data transfer object containing all necessary booking information.</param>
        /// <returns>A result containing the created booking information or error details.</returns>
        Task<Result<BookingCreatedDto>> CreateBooking(CreateBookingDto dto);

        /// <summary>
        /// Updates the check-in and check-out dates for an existing booking.
        /// This operation validates the new dates and ensures room availability.
        /// </summary>
        /// <param name="bookingId">The unique identifier of the booking to update.</param>
        /// <param name="dto">The data transfer object containing the new check-in and check-out dates.</param>
        /// <returns>A result containing the updated booking information or error details.</returns>
        Task<Result<BookingDto>> UpdateBooking(Guid bookingId, UpdateBookingDateDto dto);

        /// <summary>
        /// Updates the status of an existing booking (e.g., confirm, cancel, complete).
        /// This operation enforces business rules for status transitions.
        /// </summary>
        /// <param name="request">The request containing the booking ID and new status.</param>
        /// <returns>A result containing the updated booking information or error details.</returns>
        Task<Result<BookingDto>> UpdateBookingStatus(UpdateBookingStatusRequest request);

        /// <summary>
        /// Cancels an existing booking, making the room available for other bookings.
        /// This operation validates that the booking can be cancelled based on business rules.
        /// </summary>
        /// <param name="bookingId">The unique identifier of the booking to cancel.</param>
        /// <returns>A result containing the cancelled booking information or error details.</returns>
        Task<Result<BookingDto>> CancelBooking(Guid bookingId);

        /// <summary>
        /// Retrieves all bookings for a specific user by their email address.
        /// This operation provides a user-centric view of their booking history.
        /// </summary>
        /// <param name="emailAddress">The email address of the user whose bookings to retrieve.</param>
        /// <returns>A result containing the list of user bookings or error details.</returns>
        Task<Result<List<BookingDto>>> GetUserBookings(string emailAddress);

        /// <summary>
        /// Retrieves booking information using a booking token.
        /// This allows guests to view their booking details without authentication.
        /// </summary>
        /// <param name="bookingToken">The unique booking token for the reservation.</param>
        /// <returns>A result containing the booking information or error details.</returns>
        Task<Result<BookingDto>> ViewBookingByToken(Guid bookingToken);

        #endregion

        #region Room Operations

        /// <summary>
        /// Creates a new room with the specified details.
        /// This operation validates room information and creates the room entity.
        /// </summary>
        /// <param name="dto">The data transfer object containing all room creation information.</param>
        /// <returns>A result containing the ID of the created room or error details.</returns>
        Task<Result<Guid>> CreateRoom(CreateRoomDto dto);

        /// <summary>
        /// Updates the details of an existing room.
        /// This operation validates the updated information and applies the changes.
        /// </summary>
        /// <param name="roomId">The unique identifier of the room to update.</param>
        /// <param name="dto">The data transfer object containing the updated room information.</param>
        /// <returns>A result containing the updated room information or error details.</returns>
        Task<Result<RoomDto>> UpdateRoom(Guid roomId, UpdateRoomDto dto);

        /// <summary>
        /// Updates the operational status of a room (e.g., available, maintenance, inactive).
        /// This operation enforces business rules for status changes.
        /// </summary>
        /// <param name="request">The request containing the room ID and new status.</param>
        /// <returns>A result containing the updated room information or error details.</returns>
        Task<Result<RoomDto>> UpdateRoomStatus(UpdateRoomStatusRequest request);

        /// <summary>
        /// Retrieves detailed information about a specific room by its ID.
        /// </summary>
        /// <param name="roomId">The unique identifier of the room to retrieve.</param>
        /// <returns>A result containing the room information or error details.</returns>
        Task<Result<RoomDto>> GetRoomById(Guid roomId);

        /// <summary>
        /// Retrieves all available rooms in the system.
        /// This operation provides a comprehensive view of all rooms for browsing.
        /// </summary>
        /// <returns>A result containing the list of all rooms or error details.</returns>
        Task<Result<List<RoomDto>>> GetAllRooms();

        /// <summary>
        /// Deletes a room from the system.
        /// This operation validates that the room can be safely deleted.
        /// </summary>
        /// <param name="roomId">The unique identifier of the room to delete.</param>
        /// <returns>A result indicating success or failure of the deletion operation.</returns>
        Task<Result<bool>> DeleteRoom(Guid roomId);

        /// <summary>
        /// Checks if a room is available for booking during the specified date range.
        /// This operation considers existing bookings and room status.
        /// </summary>
        /// <param name="request">The request containing room ID and date range to check.</param>
        /// <returns>A result containing availability information or error details.</returns>
        Task<Result<CheckRoomAvailabilityResponse>> CheckRoomAvailability(CheckRoomAvailabilityRequest request);

        /// <summary>
        /// Calculates the occupancy rate for a room during the specified date range.
        /// This operation provides business intelligence for room utilization.
        /// </summary>
        /// <param name="request">The request containing room ID and date range for calculation.</param>
        /// <returns>A result containing the occupancy rate percentage or error details.</returns>
        Task<Result<GetRoomOccupancyRateResponse>> GetRoomOccupancyRate(GetRoomOccupancyRateRequest request);

        /// <summary>
        /// Calculates the total revenue generated by a room during the specified date range.
        /// This operation provides business intelligence for financial reporting.
        /// </summary>
        /// <param name="request">The request containing room ID and date range for calculation.</param>
        /// <returns>A result containing the revenue amount or error details.</returns>
        Task<Result<GetRoomRevenueResponse>> GetRoomRevenue(GetRoomRevenueRequest request);

        #endregion
    }
} 