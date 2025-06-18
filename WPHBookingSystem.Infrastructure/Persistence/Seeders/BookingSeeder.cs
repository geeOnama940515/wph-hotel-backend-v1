using Microsoft.EntityFrameworkCore;
using WPHBookingSystem.Domain.Entities;
using WPHBookingSystem.Domain.Enums;

namespace WPHBookingSystem.Infrastructure.Persistence.Seeders
{
    public static class BookingSeeder
    {
        public static void Seed(ModelBuilder modelBuilder)
        {
            // Predefined GUIDs (do not use Guid.NewGuid())
            var bookingId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
            var userId = Guid.Parse("12345678-90ab-cdef-1234-567890abcdef");
            var roomId = Guid.Parse("11111111-1111-1111-1111-111111111111");
            var bookingToken = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");

            var createdAt = new DateTimeOffset(new DateTime(2025, 6, 1, 12, 0, 0, DateTimeKind.Utc));
            var checkIn = new DateTime(2025, 7, 1, 14, 0, 0, DateTimeKind.Utc);
            var checkOut = new DateTime(2025, 7, 4, 11, 0, 0, DateTimeKind.Utc);

            // 🛏 Seed Room
            modelBuilder.Entity<Room>().HasData(new
            {
                Id = roomId,
                Name = "Deluxe Room 101",
                Description = "A luxurious room with sea view, air conditioning, and mini-bar.",
                Price = 2500m,
                Capacity = 2,
                Status = RoomStatus.Available,
                CreatedAt = createdAt,
                CreatedBy = "Seeder"
            });

            modelBuilder.Entity<Booking>().HasData(new
            {
                Id = bookingId,
                GuestName = "Juan Dela Cruz",
                RoomId = roomId,
                CheckIn = checkIn,
                CheckOut = checkOut,
                Guests = 2,
                EmailAddress = "guest@example.com",
                TotalAmount = 7500m,
                Status = BookingStatus.Confirmed,
                SpecialRequests = "Vegetarian meals only",
                BookingToken = bookingToken,
                CreatedAt = createdAt,
                CreatedBy = "Seeder",
            });

            modelBuilder.Entity<Booking>().OwnsOne(b => b.ContactInfo).HasData(new
            {
                BookingId = bookingId,
                Phone = "09171234567",
                Address = "123 Beach Road, Pagudpud"
            });
        }
    }
}
