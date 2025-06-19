using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using WPHBookingSystem.Application.DTOs.ContactMessage;
using WPHBookingSystem.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;

namespace WPHBookingSystem.WebUI.Controllers
{
    [ApiController]
    [Route("api/contact-messages")]
    public class ContactMessageController : ControllerBase
    {
        private readonly IBookingSystemFacade _facade;
        public ContactMessageController(IBookingSystemFacade facade)
        {
            _facade = facade;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateContactMessageDto dto)
        {
            var result = await _facade.CreateContactMessage(dto);
            if (!result.IsSuccess) return BadRequest(result.Message);
            return Ok(result);
        }

        [HttpGet]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _facade.GetAllContactMessages();
            if (!result.IsSuccess) return BadRequest(result.Message);
            return Ok(result);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _facade.GetContactMessageById(id);
            if (!result.IsSuccess) return NotFound(result.Message);
            return Ok(result);
        }

        [HttpPost("reply/{messageId}")]
        public async Task<IActionResult> Reply(Guid messageId,[FromBody] ReplyContactMessageDto dto)
        {

            var getMessage = await _facade.GetContactMessageById(messageId);

            var result = await _facade.ReplyToContactMessage(dto.Subject, dto.Email, dto.Body);
            if (!result.IsSuccess) return BadRequest(result.Message);
            return Ok(result);
        }
    }

    public class ReplyContactMessageDto
    {
        public string Subject { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
    }
} 