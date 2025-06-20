using System.Threading.Tasks;
using WPHBookingSystem.Application.DTOs.Identity;

namespace WPHBookingSystem.Infrastructure.Identity;

public interface IIdentityService
{
    Task<AuthResponse> LoginAsync(LoginRequest request);
    Task<AuthResponse> RegisterAsync(RegisterRequest request);
    Task<AuthResponse> RefreshTokenAsync(string refreshToken);
    Task<bool> RevokeTokenAsync(string refreshToken);

    Task<bool> DisableAccount(string userId);

    Task<bool> EnableAccount(string userId);

    Task<bool> SetUserSingleRole(string userId, string newRole);

    Task<IList<string>> GetUserRolesAsync(string userId);

    Task<bool> DeleteAccount(string userId);

    Task<List<UserResponse>> GetAllUsersAsync(); // Assuming ApplicationUser is defined in your project

    //Task<List<ApplicationUser>>
}