using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using WPHBookingSystem.Application.Interfaces.Services;

namespace WPHBookingSystem.Infrastructure.Services
{
    /// <summary>
    /// Service for handling image uploads and returning filenames.
    /// 
    /// This service wraps the existing ImageService to provide a simpler interface
    /// that returns filenames for database storage.
    /// </summary>
    public class ImageUploadService : IImageUploadService
    {
        private readonly IImageService _imageService;
        private readonly ILogger<ImageUploadService> _logger;

        public ImageUploadService(IImageService imageService, ILogger<ImageUploadService> logger)
        {
            _imageService = imageService;
            _logger = logger;
        }

        /// <summary>
        /// Uploads an image file and returns the filename.
        /// </summary>
        /// <param name="file">The image file to upload</param>
        /// <returns>The filename of the uploaded image</returns>
        public async Task<string> UploadImageAsync(IFormFile file)
        {
            try
            {
                // Generate a temporary room ID for the upload (will be replaced when room is created)
                var tempRoomId = Guid.NewGuid();
                
                var result = await _imageService.UploadImageAsync(file, tempRoomId);
                
                if (result.IsSuccess)
                {
                    _logger.LogInformation("Image uploaded successfully: {FileName}", result.FileName);
                    return result.FileName;
                }
                else
                {
                    _logger.LogError("Failed to upload image: {ErrorMessage}", result.ErrorMessage);
                    throw new InvalidOperationException($"Failed to upload image: {result.ErrorMessage}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading image");
                throw;
            }
        }

        /// <summary>
        /// Uploads multiple image files and returns their filenames.
        /// </summary>
        /// <param name="files">Collection of image files to upload</param>
        /// <returns>Collection of filenames for the uploaded images</returns>
        public async Task<List<string>> UploadImagesAsync(IFormFileCollection files)
        {
            var filenames = new List<string>();
            
            foreach (var file in files)
            {
                if (file != null && file.Length > 0)
                {
                    var filename = await UploadImageAsync(file);
                    filenames.Add(filename);
                }
            }
            
            return filenames;
        }
    }
} 