using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPHBookingSystem.Domain.Entities;
using WPHBookingSystem.Domain.Enums;

namespace WPHBookingSystem.Domain.Repositories
{
    public interface IBookingRepository
    {
        Task<Booking> GetByIdAsync(Guid id);
        Task<IEnumerable<Booking>> ListAsync(Guid? userId = null, BookingStatus? status = null);
        Task AddAsync(Booking booking);
        Task UpdateAsync(Booking booking);
        Task DeleteAsync(Booking booking);
    }
}
