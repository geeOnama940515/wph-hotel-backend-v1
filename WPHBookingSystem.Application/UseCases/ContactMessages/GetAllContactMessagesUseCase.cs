using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WPHBookingSystem.Application.DTOs.ContactMessage;
using WPHBookingSystem.Application.Interfaces;

namespace WPHBookingSystem.Application.UseCases.ContactMessages
{
    public class GetAllContactMessagesUseCase
    {
        private readonly IUnitOfWork _unitOfWork;
        public GetAllContactMessagesUseCase(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<ContactMessageDto>> ExecuteAsync()
        {
            var messages = await _unitOfWork.ContactMessageRepository.GetAllAsync();
            return messages.Select(message => new ContactMessageDto
            {
                Id = message.Id,
                Fullname = message.Fullname,
                EmailAddress = message.EmailAddress,
                PhoneNumber = message.PhoneNumber,
                Subject = message.Subject,
                Message = message.Message,
                DateCreated = message.DateCreated
            }).ToList();
        }
    }
} 