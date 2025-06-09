using System;
using NUnit.Framework;
using WPHBookingSystem.Domain.Entities;
using WPHBookingSystem.Domain.Enums;
using WPHBookingSystem.Domain.Exceptions;

namespace WPHBookingSystem.Domain.Tests
{
    [TestFixture]
    public class BookingTests
    {
        private Guid _userId;
        private Guid _roomId;
        private DateTime _checkIn;
        private DateTime _checkOut;

        [SetUp]
        public void Setup()
        {
            _userId = Guid.NewGuid();
            _roomId = Guid.NewGuid();
            _checkIn = DateTime.UtcNow.AddDays(1);
            _checkOut = DateTime.UtcNow.AddDays(3);
        }

        [Test]
        public void Create_Should_Initialize_With_Pending_Status()
        {
            var booking = Booking.Create(_userId, _roomId, _checkIn, _checkOut, 2, 100m);

            Assert.That(booking.Status, Is.EqualTo(BookingStatus.Pending));
            Assert.That(booking.CheckIn, Is.EqualTo(_checkIn));
            Assert.That(booking.CheckOut, Is.EqualTo(_checkOut));
        }

        [Test]
        public void Confirm_Should_Set_Status_To_Confirmed_When_Pending()
        {
            var booking = Booking.Create(_userId, _roomId, _checkIn, _checkOut, 2, 100m);

            booking.Confirm();

            Assert.That(booking.Status, Is.EqualTo(BookingStatus.Confirmed));
        }

        [Test]
        public void Confirm_Should_Throw_If_Not_Pending()
        {
            var booking = Booking.Create(_userId, _roomId, _checkIn, _checkOut, 2, 100m);
            booking.Confirm();

            Assert.Throws<DomainException>(() => booking.Confirm());
        }

        [Test]
        public void Cancel_Should_Set_Status_To_Cancelled_If_Not_Completed()
        {
            var booking = Booking.Create(_userId, _roomId, _checkIn, _checkOut, 2, 100m);

            booking.Cancel();

            Assert.That(booking.Status, Is.EqualTo(BookingStatus.Cancelled));
        }

        [Test]
        public void Cancel_Should_Throw_If_Completed()
        {
            // Create a booking that can be completed (past dates)
            var pastCheckIn = DateTime.UtcNow.AddDays(-5);
            var pastCheckOut = DateTime.UtcNow.AddDays(-2);

            // Use reflection or a test constructor to create booking with past dates
            var booking = CreateBookingWithPastDates(_userId, _roomId, pastCheckIn, pastCheckOut, 2, 100m);
            booking.Confirm();
            booking.Complete();

            var ex = Assert.Throws<DomainException>(() => booking.Cancel());
            Assert.That(ex.Message, Is.EqualTo("Completed bookings cannot be cancelled."));
        }

        [Test]
        public void Complete_Should_Set_Status_To_Completed_When_Confirmed_And_CheckOut_Passed()
        {
            // Create booking with past dates for completion
            var pastCheckIn = DateTime.UtcNow.AddDays(-5);
            var pastCheckOut = DateTime.UtcNow.AddDays(-2);

            var booking = CreateBookingWithPastDates(_userId, _roomId, pastCheckIn, pastCheckOut, 2, 100m);
            booking.Confirm();

            booking.Complete();

            Assert.That(booking.Status, Is.EqualTo(BookingStatus.Completed));
        }

        [Test]
        public void Complete_Should_Throw_If_Not_Confirmed()
        {
            var pastCheckIn = DateTime.UtcNow.AddDays(-5);
            var pastCheckOut = DateTime.UtcNow.AddDays(-2);

            var booking = CreateBookingWithPastDates(_userId, _roomId, pastCheckIn, pastCheckOut, 2, 100m);

            Assert.Throws<DomainException>(() => booking.Complete());
        }

        [Test]
        public void Complete_Should_Throw_If_CheckOut_Not_Passed()
        {
            var booking = Booking.Create(_userId, _roomId, _checkIn, _checkOut, 2, 100m);
            booking.Confirm();

            Assert.Throws<DomainException>(() => booking.Complete());
        }

        [Test]
        public void UpdateBookingDates_Should_Update_When_Pending_And_Valid_Dates()
        {
            var booking = Booking.Create(_userId, _roomId, _checkIn, _checkOut, 2, 100m);

            var newCheckIn = DateTime.UtcNow.AddDays(5);
            var newCheckOut = DateTime.UtcNow.AddDays(6);

            booking.UpdateBookingDates(newCheckIn, newCheckOut);

            Assert.That(booking.CheckIn, Is.EqualTo(newCheckIn));
            Assert.That(booking.CheckOut, Is.EqualTo(newCheckOut));
        }

        [Test]
        public void UpdateBookingDates_Should_Throw_If_Not_Pending()
        {
            var booking = Booking.Create(_userId, _roomId, _checkIn, _checkOut, 2, 100m);
            booking.Confirm();

            var newCheckIn = DateTime.UtcNow.AddDays(5);
            var newCheckOut = DateTime.UtcNow.AddDays(6);

            Assert.Throws<DomainException>(() => booking.UpdateBookingDates(newCheckIn, newCheckOut));
        }

        [Test]
        public void UpdateBookingDates_Should_Throw_If_Invalid_Dates()
        {
            var booking = Booking.Create(_userId, _roomId, _checkIn, _checkOut, 2, 100m);

            var newCheckIn = DateTime.UtcNow.AddDays(6);
            var newCheckOut = DateTime.UtcNow.AddDays(5);

            Assert.Throws<DomainException>(() => booking.UpdateBookingDates(newCheckIn, newCheckOut));
        }

        [Test]
        public void IsDateValid_Should_Return_True_For_Valid_Dates()
        {
            var booking = Booking.Create(_userId, _roomId, _checkIn, _checkOut, 2, 100m);

            Assert.That(booking.IsDateValid(), Is.True);
        }

        [Test]
        public void Create_Should_Throw_If_CheckIn_After_CheckOut()
        {
            // Test that Create method validates dates
            Assert.Throws<DomainException>(() =>
                Booking.Create(_userId, _roomId, _checkOut, _checkIn, 2, 100m));
        }

        [Test]
        public void Create_Should_Throw_If_CheckIn_In_The_Past()
        {
            var pastDate = DateTime.UtcNow.AddDays(-1);
            var futureDate = DateTime.UtcNow.AddDays(1);

            Assert.Throws<DomainException>(() =>
                Booking.Create(_userId, _roomId, pastDate, futureDate, 2, 100m));
        }

        [Test]
        public void UpdateBookingDates_Should_Throw_If_CheckIn_In_Past()
        {
            var booking = Booking.Create(_userId, _roomId, _checkIn, _checkOut, 2, 100m);
            var pastDate = DateTime.UtcNow.AddDays(-1);
            var futureDate = DateTime.UtcNow.AddDays(1);

            Assert.Throws<DomainException>(() =>
                booking.UpdateBookingDates(pastDate, futureDate));
        }

        // Helper method to create bookings with past dates for testing completion scenarios
        private Booking CreateBookingWithPastDates(Guid userId, Guid roomId, DateTime checkIn, DateTime checkOut, int guests, decimal totalAmount)
        {
            // Use reflection to create a booking bypassing validation for test scenarios
            var booking = (Booking)Activator.CreateInstance(typeof(Booking), true);

            // Set properties using reflection
            typeof(Booking).GetProperty("Id").SetValue(booking, Guid.NewGuid());
            typeof(Booking).GetProperty("UserId").SetValue(booking, userId);
            typeof(Booking).GetProperty("RoomId").SetValue(booking, roomId);
            typeof(Booking).GetProperty("CheckIn").SetValue(booking, checkIn);
            typeof(Booking).GetProperty("CheckOut").SetValue(booking, checkOut);
            typeof(Booking).GetProperty("Guests").SetValue(booking, guests);
            typeof(Booking).GetProperty("TotalAmount").SetValue(booking, totalAmount);
            typeof(Booking).GetProperty("Status").SetValue(booking, BookingStatus.Pending);
            typeof(Booking).GetProperty("SpecialRequests").SetValue(booking, string.Empty);
            typeof(Booking).GetProperty("Phone").SetValue(booking, string.Empty);
            typeof(Booking).GetProperty("Address").SetValue(booking, string.Empty);

            return booking;
        }
    }
}