using Microsoft.AspNetCore.Http;

namespace WPHBookingSystem.Application.Interfaces.Services
{
    /// <summary>
    /// Service interface for handling image uploads and management.
    /// 
    /// This service provides functionality for uploading, storing, and managing
    /// images for hotel rooms, including validation, file processing, and URL generation.
    /// </summary>
    public interface IImageService
    {
        /// <summary>
        /// Uploads a single image file and returns the file information.
        /// </summary>
        /// <param name="file">The image file to upload</param>
        /// <param name="roomId">The ID of the room this image belongs to</param>
        /// <returns>File information including filename and URL</returns>
        Task<ImageUploadResult> UploadImageAsync(IFormFile file, Guid roomId);

        /// <summary>
        /// Uploads multiple image files for a room.
        /// </summary>
        /// <param name="files">Collection of image files to upload</param>
        /// <param name="roomId">The ID of the room these images belong to</param>
        /// <returns>Collection of uploaded file information</returns>
        Task<List<ImageUploadResult>> UploadImagesAsync(IFormFileCollection files, Guid roomId);

        /// <summary>
        /// Deletes an image file from storage.
        /// </summary>
        /// <param name="fileName">The filename of the image to delete</param>
        /// <returns>True if deletion was successful</returns>
        Task<bool> DeleteImageAsync(string fileName);

        /// <summary>
        /// Validates if a file is a valid image.
        /// </summary>
        /// <param name="file">The file to validate</param>
        /// <returns>True if the file is a valid image</returns>
        bool IsValidImage(IFormFile file);

        /// <summary>
        /// Gets the full URL for an image file.
        /// </summary>
        /// <param name="fileName">The filename of the image</param>
        /// <returns>The full URL to access the image</returns>
        string GetImageUrl(string fileName);
    }

    /// <summary>
    /// Result of an image upload operation.
    /// </summary>
    public class ImageUploadResult
    {
        public string FileName { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public long FileSize { get; set; }
        public string ContentType { get; set; } = string.Empty;
        public bool IsSuccess { get; set; }
        public string? ErrorMessage { get; set; }
    }
} 