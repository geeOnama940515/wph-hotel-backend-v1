using System;
using System.ComponentModel.DataAnnotations;

namespace WPHBookingSystem.Application.DTOs.Booking
{
    public class CreateBookingDto
    {
        [Required(ErrorMessage = "Room ID is required.")]
        public Guid RoomId { get; set; }

        [Required(ErrorMessage = "Check-in date is required.")]
        public DateTime CheckIn { get; set; }

        [Required(ErrorMessage = "Check-out date is required.")]
        public DateTime CheckOut { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Guests must be at least 1.")]
        public int Guests { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Total amount must be greater than 0.")]
        public decimal TotalAmount { get; set; }

        [StringLength(1000, ErrorMessage = "Special requests cannot exceed 1000 characters.")]
        public string SpecialRequests { get; set; } = string.Empty;

        [Required(ErrorMessage = "Phone number is required.")]
        [Phone(ErrorMessage = "Phone number is not valid.")]
        [StringLength(20, ErrorMessage = "Phone number cannot exceed 20 characters.")]
        public string Phone { get; set; } = string.Empty;

        [Required(ErrorMessage = "Address is required.")]
        [StringLength(200, ErrorMessage = "Address cannot exceed 200 characters.")]
        public string Address { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email address is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string EmailAddress { get; set; } = string.Empty;


        [Required(ErrorMessage = "Gueest Name is required.")]
        [StringLength(50, ErrorMessage = "Guest Name cannot exceed 50 characters.")]
        public string GuestName { get; set; } = string.Empty;
    }
}
