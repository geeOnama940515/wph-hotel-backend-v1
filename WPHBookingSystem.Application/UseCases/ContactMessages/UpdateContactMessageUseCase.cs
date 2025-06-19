using System;
using System.Threading.Tasks;
using WPHBookingSystem.Application.Common;
using WPHBookingSystem.Application.DTOs.ContactMessage;
using WPHBookingSystem.Application.Interfaces;

namespace WPHBookingSystem.Application.UseCases.ContactMessages
{
    public class UpdateContactMessageUseCase
    {
        private readonly IUnitOfWork _unitOfWork;
        public UpdateContactMessageUseCase(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> ExecuteAsync(Guid id, UpdateContactMessageDto dto)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();
                var message = await _unitOfWork.ContactMessageRepository.GetByIdAsync(id);
                if (message == null)
                {
                    return Result.Failure("Contact message not found.", 404);
                }
                message.Fullname = dto.Fullname;
                message.EmailAddress = dto.EmailAddress;
                message.PhoneNumber = dto.PhoneNumber;
                message.Subject = dto.Subject;
                message.Message = dto.Message;
                await _unitOfWork.ContactMessageRepository.UpdateAsync(message);
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();
                return Result.Success("Contact message updated successfully.");
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return Result.Failure($"Failed to update contact message: {ex.Message}", 500);
            }
        }
    }
} 