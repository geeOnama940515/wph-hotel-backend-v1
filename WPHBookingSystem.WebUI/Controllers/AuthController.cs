using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using WPHBookingSystem.Application.Common;
using WPHBookingSystem.Application.DTOs.Identity;
using WPHBookingSystem.Application.Interfaces.Services;
using WPHBookingSystem.WebUI.Extensions;

namespace WPHBookingSystem.WebUI.Controllers
{
    /// <summary>
    /// Authentication controller providing user authentication and authorization endpoints.
    /// 
    /// This controller handles user registration, login, token refresh, and token revocation.
    /// It uses the IIdentityService to perform authentication operations and returns
    /// standardized responses using the ControllerExtensions for consistency.
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
        private readonly ILogger<AuthController> _logger;

        /// <summary>
        /// Initializes a new instance of the AuthController with the identity service.
        /// </summary>
        /// <param name="identityService">Service for handling user authentication and identity operations</param>
        /// <param name="logger">Logger for authentication operations</param>
        public AuthController(IIdentityService identityService, ILogger<AuthController> logger)
        {
            _identityService = identityService;
            _logger = logger;
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
        public async Task<IActionResult> Login(LoginRequest request)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for login request");
                return this.CreateResponse(400, "Invalid request data");
            }

            _logger.LogInformation("Login attempt for user {Email}", request.Email);
            
            try
            {
                var response = await _identityService.LoginAsync(request);
                if (!response.Success)
                {
                    _logger.LogWarning("Login failed for user {Email}", request.Email);
                    return this.CreateResponse(400, response.Message);
                }

                _logger.LogInformation("Login successful for user {Email}", request.Email);
                return this.CreateResponse(200, "Login successful", response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login for user {Email}", request.Email);
                return this.CreateResponse(500, $"An error occurred while processing your request: {ex.Message}");
            }
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
        public async Task<IActionResult> Register(RegisterRequest request)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for registration request");
                return this.CreateResponse(400, "Invalid request data");
            }

            _logger.LogInformation("Registration attempt for user {Email}", request.Email);
            
            try
            {
                var response = await _identityService.RegisterAsync(request);
                if (!response.Success)
                {
                    _logger.LogWarning("Registration failed for user {Email}", request.Email);
                    return this.CreateResponse(400, response.Message);
                }

                _logger.LogInformation("Registration successful for user {Email}", request.Email);
                return this.CreateResponse(200, "Registration successful", response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during registration for user {Email}", request.Email);
                return this.CreateResponse(500, $"An error occurred while processing your request: {ex.Message}");
            }
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
        public async Task<IActionResult> RefreshToken([FromBody] string refreshToken)
        {
            if (string.IsNullOrEmpty(refreshToken))
            {
                _logger.LogWarning("Refresh token request with empty token");
                return this.CreateResponse(400, "Refresh token is required");
            }

            _logger.LogInformation("Token refresh attempt");
            
            try
            {
                var response = await _identityService.RefreshTokenAsync(refreshToken);
                if (!response.Success)
                {
                    _logger.LogWarning("Token refresh failed");
                    return this.CreateResponse(400, response.Message);
                }

                _logger.LogInformation("Token refresh successful");
                return this.CreateResponse(200, "Token refreshed successfully", response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during token refresh");
                return this.CreateResponse(500, $"An error occurred while processing your request: {ex.Message}");
            }
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
        public async Task<IActionResult> RevokeToken([FromBody] string refreshToken)
        {
            if (string.IsNullOrEmpty(refreshToken))
            {
                _logger.LogWarning("Token revocation request with empty token");
                return this.CreateResponse(400, "Refresh token is required");
            }

            _logger.LogInformation("Token revocation attempt");
            
            try
            {
                var result = await _identityService.RevokeTokenAsync(refreshToken);
                _logger.LogInformation("Token revocation successful");
                return this.CreateResponse(200, "Token revoked successfully", result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during token revocation");
                return this.CreateResponse(500, $"An error occurred while processing your request: {ex.Message}");
            }
        }
    }
} 