using System;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using WPHBookingSystem.Application.DTOs.Booking;
using WPHBookingSystem.Application.Interfaces;
using WPHBookingSystem.Application.UseCases.Bookings;
using WPHBookingSystem.Domain.Entities;
using WPHBookingSystem.Domain.Enums;
using WPHBookingSystem.Domain.ValueObjects;

namespace WPHBookingSystem.Application.Tests.BookingTests
{
    [TestFixture]
    public class CreateBookingUseCaseTest
    {
        private Mock<IUnitOfWork> _mockUnitOfWork;
        private Mock<IBookingRepository> _mockBookingRepository;
        private Mock<IRoomRepository> _mockRoomRepository;
        private CreateBookingUseCase _createBookingUseCase;
        private Guid _userId;
        private Guid _roomId;

        [SetUp]
        public void Setup()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockBookingRepository = new Mock<IBookingRepository>();
            _mockRoomRepository = new Mock<IRoomRepository>();
            _mockUnitOfWork.Setup(uow => uow.BookingRepository).Returns(_mockBookingRepository.Object);
            _mockUnitOfWork.Setup(uow => uow.RoomRepository).Returns(_mockRoomRepository.Object);
            _createBookingUseCase = new CreateBookingUseCase(_mockUnitOfWork.Object);
            _userId = Guid.NewGuid();
            _roomId = Guid.NewGuid();
        }

        [Test]
        public async Task ExecuteAsync_Should_Create_Booking_When_Room_Available()
        {
            // Arrange
            var room = CreateRoom(_roomId, RoomStatus.Available);
            _mockRoomRepository.Setup(repo => repo.GetByIdAsync(_roomId))
                .ReturnsAsync(room);

            var dto = new CreateBookingDto
            {
                RoomId = _roomId,
                CheckIn = DateTime.UtcNow.AddDays(1),
                CheckOut = DateTime.UtcNow.AddDays(3),
                Guests = 2,
                Phone = "1234567890",
                Address = "Test Address",
                EmailAddress = "test@example.com",
                SpecialRequests = "No special requests"
            };

            // Act
            var result = await _createBookingUseCase.ExecuteAsync(dto);

            // Assert
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Data, Is.Not.Null);
            Assert.That(result.Data.Id, Is.Not.EqualTo(Guid.Empty));
            _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public async Task ExecuteAsync_Should_Return_Error_When_Room_Not_Found()
        {
            // Arrange
            _mockRoomRepository.Setup(repo => repo.GetByIdAsync(_roomId))
                .ReturnsAsync((Room)null);

            var dto = new CreateBookingDto
            {
                RoomId = _roomId,
                CheckIn = DateTime.UtcNow.AddDays(1),
                CheckOut = DateTime.UtcNow.AddDays(3),
                Guests = 2,
                Phone = "1234567890",
                Address = "Test Address",
                EmailAddress = "test@example.com",
                SpecialRequests = "No special requests"
            };

            // Act
            var result = await _createBookingUseCase.ExecuteAsync(dto);

            // Assert
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Message, Is.EqualTo("Room not found"));
            _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(), Times.Never);
        }

        [Test]
        public async Task ExecuteAsync_Should_Return_Error_When_Room_Not_Available()
        {
            // Arrange
            var room = CreateRoom(_roomId, RoomStatus.Maintenance);
            _mockRoomRepository.Setup(repo => repo.GetByIdAsync(_roomId))
                .ReturnsAsync(room);

            var dto = new CreateBookingDto
            {
                RoomId = _roomId,
                CheckIn = DateTime.UtcNow.AddDays(1),
                CheckOut = DateTime.UtcNow.AddDays(3),
                Guests = 2,
                Phone = "1234567890",
                Address = "Test Address",
                EmailAddress = "test@example.com",
                SpecialRequests = "No special requests"
            };

            // Act
            var result = await _createBookingUseCase.ExecuteAsync(dto);

            // Assert
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Message, Is.EqualTo("Room is not available on selected dates"));
            _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(), Times.Never);
        }

        [Test]
        public async Task ExecuteAsync_Should_Return_Error_When_Dates_Are_Invalid()
        {
            // Arrange
            var room = CreateRoom(_roomId, RoomStatus.Available);
            _mockRoomRepository.Setup(repo => repo.GetByIdAsync(_roomId))
                .ReturnsAsync(room);

            var dto = new CreateBookingDto
            {
                RoomId = _roomId,
                CheckIn = DateTime.UtcNow.AddDays(3), // Check-in after check-out
                CheckOut = DateTime.UtcNow.AddDays(1),
                Guests = 2,
                Phone = "1234567890",
                Address = "Test Address",
                EmailAddress = "test@example.com",
                SpecialRequests = "No special requests"
            };

            // Act
            var result = await _createBookingUseCase.ExecuteAsync(dto);

            // Assert
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Message, Is.EqualTo("Check-in date must be before check-out date"));
            _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(), Times.Never);
        }

        private Room CreateRoom(Guid roomId, RoomStatus status)
        {
            var room = (Room)Activator.CreateInstance(typeof(Room), true);
            typeof(Room).GetProperty("Id").SetValue(room, roomId);
            typeof(Room).GetProperty("Name").SetValue(room, "Test Room");
            typeof(Room).GetProperty("Description").SetValue(room, "Test Description");
            typeof(Room).GetProperty("Price").SetValue(room, 100m);
            typeof(Room).GetProperty("Capacity").SetValue(room, 2);
            typeof(Room).GetProperty("Status").SetValue(room, status);
            typeof(Room).GetProperty("Images").SetValue(room, new List<GalleryImage>());
            return room;
        }
    }
} 