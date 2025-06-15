using Moq;
using WPHBookingSystem.Application.Interfaces;
using WPHBookingSystem.Application.UseCases.Bookings;
using WPHBookingSystem.Domain.Entities;
using WPHBookingSystem.Domain.Exceptions;
using WPHBookingSystem.Domain.ValueObjects;

namespace WPHBookingSystem.Application.Tests.BookingTests
{
    [TestFixture]
    public class CancelBookingUseCaseTests
    {
        private Mock<IUnitOfWork> _mockUnitOfWork;
        private Mock<IBookingRepository> _mockBookingRepository;
        private CancelBookingUseCase _useCase;
        private string email = "admin@example.com";
        private Guid _bookingId;

        [SetUp]
        public void Setup()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockBookingRepository = new Mock<IBookingRepository>();
            _mockUnitOfWork.Setup(x => x.Bookings).Returns(_mockBookingRepository.Object);
            _useCase = new CancelBookingUseCase(_mockUnitOfWork.Object);
            email = Guid.NewGuid().ToString("N").Substring(0,10);
            _bookingId = Guid.NewGuid();
        }

        [Test]
        public async Task ExecuteAsync_ValidBookingAndUser_CancelsBookingSuccessfully()
        {
            // Arrange
            var booking = CreateTestBooking(_bookingId, email);
            _mockBookingRepository.Setup(x => x.GetByIdAsync(_bookingId))
                .ReturnsAsync(booking);

            // Act
            await _useCase.ExecuteAsync(_bookingId, email);

            // Assert
            _mockBookingRepository.Verify(x => x.UpdateAsync(booking), Times.Once);
            _mockUnitOfWork.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public async Task ExecuteAsync_BookingNotFound_ThrowsDomainException()
        {
            // Arrange
            _mockBookingRepository.Setup(x => x.GetByIdAsync(_bookingId))
                .ReturnsAsync((Booking)null);

            // Act & Assert
            var exception = Assert.ThrowsAsync<DomainException>(
                () => _useCase.ExecuteAsync(_bookingId, email));

            Assert.That(exception.Message, Is.EqualTo("Booking not found."));
        }

        [Test]
        public async Task ExecuteAsync_UnauthorizedUser_ThrowsDomainException()
        {
            // Arrange
            var differentEmail = "admin@test.com";
            var booking = CreateTestBooking(_bookingId, differentEmail);
            _mockBookingRepository.Setup(x => x.GetByIdAsync(_bookingId))
                .ReturnsAsync(booking);

            // Act & Assert
            var exception = Assert.ThrowsAsync<DomainException>(
                () => _useCase.ExecuteAsync(_bookingId, "admin@email.com"));

            Assert.That(exception.Message, Is.EqualTo("You are not authorized to cancel this booking."));
        }

        private Booking CreateTestBooking(Guid bookingId, string email)
        {
            // You'll need to adjust this based on your Booking entity constructor
            var contactInfo = new ContactInfo("1234567890", "Address");
            return Booking.Create(
                Guid.NewGuid(), // roomId
                DateTime.Today.AddDays(1),
                DateTime.Today.AddDays(3),
                2,
                200m,
                contactInfo,
                "NoRequests",
                "Sample@gmail.com"
            );
        }
    }
}
