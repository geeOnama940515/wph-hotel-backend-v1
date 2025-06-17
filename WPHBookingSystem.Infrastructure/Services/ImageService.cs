using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WPHBookingSystem.Application.Interfaces.Services;

namespace WPHBookingSystem.Infrastructure.Services
{
    /// <summary>
    /// Service for handling image uploads and management.
    /// 
    /// This service provides functionality for uploading, storing, and managing
    /// images for hotel rooms, including validation, file processing, and URL generation.
    /// </summary>
    public class ImageService : IImageService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<ImageService> _logger;
        private readonly string _uploadPath;
        private readonly string _baseUrl;
        private readonly long _maxFileSize;
        private readonly string[] _allowedExtensions;

        public ImageService(IConfiguration configuration, ILogger<ImageService> logger)
        {
            _configuration = configuration;
            _logger = logger;
            
            // Get configuration values
            _uploadPath = _configuration["ImageSettings:UploadPath"] ?? "wwwroot/images/rooms";
            _baseUrl = _configuration["ImageSettings:BaseUrl"] ?? "/images/rooms";
            _maxFileSize = long.Parse(_configuration["ImageSettings:MaxFileSize"] ?? "5242880"); // 5MB default
            _allowedExtensions = _configuration.GetSection("ImageSettings:AllowedExtensions")
                .Get<string[]>() ?? new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };

            // Ensure upload directory exists
            EnsureUploadDirectoryExists();
        }

        /// <summary>
        /// Uploads a single image file and returns the file information.
        /// </summary>
        public async Task<ImageUploadResult> UploadImageAsync(IFormFile file, Guid roomId)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return new ImageUploadResult
                    {
                        IsSuccess = false,
                        ErrorMessage = "No file provided"
                    };
                }

                if (!IsValidImage(file))
                {
                    return new ImageUploadResult
                    {
                        IsSuccess = false,
                        ErrorMessage = "Invalid image file"
                    };
                }

                // Generate unique filename
                var fileName = GenerateFileName(file, roomId);
                var filePath = Path.Combine(_uploadPath, fileName);

                // Save file
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                var result = new ImageUploadResult
                {
                    FileName = fileName,
                    Url = GetImageUrl(fileName),
                    FileSize = file.Length,
                    ContentType = file.ContentType,
                    IsSuccess = true
                };

                _logger.LogInformation("Image uploaded successfully: {FileName} for room {RoomId}", fileName, roomId);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading image for room {RoomId}", roomId);
                return new ImageUploadResult
                {
                    IsSuccess = false,
                    ErrorMessage = "Error uploading image"
                };
            }
        }

        /// <summary>
        /// Uploads multiple image files for a room.
        /// </summary>
        public async Task<List<ImageUploadResult>> UploadImagesAsync(IFormFileCollection files, Guid roomId)
        {
            var results = new List<ImageUploadResult>();

            foreach (var file in files)
            {
                var result = await UploadImageAsync(file, roomId);
                results.Add(result);
            }

            return results;
        }

        /// <summary>
        /// Deletes an image file from storage.
        /// </summary>
        public async Task<bool> DeleteImageAsync(string fileName)
        {
            try
            {
                var filePath = Path.Combine(_uploadPath, fileName);
                
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                    _logger.LogInformation("Image deleted successfully: {FileName}", fileName);
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting image: {FileName}", fileName);
                return false;
            }
        }

        /// <summary>
        /// Validates if a file is a valid image.
        /// </summary>
        public bool IsValidImage(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return false;

            // Check file size
            if (file.Length > _maxFileSize)
                return false;

            // Check file extension
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!_allowedExtensions.Contains(extension))
                return false;

            // Check content type
            var allowedContentTypes = new[]
            {
                "image/jpeg",
                "image/jpg",
                "image/png",
                "image/gif",
                "image/webp"
            };

            return allowedContentTypes.Contains(file.ContentType.ToLowerInvariant());
        }

        /// <summary>
        /// Gets the full URL for an image file.
        /// </summary>
        public string GetImageUrl(string fileName)
        {
            return $"{_baseUrl}/{fileName}";
        }

        /// <summary>
        /// Ensures the upload directory exists.
        /// </summary>
        private void EnsureUploadDirectoryExists()
        {
            if (!Directory.Exists(_uploadPath))
            {
                Directory.CreateDirectory(_uploadPath);
                _logger.LogInformation("Created upload directory: {UploadPath}", _uploadPath);
            }
        }

        /// <summary>
        /// Generates a unique filename for the uploaded image.
        /// </summary>
        private string GenerateFileName(IFormFile file, Guid roomId)
        {
            var extension = Path.GetExtension(file.FileName);
            var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
            var random = Guid.NewGuid().ToString("N")[..8];
            
            return $"room-{roomId}-{timestamp}-{random}{extension}";
        }
    }
} 