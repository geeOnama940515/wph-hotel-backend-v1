using System;
using System.ComponentModel.DataAnnotations;
using WPHBookingSystem.Domain.Enums;

namespace WPHBookingSystem.Application.DTOs.Booking
{
    public class BookingDto
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Guest Name is required.")]
        public string GuestName { get; set; } = string.Empty;

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

        [Required(ErrorMessage = "Booking status is required.")]
        public BookingStatus Status { get; set; }

        [StringLength(1000, ErrorMessage = "Special requests cannot exceed 1000 characters.")]
        public string SpecialRequests { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Phone number is not valid.")]
        [StringLength(20, ErrorMessage = "Phone number cannot exceed 20 characters.")]
        public string Phone { get; set; } = string.Empty;

        [StringLength(200, ErrorMessage = "Address cannot exceed 200 characters.")]
        public string Address { get; set; } = string.Empty;

        public string? RoomName { get; set; } // Optional projection
    }
}
