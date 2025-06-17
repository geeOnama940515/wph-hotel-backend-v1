using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WPHBookingSystem.Application.Common;
using WPHBookingSystem.Application.DTOs.Identity;
using WPHBookingSystem.Application.Interfaces.Services;

namespace WPHBookingSystem.WebUI.Controllers
{
    /// <summary>
    /// Authentication controller providing user authentication and authorization endpoints.
    /// 
    /// This controller handles user registration, login, token refresh, and token revocation.
    /// It uses the IIdentityService to perform authentication operations and returns
    /// standardized Result objects with JWT tokens and user information.
    /// 
    /// All endpoints are publicly accessible (no authentication required) as they handle
    /// the authentication process itself.
    /// </summary>
    [AllowAnonymous]
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IIdentityService _identityService;

        /// <summary>
        /// Initializes a new instance of the AuthController with the identity service.
        /// </summary>
        /// <param name="identityService">Service for handling user authentication and identity operations</param>
        public AuthController(IIdentityService identityService)
        {
            _identityService = identityService;
        }

        /// <summary>
        /// Authenticates a user with email and password, returning JWT tokens.
        /// 
        /// Validates user credentials and returns access and refresh tokens upon successful authentication.
        /// The access token should be included in subsequent API requests for protected endpoints.
        /// </summary>
        /// <param name="request">Login credentials (email and password)</param>
        /// <returns>Authentication response with tokens and user information</returns>
        /// <response code="200">Login successful, returns tokens and user info</response>
        /// <response code="400">Invalid credentials or authentication failed</response>
        [HttpPost("login")]
        public async Task<ActionResult<Result<AuthResponse>>> Login(LoginRequest request)
        {
            var response = await _identityService.LoginAsync(request);
            if (!response.Success)
                return BadRequest(Result<AuthResponse>.Failure(response.Message));

            return Ok(Result<AuthResponse>.Success(response, "Login successful"));
        }

        /// <summary>
        /// Registers a new user account with the provided information.
        /// 
        /// Creates a new user account and automatically logs them in by returning
        /// authentication tokens. Validates email uniqueness and password requirements.
        /// </summary>
        /// <param name="request">User registration information (name, email, password)</param>
        /// <returns>Authentication response with tokens and user information</returns>
        /// <response code="200">Registration successful, returns tokens and user info</response>
        /// <response code="400">Invalid registration data or user already exists</response>
        [HttpPost("register")]
        public async Task<ActionResult<Result<AuthResponse>>> Register(RegisterRequest request)
        {
            var response = await _identityService.RegisterAsync(request);
            if (!response.Success)
                return BadRequest(Result<AuthResponse>.Failure(response.Message));

            return Ok(Result<AuthResponse>.Success(response, "Registration successful"));
        }

        /// <summary>
        /// Refreshes an expired access token using a valid refresh token.
        /// 
        /// Allows users to obtain new access tokens without re-authenticating,
        /// as long as their refresh token is still valid and not expired.
        /// </summary>
        /// <param name="refreshToken">The refresh token to use for generating new access token</param>
        /// <returns>Authentication response with new tokens</returns>
        /// <response code="200">Token refresh successful</response>
        /// <response code="400">Invalid or expired refresh token</response>
        [HttpPost("refresh-token")]
        public async Task<ActionResult<Result<AuthResponse>>> RefreshToken([FromBody] string refreshToken)
        {
            var response = await _identityService.RefreshTokenAsync(refreshToken);
            if (!response.Success)
                return BadRequest(Result<AuthResponse>.Failure(response.Message));

            return Ok(Result<AuthResponse>.Success(response, "Token refreshed successfully"));
        }

        /// <summary>
        /// Revokes a refresh token, invalidating it for future use.
        /// 
        /// Used for logout functionality to ensure the refresh token cannot be used
        /// to generate new access tokens. This provides security by allowing users
        /// to explicitly invalidate their session.
        /// </summary>
        /// <param name="refreshToken">The refresh token to revoke</param>
        /// <returns>Boolean indicating whether the token was successfully revoked</returns>
        /// <response code="200">Token revocation successful</response>
        [HttpPost("revoke-token")]
        public async Task<ActionResult<Result<bool>>> RevokeToken([FromBody] string refreshToken)
        {
            var result = await _identityService.RevokeTokenAsync(refreshToken);
            return Ok(Result<bool>.Success(result, "Token revoked successfully"));
        }
    }
} 