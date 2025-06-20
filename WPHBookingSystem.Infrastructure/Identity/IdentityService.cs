using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using WPHBookingSystem.Application.DTOs.Identity;
using WPHBookingSystem.Application.UseCases.ContactMessages;

namespace WPHBookingSystem.Infrastructure.Identity
{
    /// <summary>
    /// Service responsible for user authentication and authorization using ASP.NET Core Identity.
    /// Handles user registration, login, JWT token generation, and role management.
    /// 
    /// This service provides secure authentication mechanisms including password validation,
    /// JWT token generation with claims, and refresh token support for enhanced security.
    /// </summary>
    public class IdentityService : IIdentityService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Initializes a new instance of the IdentityService with required dependencies.
        /// </summary>
        /// <param name="userManager">The ASP.NET Core Identity user manager for user operations.</param>
        /// <param name="configuration">The configuration containing JWT settings.</param>
        public IdentityService(
            UserManager<ApplicationUser> userManager,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }

        /// <summary>
        /// Authenticates a user with email and password, returning JWT token on success.
        /// Validates user credentials and generates authentication response with tokens.
        /// </summary>
        /// <param name="request">The login request containing email and password.</param>
        /// <returns>Authentication response with success status and tokens or error message.</returns>
        public async Task<AuthResponse> LoginAsync(LoginRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                return new AuthResponse
                {
                    Success = false,
                    Message = "User not found."
                };
            }

            var result = await _userManager.CheckPasswordAsync(user, request.Password);
            if (!result)
            {
                return new AuthResponse
                {
                    Success = false,
                    Message = "Invalid password."
                };
            }

            return await GenerateAuthResponse(user);
        }

        /// <summary>
        /// Registers a new user with the provided information and assigns default role.
        /// Creates user account and generates authentication response with tokens.
        /// </summary>
        /// <param name="request">The registration request containing user information.</param>
        /// <returns>Authentication response with success status and tokens or error message.</returns>
        public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
        {
            var existingUser = await _userManager.FindByEmailAsync(request.Email);
            if (existingUser != null)
            {
                return new AuthResponse
                {
                    Success = false,
                    Message = "User already exists."
                };
            }

            var user = new ApplicationUser
            {
                UserName = request.Email,
                Email = request.Email,
                Firstname = request.FirstName,
                LastName = request.LastName,
                PhoneNumber = request.PhoneNumber
            };

            var result = await _userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
            {
                return new AuthResponse
                {
                    Success = false,
                    Message = string.Join(", ", result.Errors.Select(e => e.Description))
                };
            }

            // Assign default role
            await _userManager.AddToRoleAsync(user, "HotelManager");

            return await GenerateAuthResponse(user);
        }


        public async Task<List<UserResponse>> GetAllUsersAsync()
        {
            var users = await _userManager.Users.Where(x => x.isDeleted == false).ToListAsync();
            var userResponses = new List<UserResponse>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);

                userResponses.Add(new UserResponse
                {
                    FirstName = user.Firstname,
                    LastName = user.LastName,
                    Email = user.Email,
                    UserId = user.Id,
                    PhoneNumber = user.PhoneNumber,
                    Roles = roles.ToList() // This matches your updated model
                });
            }

            return userResponses;
        }



        /// <summary>
        /// Refreshes an authentication token using a refresh token.
        /// Not yet implemented - placeholder for future enhancement.
        /// </summary>
        /// <param name="refreshToken">The refresh token to use for generating a new access token.</param>
        /// <returns>Authentication response with new tokens.</returns>
        /// <exception cref="NotImplementedException">Thrown as this feature is not yet implemented.</exception>
        public async Task<AuthResponse> RefreshTokenAsync( string refreshToken)
        {
            //var applicationUser = await _userManager.Users.FirstOrDefaultAsync(u => u.RefreshToken);
            // Implement refresh token logic here
            throw new NotImplementedException();
        }

        public async Task<bool> DisableAccount(string userId)
        {
            var applicationUser = await _userManager.FindByIdAsync(userId);

            if (applicationUser == null)
            {
                return false; // User not found
            }
            var result = await _userManager.SetLockoutEndDateAsync(applicationUser, DateTimeOffset.UtcNow.AddYears(100));

            return result.Succeeded; // Lock the account for 100 years
        }

        /// <summary>
        /// Revokes a refresh token to invalidate it.
        /// Not yet implemented - placeholder for future enhancement.
        /// </summary>
        /// <param name="refreshToken">The refresh token to revoke.</param>
        /// <returns>True if token was successfully revoked, false otherwise.</returns>
        /// <exception cref="NotImplementedException">Thrown as this feature is not yet implemented.</exception>
        public async Task<bool> RevokeTokenAsync(string refreshToken)
        {
            // Implement token revocation logic here
            throw new NotImplementedException();
        }

        /// <summary>
        /// Generates authentication response with JWT token and user information.
        /// Creates JWT token with user claims and generates refresh token.
        /// </summary>
        /// <param name="user">The authenticated user.</param>
        /// <returns>Complete authentication response with tokens and user details.</returns>
        private async Task<AuthResponse> GenerateAuthResponse(ApplicationUser user)
        {
            var roles = await _userManager.GetRolesAsync(user);
            var role = roles.FirstOrDefault() ?? "Administrator";

            var token = GenerateJwtToken(user, role);
            var refreshToken = GenerateRefreshToken();

            return new AuthResponse
            {
                Token = token,
                RefreshToken = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddHours(1),
                UserId = user.Id,
                Email = user.Email,
                FirstName = user.Firstname,
                LastName = user.LastName,
                Role = role,
                Success = true,
                Message = "Authentication successful."
            };
        }

        /// <summary>
        /// Generates a JWT token for the specified user with role-based claims.
        /// Creates secure token with user identity and role information.
        /// </summary>
        /// <param name="user">The user for whom to generate the token.</param>
        /// <param name="role">The user's role to include in the token claims.</param>
        /// <returns>JWT token string.</returns>
        private string GenerateJwtToken(ApplicationUser user, string role)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.GivenName, user.Firstname),
                new Claim(ClaimTypes.Surname, user.LastName),
                new Claim(ClaimTypes.Role, role)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:Key"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.UtcNow.AddHours(1);

            var token = new JwtSecurityToken(
                issuer: _configuration["JwtSettings:Issuer"],
                audience: _configuration["JwtSettings:Audience"],
                claims: claims,
                expires: expires,
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        /// <summary>
        /// Generates a refresh token for token renewal functionality.
        /// Creates a unique identifier that can be used to refresh access tokens.
        /// </summary>
        /// <returns>Refresh token string.</returns>
        private string GenerateRefreshToken()
        {
            return Guid.NewGuid().ToString();
        }

        public async Task<bool> EnableAccount(string userId)
        {
            var applicationUser = await _userManager.FindByIdAsync(userId);
            if (applicationUser == null)
            {
                return false; // User not found
            }
            var result = await _userManager.SetLockoutEndDateAsync(applicationUser, null); // Remove lockout
            return result.Succeeded; // Enable the account by removing lockout
        }

        public async Task<bool> SetUserSingleRole(string userId, string newRole)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return false;

            var currentRoles = await _userManager.GetRolesAsync(user);

            // Remove all existing roles
            var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
            if (!removeResult.Succeeded)
                return false;

            // Add the new role
            var addResult = await _userManager.AddToRoleAsync(user, newRole);
            return addResult.Succeeded;
        }

        public async Task<IList<string>> GetUserRolesAsync(string userId)
        {
            var applicationUser = await _userManager.FindByIdAsync(userId);
            if (applicationUser == null)
            {
                return new List<string>(); // User not found, return empty list
            }
            var roles = await _userManager.GetRolesAsync(applicationUser);
            return roles; // Return list of roles for the user
        }

        public async Task<bool> DeleteAccount(string userId)
        {
            var applicationUser = await _userManager.FindByIdAsync(userId);
            if (applicationUser == null)
            {
                return false; // User not found
            }
            applicationUser.isDeleted = true; // Mark user as deleted
            var result = await _userManager.UpdateAsync(applicationUser);
            if (!result.Succeeded)
            {
                return false; // Update failed
            }
            return true;
        }
    }
} 