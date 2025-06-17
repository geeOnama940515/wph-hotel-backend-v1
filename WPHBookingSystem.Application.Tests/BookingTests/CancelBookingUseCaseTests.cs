using System;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using WPHBookingSystem.Application.Interfaces;
using WPHBookingSystem.Application.UseCases.Bookings;
using WPHBookingSystem.Domain.Entities;
using WPHBookingSystem.Domain.Enums;
using WPHBookingSystem.Domain.ValueObjects;

namespace WPHBookingSystem.Application.Tests.BookingTests
{
    [TestFixture]
    public class CancelBookingUseCaseTests
    {
        private Mock<IUnitOfWork> _mockUnitOfWork;
        private Mock<IBookingRepository> _mockBookingRepository;
        private CancelBookingUseCase _cancelBookingUseCase;
        private Guid _bookingId;
        private Guid _userId;

        [SetUp]
        public void Setup()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockBookingRepository = new Mock<IBookingRepository>();
            _mockUnitOfWork.Setup(uow => uow.BookingRepository).Returns(_mockBookingRepository.Object);
            _cancelBookingUseCase = new CancelBookingUseCase(_mockUnitOfWork.Object);
            _bookingId = Guid.NewGuid();
            _userId = Guid.NewGuid();
        }

        [Test]
        public async Task ExecuteAsync_Should_Cancel_Booking_When_Valid()
        {
            // Arrange
            var booking = CreateBookingWithStatus(_bookingId, _userId, BookingStatus.Confirmed);
            _mockBookingRepository.Setup(repo => repo.GetByIdAsync(_bookingId))
                .ReturnsAsync(booking);

            // Act
            var result = await _cancelBookingUseCase.ExecuteAsync(_bookingId);

            // Assert
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(booking.Status, Is.EqualTo(BookingStatus.Cancelled));
            _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public async Task ExecuteAsync_Should_Return_Error_When_Booking_Not_Found()
        {
            // Arrange
            _mockBookingRepository.Setup(repo => repo.GetByIdAsync(_bookingId))
                .ReturnsAsync((Booking)null);

            // Act
            var result = await _cancelBookingUseCase.ExecuteAsync(_bookingId);

            // Assert
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Message, Is.EqualTo("Booking not found"));
            _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(), Times.Never);
        }

        [Test]
        public async Task ExecuteAsync_Should_Return_Error_When_Booking_Already_Cancelled()
        {
            // Arrange
            var booking = CreateBookingWithStatus(_bookingId, _userId, BookingStatus.Cancelled);
            _mockBookingRepository.Setup(repo => repo.GetByIdAsync(_bookingId))
                .ReturnsAsync(booking);

            // Act
            var result = await _cancelBookingUseCase.ExecuteAsync(_bookingId);

            // Assert
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Message, Is.EqualTo("Booking is already cancelled"));
            _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(), Times.Never);
        }

        [Test]
        public async Task ExecuteAsync_Should_Return_Error_When_Booking_Already_Completed()
        {
            // Arrange
            var booking = CreateBookingWithStatus(_bookingId, _userId, BookingStatus.Completed);
            _mockBookingRepository.Setup(repo => repo.GetByIdAsync(_bookingId))
                .ReturnsAsync(booking);

            // Act
            var result = await _cancelBookingUseCase.ExecuteAsync(_bookingId);

            // Assert
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Message, Is.EqualTo("Cannot cancel a completed booking"));
            _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(), Times.Never);
        }

        [Test]
        public async Task ExecuteAsync_Should_Return_Error_When_User_Not_Authorized()
        {
            // Arrange
            var booking = CreateBookingWithStatus(_bookingId, Guid.NewGuid(), BookingStatus.Confirmed);
            _mockBookingRepository.Setup(repo => repo.GetByIdAsync(_bookingId))
                .ReturnsAsync(booking);

            // Act
            var result = await _cancelBookingUseCase.ExecuteAsync(_bookingId);

            // Assert
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Message, Is.EqualTo("User is not authorized to cancel this booking"));
            _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(), Times.Never);
        }

        private Booking CreateBookingWithStatus(Guid bookingId, Guid userId, BookingStatus status)
        {
            var booking = (Booking)Activator.CreateInstance(typeof(Booking), true);
            typeof(Booking).GetProperty("Id").SetValue(booking, bookingId);
            typeof(Booking).GetProperty("UserId").SetValue(booking, userId);
            typeof(Booking).GetProperty("RoomId").SetValue(booking, Guid.NewGuid());
            typeof(Booking).GetProperty("CheckIn").SetValue(booking, DateTime.UtcNow.AddDays(1));
            typeof(Booking).GetProperty("CheckOut").SetValue(booking, DateTime.UtcNow.AddDays(3));
            typeof(Booking).GetProperty("Guests").SetValue(booking, 2);
            typeof(Booking).GetProperty("TotalAmount").SetValue(booking, 100m);
            typeof(Booking).GetProperty("Status").SetValue(booking, status);
            typeof(Booking).GetProperty("SpecialRequests").SetValue(booking, string.Empty);
            typeof(Booking).GetProperty("Phone").SetValue(booking, string.Empty);
            typeof(Booking).GetProperty("Address").SetValue(booking, string.Empty);
            return booking;
        }
    }
}
