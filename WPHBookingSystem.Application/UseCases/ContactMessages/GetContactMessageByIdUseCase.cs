using System;
using System.Threading.Tasks;
using WPHBookingSystem.Application.DTOs.ContactMessage;
using WPHBookingSystem.Application.Interfaces;

namespace WPHBookingSystem.Application.UseCases.ContactMessages
{
    public class GetContactMessageByIdUseCase
    {
        private readonly IUnitOfWork _unitOfWork;
        public GetContactMessageByIdUseCase(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ContactMessageDto?> ExecuteAsync(Guid id)
        {
            var message = await _unitOfWork.ContactMessageRepository.GetByIdAsync(id);
            if (message == null) return null;
            return new ContactMessageDto
            {
                Id = message.Id,
                Fullname = message.Fullname,
                EmailAddress = message.EmailAddress,
                PhoneNumber = message.PhoneNumber,
                Subject = message.Subject,
                Message = message.Message,
                DateCreated = message.DateCreated
            };
        }
    }
} 