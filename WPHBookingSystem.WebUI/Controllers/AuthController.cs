using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WPHBookingSystem.Application.DTOs.Identity;
using WPHBookingSystem.Infrastructure.Identity;
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
    [Authorize]
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
        /// </summary>

        [AllowAnonymous]
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
        ///</summary>


        [AllowAnonymous]
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

        [HttpPost("disable-account/{userId}")]
        public async Task<IActionResult> DisableAccount(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogWarning("Disable account request with empty user ID");
                return this.CreateResponse(400, "User ID is required");
            }

            _logger.LogInformation("Disabling account for user {UserId}", userId);

            try
            {
                var isSuccess = await _identityService.DisableAccount(userId);
                if (!isSuccess)
                {
                    _logger.LogWarning("Failed to disable account for user {UserId}", userId);
                    return this.CreateResponse(400, "Failed to disable account");
                }

                _logger.LogInformation("Account disabled successfully for user {UserId}", userId);
                return this.CreateResponse(200, "Account disabled successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error disabling account for user {UserId}", userId);
                return this.CreateResponse(500, $"An error occurred while processing your request: {ex.Message}");
            }
        }

        [HttpPost("add-role")]
        public async Task<IActionResult> CreateRole([FromBody] string roleName)
        {
            if (string.IsNullOrEmpty(roleName))
            {
                _logger.LogWarning("Add role request with empty role name");
                return this.CreateResponse(400, "Role name is required");
            }
            _logger.LogInformation("Creating role {RoleName}", roleName);
            try
            {
                var isSuccess = await _identityService.AddRole(roleName);
                if (!isSuccess)
                {
                    _logger.LogWarning("Failed to create role {RoleName}", roleName);
                    return this.CreateResponse(400, "Failed to create role");
                }
                _logger.LogInformation("Role {RoleName} created successfully", roleName);
                return this.CreateResponse(200, "Role created successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating role {RoleName}", roleName);
                return this.CreateResponse(500, $"An error occurred while processing your request: {ex.Message}");
            }
        }



        [HttpPost("enable-account/{userId}")]
        public async Task<IActionResult> EnableAccount(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogWarning("Enable account request with empty user ID");
                return this.CreateResponse(400, "User ID is required");
            }

            _logger.LogInformation("Enabling account for user {UserId}", userId);

            try
            {
                var isSuccess = await _identityService.EnableAccount(userId);
                if (!isSuccess)
                {
                    _logger.LogWarning("Failed to enable account for user {UserId}", userId);
                    return this.CreateResponse(400, "Failed to enable account");
                }

                _logger.LogInformation("Account enabled successfully for user {UserId}", userId);
                return this.CreateResponse(200, "Account enabled successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error enabling account for user {UserId}", userId);
                return this.CreateResponse(500, $"An error occurred while processing your request: {ex.Message}");
            }
        }

        [HttpPost("delete-account/{userId}")]
        public async Task<IActionResult> DeleteAccount(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogWarning("Delete account request with empty user ID");
                return this.CreateResponse(400, "User ID is required");
            }
            _logger.LogInformation("Deleting account for user {UserId}", userId);
            try
            {
                var isSuccess = await _identityService.DeleteAccount(userId);
                if (!isSuccess)
                {
                    _logger.LogWarning("Failed to delete account for user {UserId}", userId);
                    return this.CreateResponse(400, "Failed to delete account");
                }
                _logger.LogInformation("Account deleted successfully for user {UserId}", userId);
                return this.CreateResponse(200, "Account deleted successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting account for user {UserId}", userId);
                return this.CreateResponse(500, $"An error occurred while processing your request: {ex.Message}");
            }
        }


        [HttpPost("set-role")]
        public async Task<IActionResult> AddRoleToAccount([FromBody] AddRoleRequest request)
        {
            if (string.IsNullOrEmpty(request.UserId) || string.IsNullOrEmpty(request.RoleName))
            {
                _logger.LogWarning("Add role request with empty user ID or role name");
                return this.CreateResponse(400, "User ID and role name are required");
            }
            _logger.LogInformation("Adding role {RoleName} to user {UserId}", request.RoleName, request.UserId);
            try
            {
                var result = await _identityService.SetUserSingleRole(request.UserId, request.RoleName);
                if (!result)
                {
                    _logger.LogWarning("Failed to add role {RoleName} to user {UserId}", request.RoleName, request.UserId);
                    return this.CreateResponse(400, "Failed to set role to account");
                }
                _logger.LogInformation("Role {RoleName} added successfully to user {UserId}", request.RoleName, request.UserId);
                return this.CreateResponse(200, "Role set successfully", result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding role {RoleName} to user {UserId}", request.RoleName, request.UserId);
                return this.CreateResponse(500, $"An error occurred while processing your request: {ex.Message}");
            }
        }

        [HttpGet("list-users")]
        public async Task<IActionResult> GetUsers()
        {
            try
            {
                // Assuming you have a method in IIdentityService to get all users
                var users = await _identityService.GetAllUsersAsync();
                if (users == null || users.Count == 0)
                {
                    _logger.LogInformation("No users found");
                    return this.CreateResponse(404, "No users found");
                }
                _logger.LogInformation("Retrieved {UserCount} users", users.Count);
                return this.CreateResponse(200, "Users retrieved successfully", users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving users");
                return this.CreateResponse(500, $"An error occurred while processing your request: {ex.Message}");
            }
        }



    }

    public class AddRoleRequest
    {
        public string UserId { get; set; }
        public string RoleName { get; set; }
    }


}