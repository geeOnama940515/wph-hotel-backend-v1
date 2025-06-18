using System;
using System.ComponentModel.DataAnnotations;
using WPHBookingSystem.Domain.Enums;
using WPHBookingSystem.Domain.ValueObjects;

namespace WPHBookingSystem.Application.DTOs.Room
{/// <summary>
/// mostly for projection only, I still want to add validation just to be sure that the data is valid.
/// and consistent throughout the whole application.
/// </summary>
    public class RoomDto
    {
        public Guid Id { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Room name cannot exceed 100 characters.")]
        public string Name { get; set; } = string.Empty;

        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters.")]
        public string Description { get; set; } = string.Empty;

        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0.")]
        public decimal Price { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Capacity must be at least 1.")]
        public int Capacity { get; set; }

        // [StringLength(500, ErrorMessage = "Image URL cannot exceed 500 characters.")]
        public List<GalleryImage>? Images { get; set; } = new();

        public RoomStatus Status { get; set; }
    }
}
