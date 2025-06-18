using Microsoft.AspNetCore.Http;
using WPHBookingSystem.Application.Common;
using WPHBookingSystem.Application.DTOs.Room;
using WPHBookingSystem.Application.Exceptions;
using WPHBookingSystem.Application.Interfaces;
using WPHBookingSystem.Application.Interfaces.Services;
using WPHBookingSystem.Domain.Entities;
using WPHBookingSystem.Domain.ValueObjects;

namespace WPHBookingSystem.Application.UseCases.Rooms
{
    public class UpdateRoomWithImagesUseCase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IImageService _imageService;

        public UpdateRoomWithImagesUseCase(
            IUnitOfWork unitOfWork,
            IImageService imageService)
        {
            _unitOfWork = unitOfWork;
            _imageService = imageService;
        }

        public async Task<Result<RoomDto>> ExecuteAsync(Guid roomId, UpdateRoomWithImagesDto request)
        {
            try
            {
                // Get existing room
                var existingRoom = await _unitOfWork.RoomRepository.GetByIdAsync(roomId);
                if (existingRoom == null)
                {
                    return Result<RoomDto>.Failure($"Room with ID {roomId} not found.", 404);
                }

                // Handle images based on the replace flag
                List<GalleryImage> finalImages;
                
                if (request.NewImages != null && request.NewImages.Any())
                {
                    // Create a custom IFormFileCollection for the new images
                    var fileCollection = new CustomFormFileCollection(request.NewImages);
                    var uploadResults = await _imageService.UploadImagesAsync(fileCollection, roomId);
                    
                    // Get successful uploads
                    var successfulUploads = uploadResults.Where(r => r.IsSuccess).ToList();
                    var newGalleryImages = successfulUploads.Select(upload => new GalleryImage
                    {
                        FileName = upload.FileName
                    }).ToList();

                    if (request.ReplaceExistingImages)
                    {
                        // Replace existing images - delete old files from server
                        await DeleteExistingImagesFromServer(existingRoom.Images);
                        finalImages = newGalleryImages;
                    }
                    else
                    {
                        // Add to existing images
                        finalImages = new List<GalleryImage>(existingRoom.Images);
                        finalImages.AddRange(newGalleryImages);
                    }
                }
                else
                {
                    // No new images provided, keep existing ones
                    finalImages = new List<GalleryImage>(existingRoom.Images);
                }

                // Update room properties using the domain method
                existingRoom.UpdateDetails(request.Name, request.Description, request.Price, request.Capacity, finalImages);

                // Update the room
                await _unitOfWork.RoomRepository.UpdateAsync(existingRoom);
                await _unitOfWork.SaveChangesAsync();

                // Return updated room DTO
                var roomDto = new RoomDto
                {
                    Id = existingRoom.Id,
                    Name = existingRoom.Name,
                    Description = existingRoom.Description,
                    Price = existingRoom.Price,
                    Capacity = existingRoom.Capacity,
                    Status = existingRoom.Status,
                    Images = existingRoom.Images
                };

                var message = request.ReplaceExistingImages 
                    ? "Room updated successfully with replaced images." 
                    : "Room updated successfully with new images added.";

                return Result<RoomDto>.Success(roomDto, message);
            }
            catch (Exception ex)
            {
                return Result<RoomDto>.Failure($"Failed to update room with images: {ex.Message}", 500);
            }
        }

        /// <summary>
        /// Deletes existing image files from the server when replacing images.
        /// </summary>
        private async Task DeleteExistingImagesFromServer(List<GalleryImage> existingImages)
        {
            try
            {
                foreach (var image in existingImages)
                {
                    await _imageService.DeleteImageAsync(image.FileName);
                }
            }
            catch (Exception ex)
            {
                // Log the error but don't fail the operation
                // The database update will still succeed even if file deletion fails
                // You might want to inject ILogger here for proper logging
                Console.WriteLine($"Warning: Failed to delete some image files: {ex.Message}");
            }
        }
    }

    // Custom IFormFileCollection implementation for the new images
    public class CustomFormFileCollection : IFormFileCollection
    {
        private readonly List<IFormFile> _files;

        public CustomFormFileCollection(List<IFormFile> files)
        {
            _files = files ?? new List<IFormFile>();
        }

        public IFormFile? this[string name] => _files.FirstOrDefault(f => f.Name == name);

        public int Count => _files.Count;

        public IFormFile this[int index] => throw new NotImplementedException();

        public IEnumerator<IFormFile> GetEnumerator() => _files.GetEnumerator();

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();

        public IFormFile? GetFile(string name) => this[name];

        public IReadOnlyList<IFormFile> GetFiles(string name) => _files.Where(f => f.Name == name).ToList();
    }
} 