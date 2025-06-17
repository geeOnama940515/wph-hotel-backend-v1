using System.ComponentModel.DataAnnotations;

namespace WPHBookingSystem.Application.DTOs.Identity
{
    public class LoginRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
} 