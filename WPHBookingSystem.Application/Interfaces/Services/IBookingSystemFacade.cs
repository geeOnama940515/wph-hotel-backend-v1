using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WPHBookingSystem.Application.DTOs.Booking;
using WPHBookingSystem.Application.DTOs.Room;
using WPHBookingSystem.Application.UseCases.Bookings;
using WPHBookingSystem.Application.UseCases.Rooms;

namespace WPHBookingSystem.Application.Interfaces.Services
{
    public interface IBookingSystemFacade
    {
        // Booking operations
        Task<BookingCreatedDto> CreateBooking(CreateBookingDto dto, Guid userId);
        Task<BookingDto> UpdateBooking(Guid bookingId, UpdateBookingDateDto dto);
        Task<BookingDto> UpdateBookingStatus(UpdateBookingStatusRequest request);
        Task<BookingDto> CancelBooking(Guid bookingId);
        Task<IEnumerable<BookingDto>> GetUserBookings(Guid userId);
        Task<BookingDto> ViewBookingByToken(Guid bookingToken);

        // Room operations
        Task<Guid> CreateRoom(CreateRoomDto dto);
        Task<RoomDto> UpdateRoom(UpdateRoomDto dto);
        Task<RoomDto> UpdateRoomStatus(Guid roomId, bool isAvailable);
        Task<RoomDto> GetRoomById(Guid roomId);
        Task<IEnumerable<RoomDto>> GetAllRooms();
        Task<bool> DeleteRoom(Guid roomId);
        Task<bool> CheckRoomAvailability(Guid roomId, DateTime startDate, DateTime endDate);
        Task<decimal> GetRoomOccupancyRate(Guid roomId, DateTime startDate, DateTime endDate);
        Task<decimal> GetRoomRevenue(Guid roomId, DateTime startDate, DateTime endDate);
    }
} 