using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WPHBookingSystem.Domain.Entities;

namespace WPHBookingSystem.Application.Interfaces
{
    public interface IContactMessageRepository
    {
        Task<ContactMessage?> GetByIdAsync(Guid id);
        Task<List<ContactMessage>> GetAllAsync();
        Task AddAsync(ContactMessage message);
        Task UpdateAsync(ContactMessage message);
        Task DeleteAsync(Guid id);
    }
} 