using Microsoft.AspNetCore.Http;

namespace WPHBookingSystem.Application.Interfaces.Services
{
    /// <summary>
    /// Service interface for handling image uploads.
    /// 
    /// This service provides functionality for uploading images and returning
    /// the filename for storage in the database.
    /// </summary>
    public interface IImageUploadService
    {
        /// <summary>
        /// Uploads an image file and returns the filename.
        /// </summary>
        /// <param name="file">The image file to upload</param>
        /// <returns>The filename of the uploaded image</returns>
        Task<string> UploadImageAsync(IFormFile file);

        /// <summary>
        /// Uploads multiple image files and returns their filenames.
        /// </summary>
        /// <param name="files">Collection of image files to upload</param>
        /// <returns>Collection of filenames for the uploaded images</returns>
        Task<List<string>> UploadImagesAsync(IFormFileCollection files);
    }
} 