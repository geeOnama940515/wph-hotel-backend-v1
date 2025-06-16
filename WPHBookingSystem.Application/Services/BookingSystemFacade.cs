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
        private readonly ViewBookingByTokenUseCase _viewBookingByTokenUseCase;

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

        // Booking operations
        public async Task<BookingCreatedDto> CreateBooking(CreateBookingDto dto, Guid userId)
        {
            var result = await _createBookingUseCase.ExecuteAsync(dto, userId);
            if (!result.IsSuccess)
                throw new ApplicationException(result.Message);
            return result.Data;
        }

        public async Task<BookingDto> UpdateBooking(Guid bookingId, UpdateBookingDateDto dto)
        {
            var result = await _updateBookingUseCase.ExecuteAsync(bookingId, dto);
            if (!result.IsSuccess)
                throw new ApplicationException(result.Message);
            return result.Data;
        }

        public async Task<Result<BookingDto>> UpdateBookingStatus(UpdateBookingStatusRequest request)
        {
            return await _updateBookingStatusUseCase.ExecuteAsync(request);
        }

        public async Task<Result<BookingDto>> CancelBooking(Guid bookingId,string emailAddress)
        {
            return await _cancelBookingUseCase.ExecuteAsync(bookingId, emailAddress);
        }

        public async Task<Result<List<BookingDto>>> GetUserBookings(string emailAddress)
        {
            return await _getUserBookingsUseCase.ExecuteAsync(emailAddress);
        }

        // Room operations
        public async Task<Result<Guid>> CreateRoom(CreateRoomDto dto)
        {
            return await _createRoomUseCase.ExecuteAsync(dto);
        }

        public async Task<Result<RoomDto>> UpdateRoom(Guid roomId,UpdateRoomDto dto)
        {
            return await _updateRoomUseCase.ExecuteAsync(roomId,dto);
        }

        public async Task<Result<RoomDto>> UpdateRoomStatus(UpdateRoomStatusRequest request) 
        {
            return await _updateRoomStatusUseCase.ExecuteAsync(request);
        }

        public async Task<Result<RoomDto>> GetRoomById(Guid roomId)
        {
            return await _getRoomByIdUseCase.ExecuteAsync(roomId);
        }

        public async Task<Result<List<RoomDto>>> GetAllRooms()
        {
            return await _getAllRoomsUseCase.ExecuteAsync();
        }

        public async Task<Result<bool>> DeleteRoom(Guid roomId)
        {
            return await _deleteRoomUseCase.ExecuteAsync(roomId);
        }

        public async Task<Result<CheckRoomAvailabilityResponse>> CheckRoomAvailability(CheckRoomAvailabilityRequest request)
        {
            return await _checkRoomAvailabilityUseCase.ExecuteAsync(request);
        }

        public async Task<Result<GetRoomOccupancyRateResponse>> GetRoomOccupancyRate(GetRoomOccupancyRateRequest request)
        {
            return await _getRoomOccupancyRateUseCase.ExecuteAsync(request);
        }

        public async Task<Result<GetRoomRevenueResponse>> GetRoomRevenue(GetRoomRevenueRequest request)
        {
            return await _getRoomRevenueUseCase.ExecuteAsync(request);
        }

        public async Task<BookingDto> ViewBookingByToken(Guid bookingToken)
        {
            var result = await _viewBookingByTokenUseCase.ExecuteAsync(bookingToken);
            if (!result.IsSuccess)
                throw new ApplicationException(result.Message);
            return result.Data;
        }

    }
} 