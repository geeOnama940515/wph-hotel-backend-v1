
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPHBookingSystem.Application.Interfaces.Common;
using WPHBookingSystem.Domain.Entities;
using WPHBookingSystem.Infrastructure.Identity;

namespace WPHBookingSystem.Infrastructure.Persistence.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>, IDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        public DbSet<Room> Rooms => Set<Room>();

        public DbSet<Booking> Bookings => Set<Booking>();
    }
}
