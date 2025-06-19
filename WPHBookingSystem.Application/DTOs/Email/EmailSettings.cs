namespace WPHBookingSystem.Application.DTOs.Email
{
    /// <summary>
    /// Configuration settings for the email service.
    /// </summary>
    public class EmailSettings
    {
        /// <summary>
        /// SMTP server hostname or IP address.
        /// </summary>
        public string SmtpHost { get; set; } = string.Empty;

        /// <summary>
        /// SMTP server port number.
        /// </summary>
        public int SmtpPort { get; set; } = 587;

        /// <summary>
        /// Email address used as the sender (From address).
        /// </summary>
        public string FromEmail { get; set; } = string.Empty;

        /// <summary>
        /// Display name for the sender email.
        /// </summary>
        public string FromName { get; set; } = string.Empty;

        /// <summary>
        /// Username for SMTP authentication.
        /// </summary>
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// Password for SMTP authentication.
        /// </summary>
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// Whether to enable SSL/TLS for SMTP connection.
        /// </summary>
        public bool EnableSsl { get; set; } = true;

        /// <summary>
        /// Whether to enable SMTP authentication.
        /// </summary>
        public bool EnableAuthentication { get; set; } = true;

        /// <summary>
        /// Base URL for the application (used for generating booking summary links).
        /// </summary>
        public string BaseUrl { get; set; } = "https://wph-hotel.gregdoesdev.xyz";

        /// <summary>
        /// Hotel information for email templates.
        /// </summary>
        public HotelInfo HotelInfo { get; set; } = new();
    }

    /// <summary>
    /// Hotel information used in email templates.
    /// </summary>
    public class HotelInfo
    {
        /// <summary>
        /// Hotel name.
        /// </summary>
        public string Name { get; set; } = "WPH Hotel";

        /// <summary>
        /// Hotel address.
        /// </summary>
        public string Address { get; set; } = string.Empty;

        /// <summary>
        /// Hotel phone number.
        /// </summary>
        public string Phone { get; set; } = string.Empty;

        /// <summary>
        /// Hotel email address.
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Hotel website URL.
        /// </summary>
        public string Website { get; set; } = string.Empty;

        /// <summary>
        /// Hotel logo URL (optional).
        /// </summary>
        public string LogoUrl { get; set; } = string.Empty;
    }
} 