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

                // Handle new images if provided
                var allImages = new List<GalleryImage>(existingRoom.Images);
                if (request.NewImages != null && request.NewImages.Any())
                {
                    // Create a custom IFormFileCollection for the new images
                    var fileCollection = new CustomFormFileCollection(request.NewImages);
                    var uploadResults = await _imageService.UploadImagesAsync(fileCollection, roomId);
                    
                    // Add successful uploads to the collection
                    var successfulUploads = uploadResults.Where(r => r.IsSuccess).ToList();
                    foreach (var upload in successfulUploads)
                    {
                        allImages.Add(new GalleryImage
                        {
                            FileName = upload.FileName
                        });
                    }
                }

                // Update room properties using the domain method
                existingRoom.UpdateDetails(request.Name, request.Description, request.Price, request.Capacity, allImages);

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

                return Result<RoomDto>.Success(roomDto, "Room updated successfully with new images.");
            }
            catch (Exception ex)
            {
                return Result<RoomDto>.Failure($"Failed to update room with images: {ex.Message}", 500);
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