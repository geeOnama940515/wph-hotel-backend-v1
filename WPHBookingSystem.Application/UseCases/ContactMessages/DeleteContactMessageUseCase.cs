using System;
using System.Threading.Tasks;
using WPHBookingSystem.Application.Common;
using WPHBookingSystem.Application.Interfaces;

namespace WPHBookingSystem.Application.UseCases.ContactMessages
{
    public class DeleteContactMessageUseCase
    {
        private readonly IUnitOfWork _unitOfWork;
        public DeleteContactMessageUseCase(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> ExecuteAsync(Guid id)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();
                await _unitOfWork.ContactMessageRepository.DeleteAsync(id);
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();
                return Result.Success("Contact message deleted successfully.");
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return Result.Failure($"Failed to delete contact message: {ex.Message}", 500);
            }
        }
    }
} 