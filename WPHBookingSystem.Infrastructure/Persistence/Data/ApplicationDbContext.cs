using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using WPHBookingSystem.Application.Interfaces.Common;
using WPHBookingSystem.Domain.Entities;
using WPHBookingSystem.Infrastructure.Identity;
using WPHBookingSystem.Infrastructure.Persistence.Seeders;

namespace WPHBookingSystem.Infrastructure.Persistence.Data
{
    /// <summary>
    /// Entity Framework Core DbContext for the hotel booking system.
    /// Extends IdentityDbContext to support ASP.NET Core Identity for user management
    /// and implements IDbContext interface for application layer integration.
    /// 
    /// This context manages the database schema and provides access to all entities
    /// in the system including rooms, bookings, and user accounts.
    /// </summary>
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>, IDbContext
    {
        /// <summary>
        /// Initializes a new instance of the ApplicationDbContext with the specified options.
        /// </summary>
        /// <param name="options">The DbContext options containing connection string and configuration.</param>
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        /// <summary>
        /// Gets the DbSet for Room entities.
        /// Provides access to room data for CRUD operations and queries.
        /// </summary>
        public DbSet<Room> Rooms => Set<Room>();

        /// <summary>
        /// Gets the DbSet for Booking entities.
        /// Provides access to booking data for CRUD operations and queries.
        /// </summary>
        public DbSet<Booking> Bookings => Set<Booking>();

        /// <summary>
        /// Gets the DbSet for ContactMessage entities.
        /// Provides access to contact message data for CRUD operations and queries.
        /// </summary>
        public DbSet<ContactMessage> ContactMessages => Set<ContactMessage>();

        /// <summary>
        /// Gets the DbSet for OtpVerification entities.
        /// Provides access to OTP verification data for CRUD operations and queries.
        /// </summary>
        public DbSet<OtpVerification> OtpVerifications => Set<OtpVerification>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Booking>(b =>
            {
                b.OwnsOne(b => b.ContactInfo);
            });

            builder.Entity<Room>(room =>
            {
                room.OwnsMany(r => r.Images, image =>
                {
                    image.WithOwner().HasForeignKey("RoomId");
                    image.Property(i => i.FileName).HasColumnName("FileName");
                    image.HasKey("RoomId", "FileName");
                });
            });

            BookingSeeder.Seed(builder);
            UserRoleSeeder.Seed(builder);
        }

    }
}
