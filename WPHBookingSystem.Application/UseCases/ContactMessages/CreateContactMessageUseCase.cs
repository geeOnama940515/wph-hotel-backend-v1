using System;
using System.Threading.Tasks;
using WPHBookingSystem.Application.Common;
using WPHBookingSystem.Application.DTOs.ContactMessage;
using WPHBookingSystem.Application.Interfaces;
using WPHBookingSystem.Domain.Entities;

namespace WPHBookingSystem.Application.UseCases.ContactMessages
{
    public class CreateContactMessageUseCase
    {
        private readonly IUnitOfWork _unitOfWork;
        public CreateContactMessageUseCase(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Guid>> ExecuteAsync(CreateContactMessageDto dto)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();
                var message = new ContactMessage
                {
                    Id = Guid.NewGuid(),
                    Fullname = dto.Fullname,
                    EmailAddress = dto.EmailAddress,
                    PhoneNumber = dto.PhoneNumber,
                    Subject = dto.Subject,
                    Message = dto.Message,
                    DateCreated = DateTime.UtcNow
                };
                await _unitOfWork.ContactMessageRepository.AddAsync(message);
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();
                return Result<Guid>.Success(message.Id, "Contact message created successfully.");
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return Result<Guid>.Failure($"Failed to create contact message: {ex.Message}", 500);
            }
        }
    }
} 