using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPHBookingSystem.Domain.Entities;
using WPHBookingSystem.Domain.Enums;
using WPHBookingSystem.Domain.ValueObjects;

namespace WPHBookingSystem.Infrastructure.Persistence.Seeders
{
    public class BookingBuilder
    {
        private readonly Booking _booking = (Booking)Activator.CreateInstance(typeof(Booking), true)!;

        public BookingBuilder WithId(Guid id) { typeof(Booking).GetProperty("Id")!.SetValue(_booking, id); return this; }
        public BookingBuilder WithUserId(Guid userId) { typeof(Booking).GetProperty("UserId")!.SetValue(_booking, userId); return this; }
        public BookingBuilder WithRoomId(Guid roomId) { typeof(Booking).GetProperty("RoomId")!.SetValue(_booking, roomId); return this; }
        public BookingBuilder WithCheckIn(DateTime checkIn) { typeof(Booking).GetProperty("CheckIn")!.SetValue(_booking, checkIn); return this; }
        public BookingBuilder WithCheckOut(DateTime checkOut) { typeof(Booking).GetProperty("CheckOut")!.SetValue(_booking, checkOut); return this; }
        public BookingBuilder WithGuests(int guests) { typeof(Booking).GetProperty("Guests")!.SetValue(_booking, guests); return this; }
        public BookingBuilder WithEmail(string email) { typeof(Booking).GetProperty("EmailAddress")!.SetValue(_booking, email); return this; }
        public BookingBuilder WithContactInfo(string phone, string address)
        {
            typeof(Booking).GetProperty("ContactInfo")!.SetValue(_booking, new ContactInfo(phone, address));
            return this;
        }
        public BookingBuilder WithTotalAmount(decimal total) { typeof(Booking).GetProperty("TotalAmount")!.SetValue(_booking, total); return this; }
        public BookingBuilder WithStatus(BookingStatus status) { typeof(Booking).GetProperty("Status")!.SetValue(_booking, status); return this; }
        public BookingBuilder WithSpecialRequests(string note) { typeof(Booking).GetProperty("SpecialRequests")!.SetValue(_booking, note); return this; }
        public BookingBuilder WithToken(Guid token) { typeof(Booking).GetProperty("BookingToken")!.SetValue(_booking, token); return this; }
        public BookingBuilder WithCreatedBy(string createdBy) { typeof(Booking).GetProperty("CreatedBy")!.SetValue(_booking, createdBy); return this; }
        public Booking Build() => _booking;
    }

}
