using System;

namespace WPHBookingSystem.Domain.Entities
{
    public class ContactMessage
    {
        public Guid Id { get; set; }
        public string Fullname { get; set; } = string.Empty;
        public string EmailAddress { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public DateTime DateCreated { get; set; } = DateTime.UtcNow;
    }
} 