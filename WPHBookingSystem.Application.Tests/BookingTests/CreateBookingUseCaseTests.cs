using Moq;

using WPHBookingSystem.Application.DTOs.Booking;
using WPHBookingSystem.Application.Interfaces;
using WPHBookingSystem.Application.UseCases.Bookings;
using WPHBookingSystem.Domain.Entities;
using WPHBookingSystem.Domain.Exceptions;

namespace WPHBookingSystem.Application.Tests.BookingTests
{
    public class CreateBookingUseCaseTests
    {
        private Mock<IUnitOfWork> _mockUnitOfWork;
        private Mock<IBookingRepository> _mockBookingRepository;
        private Mock<IRoomRepository> _mockRoomRepository;
        private CreateBookingUseCase _useCase;
        private Guid _userId;
        private Guid _roomId;

        [SetUp]
        public void Setup()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockBookingRepository = new Mock<IBookingRepository>();
            _mockRoomRepository = new Mock<IRoomRepository>();
            _mockUnitOfWork.Setup(x => x.BookingRepository).Returns(_mockBookingRepository.Object);
            _mockUnitOfWork.Setup(x => x.RoomRepository).Returns(_mockRoomRepository.Object);
            _useCase = new CreateBookingUseCase(_mockUnitOfWork.Object);
            _userId = Guid.NewGuid();
            _roomId = Guid.NewGuid();
        }

        //[Test]
        //public async Task ExecuteAsync_ValidBookingData_CreatesBookingSuccessfully()
        //{
        //    // Arrange
        //    var dto = new CreateBookingDto
        //    {
        //        RoomId = _roomId,
        //        CheckIn = DateTime.Today.AddDays(1),
        //        CheckOut = DateTime.Today.AddDays(3),
        //        Guests = 2,
        //        SpecialRequests = "Late checkout",
        //        Phone = "1234567890",
        //        Address = "Test Address"
        //    };

        //    var room = CreateTestRoom(_roomId, 100m);
        //    _mockRoomRepository.Setup(x => x.GetByIdAsync(_roomId))
        //        .ReturnsAsync(room);

        //    // Act
        //    var result = await _useCase.ExecuteAsync(dto, _userId);

        //    // Assert
        //    Assert.That(result, Is.TypeOf<Guid>());
        //    Assert.That(result, Is.Not.EqualTo(Guid.Empty));
        //    _mockBookingRepository.Verify(x => x.AddAsync(It.IsAny<Booking>()), Times.Once);
        //    _mockUnitOfWork.Verify(x => x.SaveChangesAsync(), Times.Once);
        //}

        [Test]
        public async Task ExecuteAsync_RoomNotFound_ThrowsDomainException()
        {
            // Arrange
            var dto = new CreateBookingDto { RoomId = _roomId };
            _mockRoomRepository.Setup(x => x.GetByIdAsync(_roomId))
                .ReturnsAsync((Room)null);

            // Act & Assert
            var exception =  Assert.ThrowsAsync<DomainException>(
                () => _useCase.ExecuteAsync(dto));

            Assert.That(exception.Message, Is.EqualTo("Room not found."));
        }

        //[Test]
        //public async Task ExecuteAsync_RoomNotAvailable_ThrowsDomainException()
        //{
        //    // Arrange
        //    var dto = new CreateBookingDto
        //    {
        //        RoomId = _roomId,
        //        CheckIn = DateTime.Today.AddDays(1),
        //        CheckOut = DateTime.Today.AddDays(3),
        //        Guests = 2,
        //        SpecialRequests = "Late checkout",
        //        Phone = "1234567890",
        //        Address = "Test Address"
        //    };
        //    var room = Room.Create("Room 101", "Nice room", 100m, 2);
        //    var conflictingBooking = Booking.Create(
        //        room.Id,
        //        Guid.NewGuid(),
        //        DateTime.Today.AddDays(2),  // Overlaps with dto.CheckIn
        //        DateTime.Today.AddDays(4),
        //        1,
        //        2,
        //        "Note",
        //        "1234567890",
        //        "Address"
        //    );
        //    // Simulate existing booking
        //    room.GetType().GetMethod("AddBooking")!.Invoke(room, new object[] { conflictingBooking });

        //    _mockRoomRepository.Setup(x => x.GetByIdAsync(_roomId)).ReturnsAsync(room);

        //    // Act & Assert
        //    var ex =  Assert.Throws<DomainException>(() => _useCase.ExecuteAsync(dto, _userId));
        //    Assert.That(ex.Message, Is.EqualTo("Room is not available on selected dates."));
        //}


        [Test]
        public void Constructor_NullUnitOfWork_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(
                () => new CreateBookingUseCase(null));
        }

        private Room CreateTestRoom(Guid roomId, decimal price)
        {
            var roomMock = new Mock<Room>();
            roomMock.Setup(x => x.Id).Returns(roomId);
            roomMock.Setup(x => x.Price).Returns(price);
            roomMock.Setup(x => x.IsAvailable(It.IsAny<DateTime>(), It.IsAny<DateTime>())).Returns(true);
            return roomMock.Object;
        }
    }
}
