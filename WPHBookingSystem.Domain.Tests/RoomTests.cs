using System;
using System.Linq;
using NUnit.Framework;
using WPHBookingSystem.Domain.Entities;
using WPHBookingSystem.Domain.Enums;
using WPHBookingSystem.Domain.Exceptions;

namespace WPHBookingSystem.Domain.Tests
{
    [TestFixture]
    public class RoomTests
    {
        private Guid _userId;
        private string _roomName;
        private string _roomDescription;
        private decimal _roomPrice;
        private int _roomCapacity;
        private string _roomImage;

        [SetUp]
        public void Setup()
        {
            _userId = Guid.NewGuid();
            _roomName = "Deluxe Suite";
            _roomDescription = "Spacious room with ocean view";
            _roomPrice = 150.00m;
            _roomCapacity = 2;
            _roomImage = "deluxe-suite.jpg";
        }

        #region Create Tests

        [Test]
        public void Create_Should_Initialize_Room_With_Valid_Data()
        {
            var room = Room.Create(_roomName, _roomDescription, _roomPrice, _roomCapacity, _roomImage);

            Assert.That(room.Id, Is.Not.EqualTo(Guid.Empty));
            Assert.That(room.Name, Is.EqualTo(_roomName));
            Assert.That(room.Description, Is.EqualTo(_roomDescription));
            Assert.That(room.Price, Is.EqualTo(_roomPrice));
            Assert.That(room.Capacity, Is.EqualTo(_roomCapacity));
            Assert.That(room.Image, Is.EqualTo(_roomImage));
            Assert.That(room.Status, Is.EqualTo(RoomStatus.Available));
            Assert.That(room.Bookings.Count, Is.EqualTo(0));
        }

        [Test]
        public void Create_Should_Trim_Whitespace_From_Strings()
        {
            var room = Room.Create("  " + _roomName + "  ", "  " + _roomDescription + "  ", _roomPrice, _roomCapacity, "  " + _roomImage + "  ");

            Assert.That(room.Name, Is.EqualTo(_roomName));
            Assert.That(room.Description, Is.EqualTo(_roomDescription));
            Assert.That(room.Image, Is.EqualTo(_roomImage));
        }

        [Test]
        public void Create_Should_Handle_Optional_Parameters()
        {
            var room = Room.Create(_roomName, _roomDescription, _roomPrice, _roomCapacity);

            Assert.That(room.Image, Is.EqualTo(string.Empty));
        }

        [Test]
        public void Create_Should_Handle_Null_Optional_Parameters()
        {
            var room = Room.Create(_roomName, null, _roomPrice, _roomCapacity, null);

            Assert.That(room.Description, Is.EqualTo(string.Empty));
            Assert.That(room.Image, Is.EqualTo(string.Empty));
        }

        [Test]
        public void Create_Should_Throw_If_Name_Is_Null_Or_Empty()
        {
            Assert.Throws<DomainException>(() => Room.Create(null, _roomDescription, _roomPrice, _roomCapacity));
            Assert.Throws<DomainException>(() => Room.Create("", _roomDescription, _roomPrice, _roomCapacity));
            Assert.Throws<DomainException>(() => Room.Create("   ", _roomDescription, _roomPrice, _roomCapacity));
        }

        [Test]
        public void Create_Should_Throw_If_Price_Is_Zero_Or_Negative()
        {
            Assert.Throws<DomainException>(() => Room.Create(_roomName, _roomDescription, 0, _roomCapacity));
            Assert.Throws<DomainException>(() => Room.Create(_roomName, _roomDescription, -50, _roomCapacity));
        }

        [Test]
        public void Create_Should_Throw_If_Capacity_Is_Zero_Or_Negative()
        {
            Assert.Throws<DomainException>(() => Room.Create(_roomName, _roomDescription, _roomPrice, 0));
            Assert.Throws<DomainException>(() => Room.Create(_roomName, _roomDescription, _roomPrice, -1));
        }

        #endregion

        #region UpdateDetails Tests

        [Test]
        public void UpdateDetails_Should_Update_Room_Properties()
        {
            var room = Room.Create(_roomName, _roomDescription, _roomPrice, _roomCapacity, _roomImage);
            var newName = "Premium Suite";
            var newDescription = "Luxury room with balcony";
            var newPrice = 200.00m;
            var newCapacity = 4;
            var newImage = "premium-suite.jpg";

            room.UpdateDetails(newName, newDescription, newPrice, newCapacity, newImage);

            Assert.That(room.Name, Is.EqualTo(newName));
            Assert.That(room.Description, Is.EqualTo(newDescription));
            Assert.That(room.Price, Is.EqualTo(newPrice));
            Assert.That(room.Capacity, Is.EqualTo(newCapacity));
            Assert.That(room.Image, Is.EqualTo(newImage));
        }

        [Test]
        public void UpdateDetails_Should_Throw_If_Name_Is_Invalid()
        {
            var room = Room.Create(_roomName, _roomDescription, _roomPrice, _roomCapacity);

            Assert.Throws<DomainException>(() => room.UpdateDetails("", _roomDescription, _roomPrice, _roomCapacity));
            Assert.Throws<DomainException>(() => room.UpdateDetails(null, _roomDescription, _roomPrice, _roomCapacity));
        }

        [Test]
        public void UpdateDetails_Should_Throw_If_Price_Is_Invalid()
        {
            var room = Room.Create(_roomName, _roomDescription, _roomPrice, _roomCapacity);

            Assert.Throws<DomainException>(() => room.UpdateDetails(_roomName, _roomDescription, 0, _roomCapacity));
            Assert.Throws<DomainException>(() => room.UpdateDetails(_roomName, _roomDescription, -100, _roomCapacity));
        }

        [Test]
        public void UpdateDetails_Should_Throw_If_Capacity_Is_Invalid()
        {
            var room = Room.Create(_roomName, _roomDescription, _roomPrice, _roomCapacity);

            Assert.Throws<DomainException>(() => room.UpdateDetails(_roomName, _roomDescription, _roomPrice, 0));
            Assert.Throws<DomainException>(() => room.UpdateDetails(_roomName, _roomDescription, _roomPrice, -1));
        }

        #endregion

        #region IsAvailable Tests

        [Test]
        public void IsAvailable_Should_Return_True_For_Available_Room_With_No_Bookings()
        {
            var room = Room.Create(_roomName, _roomDescription, _roomPrice, _roomCapacity);
            var checkIn = DateTime.UtcNow.AddDays(1);
            var checkOut = DateTime.UtcNow.AddDays(3);

            var isAvailable = room.IsAvailable(checkIn, checkOut);

            Assert.That(isAvailable, Is.True);
        }

        [Test]
        public void IsAvailable_Should_Return_False_For_Inactive_Room()
        {
            var room = Room.Create(_roomName, _roomDescription, _roomPrice, _roomCapacity);
            room.Deactivate();
            var checkIn = DateTime.UtcNow.AddDays(1);
            var checkOut = DateTime.UtcNow.AddDays(3);

            var isAvailable = room.IsAvailable(checkIn, checkOut);

            Assert.That(isAvailable, Is.False);
        }

        [Test]
        public void IsAvailable_Should_Return_False_For_Maintenance_Room()
        {
            var room = Room.Create(_roomName, _roomDescription, _roomPrice, _roomCapacity);
            room.SetMaintenance();
            var checkIn = DateTime.UtcNow.AddDays(1);
            var checkOut = DateTime.UtcNow.AddDays(3);

            var isAvailable = room.IsAvailable(checkIn, checkOut);

            Assert.That(isAvailable, Is.False);
        }

        [Test]
        public void IsAvailable_Should_Return_False_For_Overlapping_Confirmed_Booking()
        {
            var room = Room.Create(_roomName, _roomDescription, _roomPrice, _roomCapacity);
            var existingCheckIn = DateTime.UtcNow.AddDays(2);
            var existingCheckOut = DateTime.UtcNow.AddDays(4);
            var existingBooking = CreateBookingWithStatus(_userId, room.Id, existingCheckIn, existingCheckOut, BookingStatus.Confirmed);
            room.AddBooking(existingBooking);

            // Overlapping dates
            var checkIn = DateTime.UtcNow.AddDays(3);
            var checkOut = DateTime.UtcNow.AddDays(5);

            var isAvailable = room.IsAvailable(checkIn, checkOut);

            Assert.That(isAvailable, Is.False);
        }

        [Test]
        public void IsAvailable_Should_Return_True_For_Non_Overlapping_Dates()
        {
            var room = Room.Create(_roomName, _roomDescription, _roomPrice, _roomCapacity);
            var existingCheckIn = DateTime.UtcNow.AddDays(2);
            var existingCheckOut = DateTime.UtcNow.AddDays(4);
            var existingBooking = CreateBookingWithStatus(_userId, room.Id, existingCheckIn, existingCheckOut, BookingStatus.Confirmed);
            room.AddBooking(existingBooking);

            // Non-overlapping dates (after existing booking)
            var checkIn = DateTime.UtcNow.AddDays(5);
            var checkOut = DateTime.UtcNow.AddDays(7);

            var isAvailable = room.IsAvailable(checkIn, checkOut);

            Assert.That(isAvailable, Is.True);
        }

        [Test]
        public void IsAvailable_Should_Return_True_For_Cancelled_Booking_Dates()
        {
            var room = Room.Create(_roomName, _roomDescription, _roomPrice, _roomCapacity);
            var existingCheckIn = DateTime.UtcNow.AddDays(2);
            var existingCheckOut = DateTime.UtcNow.AddDays(4);
            var cancelledBooking = CreateBookingWithStatus(_userId, room.Id, existingCheckIn, existingCheckOut, BookingStatus.Cancelled);
            room.AddBooking(cancelledBooking);

            // Same dates as cancelled booking
            var checkIn = DateTime.UtcNow.AddDays(2);
            var checkOut = DateTime.UtcNow.AddDays(4);

            var isAvailable = room.IsAvailable(checkIn, checkOut);

            Assert.That(isAvailable, Is.True);
        }

        [Test]
        public void IsAvailable_Should_Throw_If_Start_Date_After_End_Date()
        {
            var room = Room.Create(_roomName, _roomDescription, _roomPrice, _roomCapacity);
            var checkIn = DateTime.UtcNow.AddDays(5);
            var checkOut = DateTime.UtcNow.AddDays(3);

            Assert.Throws<DomainException>(() => room.IsAvailable(checkIn, checkOut));
        }

        #endregion

        #region AddBooking Tests

        [Test]
        public void AddBooking_Should_Add_Booking_When_Room_Is_Available()
        {
            var room = Room.Create(_roomName, _roomDescription, _roomPrice, _roomCapacity);
            var checkIn = DateTime.UtcNow.AddDays(1);
            var checkOut = DateTime.UtcNow.AddDays(3);
            var booking = CreateBookingWithStatus(_userId, room.Id, checkIn, checkOut, BookingStatus.Pending);

            room.AddBooking(booking);

            Assert.That(room.Bookings.Count, Is.EqualTo(1));
            Assert.That(room.Bookings.First(), Is.EqualTo(booking));
        }

        [Test]
        public void AddBooking_Should_Throw_If_Booking_Is_Null()
        {
            var room = Room.Create(_roomName, _roomDescription, _roomPrice, _roomCapacity);

            Assert.Throws<DomainException>(() => room.AddBooking(null));
        }

        [Test]
        public void AddBooking_Should_Throw_If_Booking_RoomId_Mismatch()
        {
            var room = Room.Create(_roomName, _roomDescription, _roomPrice, _roomCapacity);
            var differentRoomId = Guid.NewGuid();
            var checkIn = DateTime.UtcNow.AddDays(1);
            var checkOut = DateTime.UtcNow.AddDays(3);
            var booking = CreateBookingWithStatus(_userId, differentRoomId, checkIn, checkOut, BookingStatus.Pending);

            Assert.Throws<DomainException>(() => room.AddBooking(booking));
        }

        [Test]
        public void AddBooking_Should_Throw_If_Room_Not_Available()
        {
            var room = Room.Create(_roomName, _roomDescription, _roomPrice, _roomCapacity);
            var checkIn = DateTime.UtcNow.AddDays(1);
            var checkOut = DateTime.UtcNow.AddDays(3);

            // Add first booking
            var firstBooking = CreateBookingWithStatus(_userId, room.Id, checkIn, checkOut, BookingStatus.Confirmed);
            room.AddBooking(firstBooking);

            // Try to add overlapping booking
            var overlappingBooking = CreateBookingWithStatus(_userId, room.Id, checkIn.AddDays(1), checkOut.AddDays(1), BookingStatus.Pending);

            Assert.Throws<DomainException>(() => room.AddBooking(overlappingBooking));
        }

        #endregion

        #region Status Management Tests

        [Test]
        public void Activate_Should_Set_Status_To_Available()
        {
            var room = Room.Create(_roomName, _roomDescription, _roomPrice, _roomCapacity);
            room.Deactivate();

            room.Activate();

            Assert.That(room.Status, Is.EqualTo(RoomStatus.Available));
        }

        [Test]
        public void Activate_Should_Throw_If_Already_Active()
        {
            var room = Room.Create(_roomName, _roomDescription, _roomPrice, _roomCapacity);

            Assert.Throws<DomainException>(() => room.Activate());
        }

        [Test]
        public void Deactivate_Should_Set_Status_To_Inactive()
        {
            var room = Room.Create(_roomName, _roomDescription, _roomPrice, _roomCapacity);

            room.Deactivate();

            Assert.That(room.Status, Is.EqualTo(RoomStatus.Inactive));
        }

        [Test]
        public void Deactivate_Should_Throw_If_Already_Inactive()
        {
            var room = Room.Create(_roomName, _roomDescription, _roomPrice, _roomCapacity);
            room.Deactivate();

            Assert.Throws<DomainException>(() => room.Deactivate());
        }

        [Test]
        public void Deactivate_Should_Throw_If_Has_Future_Bookings()
        {
            var room = Room.Create(_roomName, _roomDescription, _roomPrice, _roomCapacity);
            var futureCheckIn = DateTime.UtcNow.AddDays(5);
            var futureCheckOut = DateTime.UtcNow.AddDays(7);
            var futureBooking = CreateBookingWithStatus(_userId, room.Id, futureCheckIn, futureCheckOut, BookingStatus.Confirmed);
            room.AddBooking(futureBooking);

            Assert.Throws<DomainException>(() => room.Deactivate());
        }

        [Test]
        public void SetMaintenance_Should_Set_Status_To_Maintenance()
        {
            var room = Room.Create(_roomName, _roomDescription, _roomPrice, _roomCapacity);

            room.SetMaintenance();

            Assert.That(room.Status, Is.EqualTo(RoomStatus.Maintenance));
        }

        [Test]
        public void SetMaintenance_Should_Throw_If_Already_In_Maintenance()
        {
            var room = Room.Create(_roomName, _roomDescription, _roomPrice, _roomCapacity);
            room.SetMaintenance();

            Assert.Throws<DomainException>(() => room.SetMaintenance());
        }

        [Test]
        public void SetMaintenance_Should_Throw_If_Has_Future_Bookings()
        {
            var room = Room.Create(_roomName, _roomDescription, _roomPrice, _roomCapacity);
            var futureCheckIn = DateTime.UtcNow.AddDays(5);
            var futureCheckOut = DateTime.UtcNow.AddDays(7);
            var futureBooking = CreateBookingWithStatus(_userId, room.Id, futureCheckIn, futureCheckOut, BookingStatus.Confirmed);
            room.AddBooking(futureBooking);

            Assert.Throws<DomainException>(() => room.SetMaintenance());
        }

        #endregion

        #region Revenue Calculation Tests

        [Test]
        public void CalculateRevenue_Should_Return_Zero_For_No_Completed_Bookings()
        {
            var room = Room.Create(_roomName, _roomDescription, _roomPrice, _roomCapacity);

            var revenue = room.CalculateRevenue();

            Assert.That(revenue, Is.EqualTo(0));
        }

        [Test]
        public void CalculateRevenue_Should_Sum_Completed_Bookings()
        {
            var room = Room.Create(_roomName, _roomDescription, _roomPrice, _roomCapacity);
            var booking1 = CreateBookingWithStatus(_userId, room.Id, DateTime.UtcNow.AddDays(-10), DateTime.UtcNow.AddDays(-8), BookingStatus.Completed, 100m);
            var booking2 = CreateBookingWithStatus(_userId, room.Id, DateTime.UtcNow.AddDays(-6), DateTime.UtcNow.AddDays(-4), BookingStatus.Completed, 150m);
            var booking3 = CreateBookingWithStatus(_userId, room.Id, DateTime.UtcNow.AddDays(-2), DateTime.UtcNow.AddDays(1), BookingStatus.Confirmed, 200m); // Not completed
            room.AddBooking(booking1);
            room.AddBooking(booking2);
            room.AddBooking(booking3);

            var revenue = room.CalculateRevenue();

            Assert.That(revenue, Is.EqualTo(250m)); // Only completed bookings
        }

        [Test]
        public void CalculateRevenue_Should_Filter_By_Date_Range()
        {
            var room = Room.Create(_roomName, _roomDescription, _roomPrice, _roomCapacity);
            var booking1 = CreateBookingWithStatus(_userId, room.Id, DateTime.UtcNow.AddDays(-20), DateTime.UtcNow.AddDays(-18), BookingStatus.Completed, 100m);
            var booking2 = CreateBookingWithStatus(_userId, room.Id, DateTime.UtcNow.AddDays(-10), DateTime.UtcNow.AddDays(-8), BookingStatus.Completed, 150m);
            room.AddBooking(booking1);
            room.AddBooking(booking2);

            var revenue = room.CalculateRevenue(DateTime.UtcNow.AddDays(-15), DateTime.UtcNow);

            Assert.That(revenue, Is.EqualTo(150m)); // Only booking2 is in range
        }

        #endregion

        #region Occupancy Rate Tests

        [Test]
        public void GetOccupancyRate_Should_Return_Zero_For_No_Completed_Bookings()
        {
            var room = Room.Create(_roomName, _roomDescription, _roomPrice, _roomCapacity);
            var startDate = DateTime.UtcNow.AddDays(-30);
            var endDate = DateTime.UtcNow;

            var occupancyRate = room.GetOccupancyRate(startDate, endDate);

            Assert.That(occupancyRate, Is.EqualTo(0));
        }

        [Test]
        public void GetOccupancyRate_Should_Calculate_Correct_Percentage()
        {
            var room = Room.Create(_roomName, _roomDescription, _roomPrice, _roomCapacity);
            var startDate = DateTime.UtcNow.AddDays(-10);
            var endDate = DateTime.UtcNow;

            // 3-day booking in the middle of 10-day period = 30% occupancy
            var booking = CreateBookingWithStatus(_userId, room.Id, DateTime.UtcNow.AddDays(-7), DateTime.UtcNow.AddDays(-4), BookingStatus.Completed);
            room.AddBooking(booking);

            var occupancyRate = room.GetOccupancyRate(startDate, endDate);

            Assert.That(occupancyRate, Is.EqualTo(30));
        }

        [Test]
        public void GetOccupancyRate_Should_Throw_If_Start_Date_After_End_Date()
        {
            var room = Room.Create(_roomName, _roomDescription, _roomPrice, _roomCapacity);

            Assert.Throws<DomainException>(() => room.GetOccupancyRate(DateTime.UtcNow, DateTime.UtcNow.AddDays(-1)));
        }

        #endregion

        #region Helper Methods

        private Booking CreateBookingWithStatus(Guid userId, Guid roomId, DateTime checkIn, DateTime checkOut, BookingStatus status, decimal amount = 100m)
        {
            // Use reflection to create a booking with specific status
            var booking = (Booking)Activator.CreateInstance(typeof(Booking), true);

            typeof(Booking).GetProperty("Id").SetValue(booking, Guid.NewGuid());
            typeof(Booking).GetProperty("UserId").SetValue(booking, userId);
            typeof(Booking).GetProperty("RoomId").SetValue(booking, roomId);
            typeof(Booking).GetProperty("CheckIn").SetValue(booking, checkIn);
            typeof(Booking).GetProperty("CheckOut").SetValue(booking, checkOut);
            typeof(Booking).GetProperty("Guests").SetValue(booking, 2);
            typeof(Booking).GetProperty("TotalAmount").SetValue(booking, amount);
            typeof(Booking).GetProperty("Status").SetValue(booking, status);
            typeof(Booking).GetProperty("SpecialRequests").SetValue(booking, string.Empty);
            typeof(Booking).GetProperty("Phone").SetValue(booking, string.Empty);
            typeof(Booking).GetProperty("Address").SetValue(booking, string.Empty);

            return booking;
        }

        #endregion
    }
}