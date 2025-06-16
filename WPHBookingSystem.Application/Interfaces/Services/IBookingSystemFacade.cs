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
    public interface IBookingSystemFacade
    {
        // Booking operations
        Task<BookingCreatedDto> CreateBooking(CreateBookingDto dto, Guid userId);
        Task<BookingDto> UpdateBooking(Guid bookingId, UpdateBookingDateDto dto);
        Task<Result<BookingDto>> UpdateBookingStatus(UpdateBookingStatusRequest request);
        Task<Result<BookingDto>> CancelBooking(Guid bookingId, string emailAddress);
        Task<Result<List<BookingDto>>> GetUserBookings(string emailAddress);
        Task<BookingDto> ViewBookingByToken(Guid bookingToken);

        // Room operations
        Task<Result<Guid>> CreateRoom(CreateRoomDto dto);
        Task<Result<RoomDto>> UpdateRoom(Guid roomId, UpdateRoomDto dto);
        Task<Result<RoomDto>> UpdateRoomStatus(UpdateRoomStatusRequest request);
        Task<Result<RoomDto>> GetRoomById(Guid roomId);
        Task<Result<List<RoomDto>>> GetAllRooms();
        Task<Result<bool>> DeleteRoom(Guid roomId);
        Task<Result<CheckRoomAvailabilityResponse>> CheckRoomAvailability(CheckRoomAvailabilityRequest request);
        Task<Result<GetRoomOccupancyRateResponse>> GetRoomOccupancyRate(GetRoomOccupancyRateRequest request);
        Task<Result<GetRoomRevenueResponse>> GetRoomRevenue(GetRoomRevenueRequest request);
    }
} 