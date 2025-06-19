using System;

namespace WPHBookingSystem.Application.DTOs.ContactMessage
{
    public class ContactMessageDto
    {
        public Guid Id { get; set; }
        public string Fullname { get; set; } = string.Empty;
        public string EmailAddress { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public DateTime DateCreated { get; set; }
    }
} 