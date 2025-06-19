using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPHBookingSystem.Domain.Entities;

namespace WPHBookingSystem.Application.Interfaces.Common
{
    public interface IDbContext
    {
        public DbSet<Room> Rooms {get;}
        public DbSet<Booking> Bookings {get;}
        public DbSet<ContactMessage> ContactMessages {get;}
    }
}
