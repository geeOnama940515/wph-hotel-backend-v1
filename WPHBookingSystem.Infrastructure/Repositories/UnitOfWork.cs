using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPHBookingSystem.Application.Interfaces;
using WPHBookingSystem.Infrastructure.Persistence.Data;

namespace WPHBookingSystem.Infrastructure.Repositories
{
    public class UnitOfWork(ApplicationDbContext context) : IUnitOfWork
    {
        private readonly ApplicationDbContext _context = context;
        public IRoomRepository Rooms => throw new NotImplementedException();

        public IBookingRepository Bookings => throw new NotImplementedException();

        public async Task<int> SaveChangesAsync()
        {
           var result = await _context.SaveChangesAsync();
           return result;
        }
    }
}
