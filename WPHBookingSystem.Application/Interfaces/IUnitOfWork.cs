using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPHBookingSystem.Domain.Entities;

namespace WPHBookingSystem.Application.Interfaces
{
    public interface IUnitOfWork 
    {
        IRoomRepository Rooms { get; }
        IBookingRepository Bookings { get; }

        Task<int> SaveChangesAsync();
    }
}
