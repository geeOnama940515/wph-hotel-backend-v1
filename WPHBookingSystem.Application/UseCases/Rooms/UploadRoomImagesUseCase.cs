using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WPHBookingSystem.Application.Common;
using WPHBookingSystem.Application.DTOs.Room;
using WPHBookingSystem.Application.Interfaces.Services;
using WPHBookingSystem.Domain.Entities;
using WPHBookingSystem.Domain.Exceptions;
using WPHBookingSystem.Domain.ValueObjects;

namespace WPHBookingSystem.Application.UseCases.Rooms
{
    /// <summary>
    /// Use case for uploading images to a room.
    /// 
    /// This use case handles the business logic for uploading multiple images to a room,
    /// including validation, file processing, and updating the room's image collection.
    /// </summary>
    public class UploadRoomImagesUseCase
    {
        private readonly IRoomRepository _roomRepository;
        private readonly IImageService _imageService;
        private readonly IUnitOfWork _unitOfWork;

        public UploadRoomImagesUseCase(
            IRoomRepository roomRepository,
            IImageService imageService,
            IUnitOfWork unitOfWork)
        {
            _roomRepository = roomRepository;
            _imageService = imageService;
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Executes the upload room images use case.
        /// </summary>
        /// <param name="roomId">The ID of the room to upload images for</param>
        /// <param name="files">Collection of image files to upload</param>
        /// <returns>Result containing upload information and any errors</returns>
        public async Task<Result<ImageUploadResponseDto>> ExecuteAsync(Guid roomId, IFormFileCollection files)
        {
            try
            {
                // Validate input
                if (files == null || files.Count == 0)
                {
                    return Result<ImageUploadResponseDto>.Failure(400, "No image files provided");
                }

                // Check if room exists
                var room = await _roomRepository.GetByIdAsync(roomId);
                if (room == null)
                {
                    return Result<ImageUploadResponseDto>.Failure(404, "Room not found");
                }

                // Upload images
                var uploadResults = await _imageService.UploadImagesAsync(files, roomId);

                // Process results
                var successfulUploads = uploadResults.Where(r => r.IsSuccess).ToList();
                var failedUploads = uploadResults.Where(r => !r.IsSuccess).ToList();

                // Update room with new images
                if (successfulUploads.Any())
                {
                    var newImages = successfulUploads.Select(upload => new GalleryImage
                    {
                        FileName = upload.FileName
                    }).ToList();

                    // Add new images to existing ones
                    var updatedImages = room.Images.ToList();
                    updatedImages.AddRange(newImages);
                    
                    room.UpdateDetails(
                        room.Name,
                        room.Description,
                        room.Price,
                        room.Capacity,
                        updatedImages
                    );

                    await _roomRepository.UpdateAsync(room);
                    await _unitOfWork.SaveChangesAsync();
                }

                // Build response
                var response = new ImageUploadResponseDto
                {
                    Success = successfulUploads.Any(),
                    Message = successfulUploads.Any() 
                        ? $"Successfully uploaded {successfulUploads.Count} image(s)" 
                        : "No images were uploaded successfully",
                    Images = uploadResults.Select(r => new ImageInfoDto
                    {
                        FileName = r.FileName,
                        Url = r.Url,
                        FileSize = r.FileSize,
                        ContentType = r.ContentType,
                        IsSuccess = r.IsSuccess,
                        ErrorMessage = r.ErrorMessage
                    }).ToList(),
                    Errors = failedUploads.Select(r => r.ErrorMessage ?? "Unknown error").ToList()
                };

                var statusCode = successfulUploads.Any() ? 200 : 400;
                return Result<ImageUploadResponseDto>.Success(response, statusCode);
            }
            catch (DomainException ex)
            {
                return Result<ImageUploadResponseDto>.Failure(400, ex.Message);
            }
            catch (Exception ex)
            {
                return Result<ImageUploadResponseDto>.Failure(500, "An error occurred while uploading images");
            }
        }
    }
} 