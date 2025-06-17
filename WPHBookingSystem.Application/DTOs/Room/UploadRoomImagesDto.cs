using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace WPHBookingSystem.Application.DTOs.Room
{
    /// <summary>
    /// DTO for uploading images to a room.
    /// </summary>
    public class UploadRoomImagesDto
    {
        /// <summary>
        /// The ID of the room to upload images for.
        /// </summary>
        [Required(ErrorMessage = "Room ID is required.")]
        public Guid RoomId { get; set; }

        /// <summary>
        /// Collection of image files to upload.
        /// </summary>
        [Required(ErrorMessage = "At least one image file is required.")]
        public IFormFileCollection Images { get; set; } = null!;
    }

    /// <summary>
    /// Response DTO for image upload operations.
    /// </summary>
    public class ImageUploadResponseDto
    {
        /// <summary>
        /// Indicates if the upload was successful.
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Message describing the result of the upload operation.
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// Collection of uploaded image information.
        /// </summary>
        public List<ImageInfoDto> Images { get; set; } = new();

        /// <summary>
        /// Collection of any errors that occurred during upload.
        /// </summary>
        public List<string> Errors { get; set; } = new();
    }

    /// <summary>
    /// DTO containing information about an uploaded image.
    /// </summary>
    public class ImageInfoDto
    {
        /// <summary>
        /// The filename of the uploaded image.
        /// </summary>
        public string FileName { get; set; } = string.Empty;

        /// <summary>
        /// The URL where the image can be accessed.
        /// </summary>
        public string Url { get; set; } = string.Empty;

        /// <summary>
        /// The size of the image file in bytes.
        /// </summary>
        public long FileSize { get; set; }

        /// <summary>
        /// The MIME content type of the image.
        /// </summary>
        public string ContentType { get; set; } = string.Empty;

        /// <summary>
        /// Indicates if this specific image upload was successful.
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <summary>
        /// Error message if the upload failed.
        /// </summary>
        public string? ErrorMessage { get; set; }
    }
} 