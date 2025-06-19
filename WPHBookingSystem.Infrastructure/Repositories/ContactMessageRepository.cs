using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WPHBookingSystem.Application.Interfaces;
using WPHBookingSystem.Domain.Entities;
using WPHBookingSystem.Infrastructure.Persistence.Data;

namespace WPHBookingSystem.Infrastructure.Repositories
{
    public class ContactMessageRepository : GenericRepository<ContactMessage>, IContactMessageRepository
    {
        private readonly ApplicationDbContext _context;
        public ContactMessageRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<ContactMessage?> GetByIdAsync(Guid id)
        {
            return await _context.ContactMessages.FindAsync(id);
        }

        public async Task<List<ContactMessage>> GetAllAsync()
        {
            return await _context.ContactMessages.OrderByDescending(m => m.DateCreated).ToListAsync();
        }

        public async Task AddAsync(ContactMessage message)
        {
            await _context.ContactMessages.AddAsync(message);
        }

        public async Task UpdateAsync(ContactMessage message)
        {
            _context.ContactMessages.Update(message);
        }

        public async Task DeleteAsync(Guid id)
        {
            var message = await _context.ContactMessages.FindAsync(id);
            if (message != null)
            {
                _context.ContactMessages.Remove(message);
            }
        }
    }
} 