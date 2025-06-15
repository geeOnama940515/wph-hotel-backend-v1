using System;
using System.ComponentModel.DataAnnotations;
using WPHBookingSystem.Domain.ValueObjects;

namespace WPHBookingSystem.Application.DTOs.Room
{
    public class UpdateRoomDto
    {
        [Required(ErrorMessage = "Room ID is required.")]
        public Guid RoomId { get; set; }

        [Required(ErrorMessage = "Room name is required.")]
        [StringLength(100, ErrorMessage = "Room name cannot exceed 100 characters.")]
        public string Name { get; set; } = string.Empty;

        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters.")]
        public string Description { get; set; } = string.Empty;

        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0.")]
        public decimal Price { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Capacity must be at least 1.")]
        public int Capacity { get; set; }

        // [StringLength(500, ErrorMessage = "Image URL cannot exceed 500 characters.")]
        public List<GalleryImage> Images { get; set; } = new();
    }
}
