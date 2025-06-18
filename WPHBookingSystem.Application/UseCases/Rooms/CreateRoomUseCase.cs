using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;
using WPHBookingSystem.Application.Common;
using WPHBookingSystem.Application.DTOs.Room;
using WPHBookingSystem.Application.Interfaces;
using WPHBookingSystem.Application.Interfaces.Services;
using WPHBookingSystem.Domain.Entities;
using WPHBookingSystem.Domain.ValueObjects;

namespace WPHBookingSystem.Application.UseCases.Rooms
{
    /// <summary>
    /// Use case responsible for creating new rooms in the hotel booking system.
    /// Handles room creation business logic and persistence within a transaction.
    /// </summary>
    public class CreateRoomUseCase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IImageUploadService _imageUploadService;

        public CreateRoomUseCase(IUnitOfWork unitOfWork, IImageUploadService imageUploadService)
        {
            _unitOfWork = unitOfWork;
            _imageUploadService = imageUploadService;
        }

        /// <summary>
        /// Creates a room without images.
        /// </summary>
        public async Task<Result<Guid>> ExecuteAsync(CreateRoomDto dto)
        {
            return await ExecuteAsync(dto, null);
        }

        /// <summary>
        /// Creates a room with optional images.
        /// </summary>
        public async Task<Result<Guid>> ExecuteAsync(CreateRoomDto dto, IFormFileCollection? images = null)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                // Process images first if provided
                List<GalleryImage> galleryImages = new();
                if (images != null && images.Count > 0)
                {
                    var validImages = images.Where(f => f != null && f.Length > 0).ToList();
                    if (validImages.Any())
                    {
                        // Upload images and get filenames
                        var filenames = await _imageUploadService.UploadImagesAsync(images);
                        
                        // Create GalleryImage objects from filenames
                        galleryImages = filenames.Select(filename => new GalleryImage { FileName = filename }).ToList();
                    }
                }

                // Create room with images
                var room = Room.Create(dto.Name, dto.Description, dto.Price, dto.Capacity, galleryImages);

                await _unitOfWork.RoomRepository.AddAsync(room);
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();

                return Result<Guid>.Success(room.Id, "Room created successfully.");
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return Result<Guid>.Failure($"Failed to create room: {ex.Message}", 500);
            }
        }
    }
}
