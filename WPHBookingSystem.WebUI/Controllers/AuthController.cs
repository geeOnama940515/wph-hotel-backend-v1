using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WPHBookingSystem.Application.DTOs.Identity;
using WPHBookingSystem.Application.Interfaces.Services;

namespace WPHBookingSystem.WebUI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IIdentityService _identityService;

        public AuthController(IIdentityService identityService)
        {
            _identityService = identityService;
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponse>> Login(LoginRequest request)
        {
            var response = await _identityService.LoginAsync(request);
            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }

        [HttpPost("register")]
        public async Task<ActionResult<AuthResponse>> Register(RegisterRequest request)
        {
            var response = await _identityService.RegisterAsync(request);
            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }

        [HttpPost("refresh-token")]
        public async Task<ActionResult<AuthResponse>> RefreshToken([FromBody] string refreshToken)
        {
            var response = await _identityService.RefreshTokenAsync(refreshToken);
            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }

        [HttpPost("revoke-token")]
        public async Task<ActionResult<bool>> RevokeToken([FromBody] string refreshToken)
        {
            var result = await _identityService.RevokeTokenAsync(refreshToken);
            return Ok(result);
        }
    }
} 