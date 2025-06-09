using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPHBookingSystem.Domain.Services
{
    public interface IBookingDomainService
    {
        Task<bool> IsBookingDateAvailable(Guid roomId, DateTime checkIn, DateTime checkOut);
    }
}
