using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WPHBookingSystem.Application.DTOs.Booking;
using WPHBookingSystem.Application.DTOs.Room;
using WPHBookingSystem.Application.Interfaces.Services;
using WPHBookingSystem.Application.UseCases.Bookings;
using WPHBookingSystem.Application.UseCases.Rooms;

namespace WPHBookingSystem.Application.Services
{
    public class BookingSystemFacade : IBookingSystemFacade
    {
        private readonly CreateBookingUseCase _createBookingUseCase;
        private readonly UpdateBookingDatesUseCase _updateBookingUseCase;
        private readonly UpdateBookingStatusUseCase _updateBookingStatusUseCase;
        private readonly CancelBookingUseCase _cancelBookingUseCase;
        private readonly GetUserBookingsUseCase _getUserBookingsUseCase;

        private readonly CreateRoomUseCase _createRoomUseCase;
        private readonly UpdateRoomUseCase _updateRoomUseCase;
        private readonly UpdateRoomStatusUseCase _updateRoomStatusUseCase;
        private readonly GetRoomByIdUseCase _getRoomByIdUseCase;
        private readonly GetAllRoomsUseCase _getAllRoomsUseCase;
        private readonly DeleteRoomUseCase _deleteRoomUseCase;
        private readonly CheckRoomAvailabilityUseCase _checkRoomAvailabilityUseCase;
        private readonly GetRoomOccupancyRateUseCase _getRoomOccupancyRateUseCase;
        private readonly GetRoomRevenueUseCase _getRoomRevenueUseCase;

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
            GetRoomRevenueUseCase getRoomRevenueUseCase)
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
        }

        // Booking operations
        public async Task<BookingCreatedDto> CreateBooking(CreateBookingDto dto, Guid userId)
        {
            var result = await _createBookingUseCase.ExecuteAsync(dto, userId);
            if (!result.Success)
                throw new ApplicationException(result.Message);
            return result.Data;
        }

        public async Task<BookingDto> UpdateBooking(UpdateBookingDateDto dto)
        {
            return await _updateBookingUseCase.ExecuteAsync(dto);
        }

        public async Task<BookingDto> UpdateBookingStatus(UpdateBookingStatusRequest request)
        {
            return await _updateBookingStatusUseCase.ExecuteAsync(request);
        }

        public async Task<BookingDto> CancelBooking(Guid bookingId)
        {
            return await _cancelBookingUseCase.ExecuteAsync(bookingId);
        }

        public async Task<IEnumerable<BookingDto>> GetUserBookings(Guid userId)
        {
            return await _getUserBookingsUseCase.ExecuteAsync(userId);
        }

        // Room operations
        public async Task<Guid> CreateRoom(CreateRoomDto dto)
        {
            return await _createRoomUseCase.ExecuteAsync(dto);
        }

        public async Task<RoomDto> UpdateRoom(UpdateRoomDto dto)
        {
            return await _updateRoomUseCase.ExecuteAsync(dto);
        }

        public async Task<RoomDto> UpdateRoomStatus(Guid roomId, bool isAvailable)
        {
            return await _updateRoomStatusUseCase.ExecuteAsync(roomId, isAvailable);
        }

        public async Task<RoomDto> GetRoomById(Guid roomId)
        {
            return await _getRoomByIdUseCase.ExecuteAsync(roomId);
        }

        public async Task<IEnumerable<RoomDto>> GetAllRooms()
        {
            return await _getAllRoomsUseCase.ExecuteAsync();
        }

        public async Task<bool> DeleteRoom(Guid roomId)
        {
            return await _deleteRoomUseCase.ExecuteAsync(roomId);
        }

        public async Task<bool> CheckRoomAvailability(Guid roomId, DateTime startDate, DateTime endDate)
        {
            return await _checkRoomAvailabilityUseCase.ExecuteAsync(roomId, startDate, endDate);
        }

        public async Task<decimal> GetRoomOccupancyRate(Guid roomId, DateTime startDate, DateTime endDate)
        {
            return await _getRoomOccupancyRateUseCase.ExecuteAsync(roomId, startDate, endDate);
        }

        public async Task<decimal> GetRoomRevenue(Guid roomId, DateTime startDate, DateTime endDate)
        {
            return await _getRoomRevenueUseCase.ExecuteAsync(roomId, startDate, endDate);
        }

        public async Task<BookingDto> ViewBookingByToken(Guid bookingToken)
        {
            var result = await _viewBookingByTokenUseCase.ExecuteAsync(bookingToken);
            if (!result.Success)
                throw new ApplicationException(result.Message);
            return result.Data;
        }
    }
} 