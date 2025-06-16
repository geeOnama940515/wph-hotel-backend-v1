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
        Task<Guid> CreateBooking(CreateBookingDto dto, Guid userId);
        Task<BookingDto> UpdateBooking(UpdateBookingDateDto dto);
        Task<BookingDto> UpdateBookingStatus(UpdateBookingStatusRequest request);
        Task<BookingDto> CancelBooking(int bookingId);
        Task<IEnumerable<BookingDto>> GetUserBookings(int userId);

        // Room operations
        Task<Guid> CreateRoom(CreateRoomDto dto);
        Task<RoomDto> UpdateRoom(UpdateRoomDto dto);
        Task<RoomDto> UpdateRoomStatus(int roomId, bool isAvailable);
        Task<RoomDto> GetRoomById(int roomId);
        Task<IEnumerable<RoomDto>> GetAllRooms();
        Task<bool> DeleteRoom(int roomId);
        Task<bool> CheckRoomAvailability(int roomId, DateTime startDate, DateTime endDate);
        Task<decimal> GetRoomOccupancyRate(int roomId, DateTime startDate, DateTime endDate);
        Task<decimal> GetRoomRevenue(int roomId, DateTime startDate, DateTime endDate);
    }
} 