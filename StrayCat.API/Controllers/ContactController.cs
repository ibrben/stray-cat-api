
using Microsoft.AspNetCore.Mvc;

using StrayCat.Application.DTOs;
using StrayCat.Application.Interfaces;

namespace StrayCat.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContactController : ControllerBase
    {
        private readonly ILineMessagingService _lineMessagingService;
        private readonly IConfiguration _configuration;
        
        public ContactController(ILineMessagingService lineMessagingService, IConfiguration configuration)
        {
            _lineMessagingService = lineMessagingService;
            _configuration = configuration;
        }

        [HttpPost]
        public async Task<IActionResult> Submit(ContactUsDto contactUsDto)
        {
            var groupId = _configuration["LineMessaging:ToGroupId"];
            var msg = $"Name: {contactUsDto.Name}\nEmail: {contactUsDto.Email}\nPhone: {contactUsDto.Phone}\nCategory: {contactUsDto.Category}\nMessage: {contactUsDto.Message}";
            await _lineMessagingService.SendMessageAsync(groupId, msg);
            return Ok(new ContactUsResponseDto { Success = true, Message = "Message sent successfully" });
        }
    }
}