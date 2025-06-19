namespace WPHBookingSystem.Application.DTOs.ContactMessage
{
    public class UpdateContactMessageDto
    {
        public string Fullname { get; set; } = string.Empty;
        public string EmailAddress { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }
} 